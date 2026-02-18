using UnityEngine;

public class DebugDrawing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnDrawGizmos()
    {
        RectInt main = new RectInt(-5,-5,10,10);

        AlgorithmsUtils.DebugRectInt(main, Color.red);
    }
}
