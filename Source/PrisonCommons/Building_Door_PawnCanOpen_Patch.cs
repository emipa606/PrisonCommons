using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonCommons;

[HarmonyPatch(typeof(Building_Door), nameof(Building_Door.PawnCanOpen))]
internal static class Building_Door_PawnCanOpen_Patch
{
    public static void Postfix(ref bool __result, Building_Door __instance, Pawn p)
    {
        if (__result)
        {
            return;
        }

        if (!p.IsPrisoner || p.guest.HostFaction != __instance.Faction)
        {
            return;
        }

        __result = PrisonCommons.IsAllowedDoorway(p, __instance.GetRoom());
    }
}