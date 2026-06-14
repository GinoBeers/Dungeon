using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenV1 : MonoBehaviour
{
    public int height = 50;
    public int width = 100;

    public List<RectInt> ToDo = new ();
    public List<RectInt> Done = new ();

    [ContextMenu("Generate dungeon")]
    [Button("Generate dungeon", EButtonEnableMode.Playmode)]
    private void Start()
    {

        StartCoroutine(StartDungeonGeneration());
    }

    private IEnumerator StartDungeonGeneration()
    {
        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();

        yield return null;

        ToDo.Add( new RectInt(0, 0, width, height));

        DebugDrawingBatcher.GetInstance().BatchCall(
            () => AlgorithmsUtils.DebugRectInt(ToDo[0], Color.red)
        );

        //yield return keypress
        yield return new WaitUntil (() => Input.GetKeyDown(KeyCode.Space));
        yield return null;

        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();

        RectInt main = ToDo[0];

        int splitX = main.width / 2;
        RectInt half1 = new RectInt(main.x, main.y, splitX + 1, main.height);

        DebugDrawingBatcher.GetInstance().BatchCall(
                () => AlgorithmsUtils.DebugRectInt(half1, Color.yellow)
        );


        RectInt half2 = new RectInt(main.x + splitX - 1, main.y, splitX + 1, main.height);

        DebugDrawingBatcher.GetInstance().BatchCall(
                () => AlgorithmsUtils.DebugRectInt(half2, Color.yellow)
        );


    }

    

    
    void VerticalSplit(RectInt main)
    {
        DebugDrawingBatcher.GetInstance().ClearAllBatchedCalls();

        int splitX = main.width / 2;
        RectInt half1 = new RectInt(main.x, main.y, splitX + 1, main.height);

        DebugDrawingBatcher.GetInstance().BatchCall(
                () => AlgorithmsUtils.DebugRectInt(half1, Color.yellow)
        );


        RectInt half2 = new RectInt(main.x + splitX-1, main.y, splitX + 1, main.height);

        DebugDrawingBatcher.GetInstance().BatchCall(
                () => AlgorithmsUtils.DebugRectInt(half2, Color.yellow)
        );

    }

    void WaitForSpace()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //VerticalSplit();
        }
    }
}
