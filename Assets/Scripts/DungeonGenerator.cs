using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int height = 50;
    public int width = 100;

    private void OnDrawGizmos()
    {
        RectInt main = new RectInt(0,0,width,height);

        AlgorithmsUtils.DebugRectInt(main, Color.red);
    }
}
