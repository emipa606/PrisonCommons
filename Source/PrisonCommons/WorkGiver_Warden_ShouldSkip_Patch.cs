﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonCommons;

[HarmonyPatch(typeof(WorkGiver_Warden), nameof(WorkGiver_Warden.ShouldSkip))]
internal static class WorkGiver_Warden_ShouldSkip_Patch
{
    public static void Postfix(ref bool __result, Pawn pawn, bool forced)
    {
        if (pawn.IsPrisoner)
        {
            __result = true;
        }
    }
}