using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonCommons;

[HarmonyPatch(typeof(WorkGiver_Warden), nameof(WorkGiver_Warden.ShouldSkip))]
internal static class WorkGiver_Warden_ShouldSkip
{
    public static void Postfix(ref bool __result, Pawn pawn)
    {
        if (pawn.IsPrisoner)
        {
            __result = true;
        }
    }
}