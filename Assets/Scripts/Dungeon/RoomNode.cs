using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public RectInt room;
    public List<RoomNode> connections = new();
}