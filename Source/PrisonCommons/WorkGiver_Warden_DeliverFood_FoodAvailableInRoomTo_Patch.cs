using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonCommons;

[HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood), "FoodAvailableInRoomTo")]
internal static class WorkGiver_Warden_DeliverFood_FoodAvailableInRoomTo_Patch
{
    public static bool Postfix(bool __result, Pawn prisoner)
    {
        if (__result)
        {
            return true;
        }

        var room = prisoner.GetRoom();
        if (room == null)
        {
            return false;
        }

        var seenRooms = new HashSet<Room>();
        float neededNutrition = 0f, availableNutrition = 0f;
        CountNutrition(seenRooms, prisoner, room, ref neededNutrition, ref availableNutrition);

        return availableNutrition + 0.5f >= neededNutrition;
    }

    [HarmonyReversePatch]
    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood), "NutritionAvailableForFrom")]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float NutritionAvailableForFrom(Pawn p, Thing foodSource)
    {
        throw new NotImplementedException("stub method should not be callable");
    }

    private static void CountNutrition(HashSet<Room> seenRooms, Pawn prisoner, Room room, ref float neededNutrition,
        ref float availableNutrition)
    {
        if (!seenRooms.Add(room))
        {
            return;
        }

        var regions = room.Regions;
        foreach (var region in regions)
        {
            var things = region.ListerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
            foreach (var thing in things)
            {
                if (!thing.def.IsIngestible ||
                    thing.def.ingestible.preferability > FoodPreferability.DesperateOnlyForHumanlikes)
                {
                    availableNutrition += NutritionAvailableForFrom(prisoner, thing);
                }
            }

            var pawns = region.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
            foreach (var thing in pawns)
            {
                if (thing is Pawn { IsPrisonerOfColony: true } pawn &&
                    pawn.needs.food.CurLevelPercentage < pawn.needs.food.PercentageThreshHungry + 0.02f &&
                    (pawn.carryTracker.CarriedThing == null || !pawn.WillEat_NewTemp(pawn.carryTracker.CarriedThing)))
                {
                    neededNutrition += pawn.needs.food.NutritionWanted;
                }
            }

            // second part: also spread to accessible rooms
            foreach (var link in region.links)
            {
                var other = link.GetOtherRegion(region);
                if (other.type != RegionType.Portal || !seenRooms.Add(other.Room) ||
                    !PrisonCommons.IsAllowedDoorway(prisoner, other.Room))
                {
                    continue;
                }

                foreach (var neighbor in other.Neighbors)
                {
                    CountNutrition(seenRooms, prisoner, neighbor.Room, ref neededNutrition, ref availableNutrition);
                }
            }
        }
    }
}