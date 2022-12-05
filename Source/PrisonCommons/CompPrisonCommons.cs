using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class CompPrisonCommons : ThingComp
{
    private static readonly Texture2D ForPrisonersTex = ContentFinder<Texture2D>.Get("UI/Commands/ForPrisoners");

    private bool isPrisonCommons;

    public bool Active
    {
        get => isPrisonCommons;
        set
        {
            if (isPrisonCommons == value)
            {
                return;
            }

            isPrisonCommons = value;
            if (parent.Spawned)
            {
                PrisonCommons.PrisonCommons.SetPrisonCommons(this, isPrisonCommons);
            }
        }
    }

    public override void PostExposeData()
    {
        Scribe_Values.Look(ref isPrisonCommons, "isPrisonCommons");
    }

    public override void PostDeSpawn(Map map)
    {
        base.PostDeSpawn(map);
        if (Active)
        {
            PrisonCommons.PrisonCommons.SetPrisonCommons(this, false);
        }
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        base.PostDestroy(mode, previousMap);
        if (Active)
        {
            PrisonCommons.PrisonCommons.SetPrisonCommons(this, false);
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (parent.Faction != Faction.OfPlayer && !respawningAfterLoad)
        {
            isPrisonCommons = false;
        }

        if (Active)
        {
            PrisonCommons.PrisonCommons.SetPrisonCommons(this, true);
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        var command_Toggle = new Command_Toggle
        {
            defaultLabel = "CommandPrisonCommonsToggleLabel".Translate(),
            icon = ForPrisonersTex,
            isActive = () => Active,
            toggleAction = delegate { Active = !Active; },
            defaultDesc = Active
                ? "CommandPrisonCommonsToggleDescActive".Translate()
                : "CommandPrisonCommonsToggleDescInactive".Translate()
        };

        yield return command_Toggle;
    }
}