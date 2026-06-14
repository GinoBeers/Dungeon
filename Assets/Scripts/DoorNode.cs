using System.Collections.Generic;
using UnityEngine;

public class DoorNode
{
    public RectInt rect;
    public RoomNode a;
    public RoomNode b;

    private HashSet<RoomNode> visitedRooms = new();
    private HashSet<DoorNode> visitedDoors = new();
}