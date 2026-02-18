using UnityEngine;

public class DebugDrawing : MonoBehaviour
{
    public int height = 10;
    public int width = 20;
    public int verticalSplit = 0;

    private void OnDrawGizmos()
    {
        RectInt main = new RectInt(-width/2,-height/2,width,height);

        AlgorithmsUtils.DebugRectInt(main, Color.red);

        Debug.DrawLine(new Vector3(verticalSplit, 0, -height / 2), new Vector3(verticalSplit, 0, height / 2), Color.yellow);
    }

    //int VerticalSplit()
    //{ 
       // if
    //}
}
