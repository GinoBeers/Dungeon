using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovedGenScript : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] private RectInt dungeonBounds = new RectInt(0, 0, 100, 50);
    [SerializeField] private int minimumRoomSize = 10;
    [SerializeField] private float generationDelay = 0.05f;

    private List<RectInt> toDo = new();
    private List<RectInt> done = new();

    private List<RoomNode> nodes = new();
    private List<DoorNode> doorNodes = new();
    private List<RectInt> doors = new();

    private HashSet<RoomNode> visitedRooms = new();
    private HashSet<DoorNode> visitedDoors = new();

    [Button("Generate Dungeon")]
    private void Generate()
    {
        StopAllCoroutines();
        StartCoroutine(GenerateDungeon());
    }

    private IEnumerator GenerateDungeon()
    {
        toDo.Clear();
        done.Clear();
        nodes.Clear();
        doorNodes.Clear();
        doors.Clear();
        visitedRooms.Clear();
        visitedDoors.Clear();

        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();

        toDo.Add(dungeonBounds);

        while (toDo.Count > 0)
        {
            RectInt current = toDo[0];
            toDo.RemoveAt(0);

            bool canSplitH = current.height >= minimumRoomSize * 2;
            bool canSplitV = current.width >= minimumRoomSize * 2;

            if (!canSplitH && !canSplitV)
            {
                done.Add(current);
                Draw();
                yield return new WaitForSeconds(generationDelay);
                continue;
            }

            bool splitVertical = Random.value > 0.5f;

            if (!canSplitV) splitVertical = false;
            if (!canSplitH) splitVertical = true;

            RectInt a, b;

            if (splitVertical)
                (a, b) = SplitVertical(current);
            else
                (a, b) = SplitHorizontal(current);

            toDo.Add(a);
            toDo.Add(b);

            Draw();
            yield return new WaitForSeconds(generationDelay);
        }

        BuildGraph();
        BuildDoors();

        yield return StartCoroutine(ValidateConnectivity());

        Draw();
    }

    private (RectInt, RectInt) SplitVertical(RectInt r)
    {
        int splitX = Random.Range(minimumRoomSize, r.width - minimumRoomSize);

        RectInt a = new RectInt(r.x, r.y, splitX, r.height);
        RectInt b = new RectInt(r.x + splitX - 1, r.y, r.width - splitX + 1, r.height);

        return (a, b);
    }

    private (RectInt, RectInt) SplitHorizontal(RectInt r)
    {
        int splitY = Random.Range(minimumRoomSize, r.height - minimumRoomSize);

        RectInt a = new RectInt(r.x, r.y, r.width, splitY);
        RectInt b = new RectInt(r.x, r.y + splitY - 1, r.width, r.height - splitY + 1);

        return (a, b);
    }

    private void BuildGraph()
    {
        nodes.Clear();

        foreach (RectInt r in done)
        {
            RoomNode node = new RoomNode();
            node.room = r;
            node.connections = new List<RoomNode>();
            nodes.Add(node);
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = i + 1; j < nodes.Count; j++)
            {
                if (AreNeighbours(nodes[i].room, nodes[j].room))
                {
                    nodes[i].connections.Add(nodes[j]);
                    nodes[j].connections.Add(nodes[i]);
                }
            }
        }
    }

    private bool AreNeighbours(RectInt a, RectInt b)
    {
        RectInt inter = AlgorithmsUtils.Intersect(a, b);
        return inter.width > 0 || inter.height > 0;
    }

    private void BuildDoors()
    {
        doors.Clear();
        doorNodes.Clear();

        HashSet<(RoomNode, RoomNode)> processed = new();

        foreach (RoomNode a in nodes)
        {
            foreach (RoomNode b in a.connections)
            {
                if (processed.Contains((a, b)) || processed.Contains((b, a)))
                    continue;

                RectInt inter = AlgorithmsUtils.Intersect(a.room, b.room);

                if (inter.width <= 0 && inter.height <= 0)
                    continue;

                RectInt door = CreateDoor(inter);

                if (door.width > 0 && door.height > 0)
                {
                    doors.Add(door);

                    DoorNode dn = new DoorNode();
                    dn.rect = door;
                    dn.a = a;
                    dn.b = b;

                    doorNodes.Add(dn);
                }

                processed.Add((a, b));
            }
        }
    }

    private RectInt CreateDoor(RectInt inter)
    {
        int minDoorSpace = 5;

        if (inter.width > inter.height)
        {
            if (inter.width < minDoorSpace) return default;

            int margin = minDoorSpace / 2;

            int min = inter.x + margin;
            int max = inter.xMax - margin;

            if (max <= min) return default;

            int x = Random.Range(min, max);
            return new RectInt(x, inter.y, 1, 1);
        }
        else
        {
            if (inter.height < minDoorSpace) return default;

            int margin = minDoorSpace / 2;

            int min = inter.y + margin;
            int max = inter.yMax - margin;

            if (max <= min) return default;

            int y = Random.Range(min, max);
            return new RectInt(inter.x, y, 1, 1);
        }
    }

    private IEnumerator ValidateConnectivity()
    {
        visitedRooms.Clear();
        visitedDoors.Clear();

        if (nodes.Count == 0)
            yield break;

        yield return StartCoroutine(DFS(nodes[0]));
    }

    private IEnumerator DFS(RoomNode start)
    {
        Stack<RoomNode> stack = new();
        stack.Push(start);

        visitedRooms.Clear();
        visitedDoors.Clear();

        while (stack.Count > 0)
        {
            RoomNode node = stack.Pop();

            if (visitedRooms.Contains(node))
                continue;

            visitedRooms.Add(node);

            Draw();
            yield return new WaitForSeconds(generationDelay);

            foreach (RoomNode next in node.connections)
            {
                if (visitedRooms.Contains(next))
                    continue;

                DoorNode door = FindDoor(node, next);

                if (door != null)
                {
                    visitedDoors.Add(door);

                    Draw();
                    yield return new WaitForSeconds(generationDelay);
                }

                stack.Push(next);
            }
        }
    }

    private DoorNode FindDoor(RoomNode a, RoomNode b)
    {
        foreach (DoorNode d in doorNodes)
        {
            if ((d.a == a && d.b == b) || (d.a == b && d.b == a))
                return d;
        }
        return null;
    }

    private void Draw()
    {
        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();

        DebugDrawingBatcher.GetInstance().BatchCall(() =>
        {
            foreach (RectInt r in toDo)
                AlgorithmsUtils.DebugRectInt(r, Color.yellow);

            foreach (RectInt r in done)
                AlgorithmsUtils.DebugRectInt(r, Color.red);

            foreach (RectInt d in doors)
                AlgorithmsUtils.DebugRectInt(d, Color.cyan);

            DrawGraphDebug();
        });
    }

    private void DrawGraphDebug()
    {
        foreach (RoomNode node in nodes)
        {
            Vector3 nodePos = GetRoomCenter(node.room);
            Color roomColor = visitedRooms.Contains(node) ? Color.blue : Color.yellow;
            DebugExtension.DebugWireSphere(nodePos, roomColor, 0.6f);

            foreach (DoorNode door in doorNodes)
            {
                if (door.a != node && door.b != node)
                    continue;

                Vector3 doorPos = new Vector3(
                    door.rect.x + door.rect.width * 0.5f,
                    0,
                    door.rect.y + door.rect.height * 0.5f
                );

                Color c = visitedDoors.Contains(door) ? Color.blue : Color.yellow;

                DebugExtension.DebugWireSphere(doorPos, c, 0.3f);

                Vector3 otherRoom = GetRoomCenter(
                    door.a == node ? door.b.room : door.a.room
                );

                Debug.DrawLine(nodePos, doorPos, c);
                Debug.DrawLine(doorPos, otherRoom, c);
            }
        }
    }

    private Vector3 GetRoomCenter(RectInt r)
    {
        return new Vector3(
            r.x + r.width * 0.5f,
            0,
            r.y + r.height * 0.5f
        );
    }
}