using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField]
    private RectInt dungeonBounds = new RectInt(0, 0, 100, 50);

    [SerializeField]
    private int minimumRoomSize = 10;

    [SerializeField]
    private float generationDelay = 0.1f;

    private List<RectInt> toDo = new();

    private List<RectInt> done = new();

    private List<RectInt> doors = new();

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
        doors.Clear();

        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();

        toDo.Add(dungeonBounds);    

        while (toDo.Count > 0)
        {
            RectInt currentRoom = toDo[0];
            toDo.RemoveAt(0);

            bool canSplitVertically =
                currentRoom.width >= minimumRoomSize * 2;

            bool canSplitHorizontally =
                currentRoom.height >= minimumRoomSize * 2;

            if (!canSplitVertically && !canSplitHorizontally)
            {
                done.Add(currentRoom);

                DrawDungeon();

                yield return new WaitForSeconds(generationDelay);
                continue;
            }

            bool splitVertical = Random.value > 0.5f;

            if (!canSplitVertically)
                splitVertical = false;

            if (!canSplitHorizontally)
                splitVertical = true;

            RectInt roomA;
            RectInt roomB;

            if (splitVertical)
            {
                (roomA, roomB) = SplitVertically(currentRoom);

                CreateVerticalDoor(roomA, roomB);
            }
            else
            {
                (roomA, roomB) = SplitHorizontally(currentRoom);

                CreateHorizontalDoor(roomA, roomB);
            }

            toDo.Add(roomA);
            toDo.Add(roomB);

            DrawDungeon();

            yield return new WaitForSeconds(generationDelay);
        }

        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();
        DrawDungeon();
    }

    private (RectInt, RectInt) SplitVertically(RectInt room)
    {
        int splitX = Random.Range(
            minimumRoomSize,
            room.width - minimumRoomSize
        );

        RectInt roomA = new RectInt(
            room.x,
            room.y,
            splitX,
            room.height
        );

        RectInt roomB = new RectInt(
            room.x + splitX - 1,
            room.y,
            room.width - splitX + 1,
            room.height
        );

        return (roomA, roomB);
    }

    private (RectInt, RectInt) SplitHorizontally(RectInt room)
    {
        int splitY = Random.Range(
            minimumRoomSize,
            room.height - minimumRoomSize
        );

        RectInt roomA = new RectInt(
            room.x,
            room.y,
            room.width,
            splitY
        );

        RectInt roomB = new RectInt(
            room.x,
            room.y + splitY - 1,
            room.width,
            room.height - splitY + 1
        );

        return (roomA, roomB);
    }

    private void CreateVerticalDoor(RectInt roomA, RectInt roomB)
    {
        RectInt intersection = AlgorithmsUtils.Intersect(roomA, roomB);

        if (intersection.height <= 2)
            return;

        int randomY = Random.Range(
            intersection.y + 1,
            intersection.yMax - 1
        );

        RectInt door = new RectInt(
            intersection.x,
            randomY,
            1,
            1
        );

        doors.Add(door);
    }

    private void CreateHorizontalDoor(RectInt roomA, RectInt roomB)
    {
        RectInt intersection = AlgorithmsUtils.Intersect(roomA, roomB);

        if (intersection.width <= 2)
            return;

        int randomX = Random.Range(
            intersection.x + 1,
            intersection.xMax - 1
        );

        RectInt door = new RectInt(
            randomX,
            intersection.y,
            1,
            1
        );

        doors.Add(door);
    }

    private void DrawDungeon()
    {
        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();

        DebugDrawingBatcher.GetInstance().BatchCall(() =>
        {
            foreach (RectInt room in toDo)
            {
                AlgorithmsUtils.DebugRectInt(room, Color.yellow);
            }

            foreach (RectInt room in done)
            {
                AlgorithmsUtils.DebugRectInt(room, Color.green);

                if (room.width > 2 && room.height > 2)
                {
                    RectInt innerRoom = new RectInt(
                        room.x + 1,
                        room.y + 1,
                        room.width - 2,
                        room.height - 2
                    );

                    AlgorithmsUtils.DebugRectInt(innerRoom, Color.white);
                }
            }

            foreach (RectInt door in doors)
            {
                AlgorithmsUtils.DebugRectInt(door, Color.cyan);
            }
        });
    }
}