using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonCommons;

[HarmonyPatch(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.InitiatePrisonBreakMtbDays))]
internal static class PrisonBreakUtility_InitiatePrisonBreakMtbDays
{
    public static bool Prefix(ref float __result, Pawn pawn, StringBuilder sb, HashSet<Region> ___tmpRegions,
        SimpleCurve ___PrisonBreakMTBFactorForDaysSincePrisonBreak)
    {
        if (!pawn.Awake() || !PrisonBreakUtility.CanParticipateInPrisonBreak(pawn))
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

        var days = pawn.kindDef.basePrisonBreakMtbDays;

        var movementFactor = Mathf.Clamp(pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving), 0.01f, 1f);
        days /= movementFactor;
        if (sb != null && movementFactor != 1f)
        {
            sb.AppendLineIfNotEmpty();
            sb.Append("FactorForMovement".Translate() + ": " + movementFactor.ToStringPercent());
        }

        ___tmpRegions.Clear();

        var doorFactor = (float)countDoorsCombined(___tmpRegions, room);
        if (doorFactor > 0f)
        {
            days /= doorFactor;
            if (sb != null && doorFactor > 1f)
            {
                sb.AppendLineIfNotEmpty();
                sb.Append("FactorForDoorCount".Translate() + ": " + (1f / doorFactor).ToStringPercent());
            }
        }

        if (pawn.guest.everParticipatedInPrisonBreak)
        {
            var daysSincePrisonBreak =
                (Find.TickManager.TicksGame - pawn.guest.lastPrisonBreakTicks) / GenDate.TicksPerDay;
            days *= ___PrisonBreakMTBFactorForDaysSincePrisonBreak.Evaluate(daysSincePrisonBreak);
        }

        if (pawn.genes != null)
        {
            days *= pawn.genes.PrisonBreakIntervalFactor;
            if (sb != null && days != 1f)
            {
                sb.AppendLineIfNotEmpty();
                sb.Append($"  - {"FactorForGenes".Translate()}: {days.ToStringPercent()}");
            }
        }

        if (ModsConfig.AnomalyActive && pawn.health.hediffSet.HasHediff(HediffDefOf.BlissLobotomy))
        {
            days *= 10f;
            if (sb != null)
            {
                sb.AppendLineIfNotEmpty();
                sb.Append("  - " + "BlissLobotomy".Translate() + ": " + 10f.ToStringPercent());
            }
        }

        __result = days;

        return false;
    }

    private static int countDoorsCombined(HashSet<Region> ___tmpRegions, Room room)
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
                        count += countDoorsCombined(___tmpRegions, neighbor.Room);
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