using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonCommons;

[HarmonyPatch(typeof(WorkGiver_Warden_TakeToBed), nameof(WorkGiver_Warden_TakeToBed.TryMakeJob))]
internal static class WorkGiver_Warden_TakeToBed_TryMakeJob_Patch
{
    public static void Postfix(ref Job __result, bool forced)
    {
        if (forced || __result == null || __result.def != JobDefOf.EscortPrisonerToBed)
        {
            return;
        }

        var prisoner = __result.targetA.Pawn;
        var currentRoom = prisoner.GetRoom();

        // don't drag prisoners out of the commons unless instructed to by the player
        if (PrisonCommons.IsPrisonCommons(currentRoom) || PrisonCommons.IsAllowedDoorway(prisoner, currentRoom))
        {
            __result = null;
        }
    }
}