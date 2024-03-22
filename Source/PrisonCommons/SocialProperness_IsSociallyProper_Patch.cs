using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonCommons;

[HarmonyPatch(typeof(SocialProperness), nameof(SocialProperness.IsSociallyProper), typeof(Thing), typeof(Pawn),
    typeof(bool), typeof(bool))]
internal static class SocialProperness_IsSociallyProper_Patch
{
    public static void Postfix(ref bool __result, Thing t, Pawn p, bool forPrisoner)
    {
        if (__result || !forPrisoner || p == null)
        {
            return;
        }

        var interactionSpot = t.def.hasInteractionCell ? t.InteractionCell : t.Position;
        var interactionRoom = interactionSpot.GetRoomOrAdjacent(t.Map);

        if (interactionRoom == null)
        {
            return;
        }

        __result = PrisonCommons.IsPrisonCommons(interactionRoom) ||
                   PrisonCommons.IsAllowedDoorway(p, interactionRoom);

        if (__result)
        {
            return;
        }

        var prisonerBed = p.ownership.OwnedBed;
        var prisonerBedroom = prisonerBed?.GetRoom();

        __result = interactionRoom == prisonerBedroom;
    }
}