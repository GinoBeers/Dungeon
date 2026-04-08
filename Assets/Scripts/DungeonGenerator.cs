using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private RectInt dungeonBounds;

    [SerializeField]
    public List<RectInt> ToDo = new();
    public List<RectInt> Done = new();

    [SerializeField]
    private RectInt door;

    [ContextMenu("Generate dungeon")]
    [Button("Generate dungeon", EButtonEnableMode.Playmode)]
    private void Start()
    {
        StartCoroutine(GenerateDungeon());
    }

    private IEnumerator GenerateDungeon()
    {
        ToDo.Clear();
        door = RectInt.zero;

        (RectInt roomA, RectInt roomB) = SplitVertically(dungeonBounds);
        ToDo.Add(roomA);
        ToDo.Add(roomB);

        RectInt intersection = AlgorithmsUtils.Intersect(roomA, roomB);
        int randomY = UnityEngine.Random.Range(intersection.y + 1, intersection.y + intersection.height - 1);

        door = new RectInt(intersection.x, randomY, intersection.width, intersection.width);

        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();

        yield return null;

        DebugDrawingBatcher.GetInstance().BatchCall(() =>
        {
            // Draw the rooms
            foreach (var room in ToDo)
            {
                AlgorithmsUtils.DebugRectInt(roomA, Color.red);
                RectInt innerRoomA = new RectInt(roomA.x + 1, roomA.y + 1, roomA.width - 2, roomA.height - 2);
                AlgorithmsUtils.DebugRectInt(innerRoomA, Color.red);

                AlgorithmsUtils.DebugRectInt(roomB, Color.red);
                RectInt innerRoomB = new RectInt(roomB.x + 1, roomB.y + 1, roomB.width - 2, roomB.height - 2);
                AlgorithmsUtils.DebugRectInt(innerRoomB, Color.red);

            }

            // Draw the door
            AlgorithmsUtils.DebugRectInt(door, Color.cyan);
        });
    }
    private (RectInt, RectInt) SplitVertically(RectInt pRect)
    {
        RectInt roomA = pRect;
        RectInt roomB = pRect;

        roomA.width = (roomA.width / 2) + UnityEngine.Random.Range(-2, 2);
        roomB.width -= (roomA.width - 1);

        roomB.x += roomA.width - 1;

        return (roomA, roomB);
    }
}
