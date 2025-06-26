using HarmonyLib;
using Verse;

namespace PrisonCommons;

[HarmonyPatch(typeof(Room), nameof(Room.IsPrisonCell), MethodType.Getter)]
internal static class Room_IsPrisonCell
{
    public static void Postfix(ref bool __result, Room __instance)
    {
        if (!__result)
        {
            __result = PrisonCommons.IsPrisonCommons(__instance);
        }
    }
}