using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonCommons;

[StaticConstructorOnStartup]
public static class PrisonCommons
{
    private static readonly HashSet<CompPrisonCommons> comps = new HashSet<CompPrisonCommons>();

    private static readonly HashSet<Room> matchingRooms = new HashSet<Room>();
    private static readonly HashSet<Room> seenDoors = new HashSet<Room>();
    private static readonly Queue<Room> doorQueue = new Queue<Room>();

    static PrisonCommons()
    {
        new Harmony("Mlie.PrisonCommons").PatchAll(Assembly.GetExecutingAssembly());
    }

    public static bool IsPrisonCommons(Room room)
    {
        if (room.TouchesMapEdge)
        {
            return false; // room must be enclosed
        }

        foreach (var comp in comps)
        {
            if (!comp.parent.Spawned || comp.parent.Map != room.Map)
            {
                continue;
            }

            if (room == comp.parent.GetRoom())
            {
                return true;
            }
        }

        return false;
    }

    internal static void SetPrisonCommons(CompPrisonCommons comp, bool add)
    {
        if (add)
        {
            comps.Add(comp);
        }
        else
        {
            comps.Remove(comp);
        }

        if (comp.parent.Spawned)
        {
            comp.parent.GetRoom()?.Notify_RoomShapeChanged();
        }
    }

    public static bool IsAllowedDoorway(Pawn p, Room room)
    {
        if (p == null || room is not { IsDoorway: true })
        {
            return false;
        }

        var prisonerBed = p.ownership.OwnedBed;
        var prisonerBedroom = prisonerBed?.GetRoom();

        matchingRooms.Clear();
        seenDoors.Clear();
        seenDoors.Add(room);
        doorQueue.Clear();
        doorQueue.Enqueue(room);

        while (doorQueue.Count != 0)
        {
            var door = doorQueue.Dequeue();
            foreach (var neighbor in door.Districts[0].Neighbors)
            {
                var neighborRoom = neighbor.Room;

                if (neighborRoom.IsDoorway && seenDoors.Add(neighborRoom))
                {
                    doorQueue.Enqueue(neighborRoom);
                }

                if (neighborRoom == prisonerBedroom || IsPrisonCommons(neighborRoom))
                {
                    matchingRooms.Add(neighborRoom);
                }
            }
        }

        return matchingRooms.Count >= 2;
    }
}