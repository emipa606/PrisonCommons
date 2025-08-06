using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonCommons;

[StaticConstructorOnStartup]
public static class PrisonCommons
{
    private static readonly HashSet<CompPrisonCommons> comps = [];

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

        Room firstMatchingRoom = null;
        HashSet<Room> seenDoors = [room];
        Queue<Room> doorQueue = new();
        doorQueue.Enqueue(room);

        while (doorQueue.Count != 0)
        {
            var door = doorQueue.Dequeue();
            foreach (var neighbor in door.Districts[0].Neighbors)
            {
                var neighborRoom = neighbor.Room;

                if (neighborRoom == prisonerBedroom || IsPrisonCommons(neighborRoom))
                {
                    if (firstMatchingRoom is null)
                    {
                        // matchingRooms.Count >= 1
                        firstMatchingRoom = neighborRoom;
                    }
                    else if (firstMatchingRoom != neighborRoom)
                    {
                        // matchingRooms.Count >= 2
                        return true;
                    }
                }
                else if (neighborRoom.IsDoorway && seenDoors.Add(neighborRoom))
                {
                    doorQueue.Enqueue(neighborRoom);
                }
            }
        }

        return false;
    }
}
