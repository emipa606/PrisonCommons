using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonCommons;

[HarmonyPatch(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.InitiatePrisonBreakMtbDays))]
internal static class PrisonBreakUtility_InitiatePrisonBreakMtbDays_Patch
{
    private static readonly float BaseInitiatePrisonBreakMtbDays =
        (float)AccessTools.Field(typeof(PrisonBreakUtility), "BaseInitiatePrisonBreakMtbDays").GetValue(null);

    private static readonly SimpleCurve PrisonBreakMTBFactorForDaysSincePrisonBreak = (SimpleCurve)AccessTools
        .Field(typeof(PrisonBreakUtility), "PrisonBreakMTBFactorForDaysSincePrisonBreak").GetValue(null);

    public static bool Prefix(ref float __result, Pawn pawn, StringBuilder sb, HashSet<Region> ___tmpRegions)
    {
        if (!pawn.Awake())
        {
            __result = -1f;

            return false;
        }

        if (!PrisonBreakUtility.CanParticipateInPrisonBreak(pawn))
        {
            __result = -1f;

            return false;
        }

        var room = pawn.GetRoom();
        if (room is not { IsPrisonCell: true })
        {
            __result = -1f;

            return false;
        }

        var days = BaseInitiatePrisonBreakMtbDays;

        var movementFactor = Mathf.Clamp(pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving), 0.01f, 1f);
        days /= movementFactor;
        if (sb != null && movementFactor != 1f)
        {
            sb.AppendLineIfNotEmpty();
            sb.Append("FactorForMovement".Translate() + ": " + movementFactor.ToStringPercent());
        }

        ___tmpRegions.Clear();

        var doorFactor = (float)CountDoorsCombined(___tmpRegions, room);
        if (doorFactor > 0f)
        {
            days /= doorFactor;
            if (sb != null && doorFactor > 1f)
            {
                sb.AppendLineIfNotEmpty();
                sb.Append("FactorForDoorCount".Translate() + ": " + doorFactor.ToStringPercent());
            }
        }

        if (pawn.guest.everParticipatedInPrisonBreak)
        {
            var daysSincePrisonBreak =
                (Find.TickManager.TicksGame - pawn.guest.lastPrisonBreakTicks) / GenDate.TicksPerDay;
            days *= PrisonBreakMTBFactorForDaysSincePrisonBreak.Evaluate(daysSincePrisonBreak);
        }

        __result = days;

        return false;
    }

    private static int CountDoorsCombined(HashSet<Region> ___tmpRegions, Room room)
    {
        var count = 0;

        var regions =
            room.Regions.ToArray(); // if we don't make a copy, the list gets mutated when the same room is found again

        foreach (var region in regions)
        {
            if (!___tmpRegions.Add(region))
            {
                continue;
            }

            foreach (var link in region.links)
            {
                var other = link.GetOtherRegion(region);
                if (other.type != RegionType.Portal || !___tmpRegions.Add(other))
                {
                    continue;
                }

                foreach (var neighbor in other.Neighbors)
                {
                    if (neighbor.Room.IsPrisonCell)
                    {
                        count += CountDoorsCombined(___tmpRegions, neighbor.Room);
                    }
                    else
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }
}