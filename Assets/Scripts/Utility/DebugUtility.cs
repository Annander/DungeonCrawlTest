using UnityEngine;

public class DebugUtility
{
    public static void DrawDebugCross(Vector3 worldPosition, Color color, float size = .15f, float duration = 0)
    {
        Debug.DrawRay(worldPosition, Vector3.up * size, color, duration);
        Debug.DrawRay(worldPosition, Vector3.down * size, color, duration);
        Debug.DrawRay(worldPosition, Vector3.right * size, color, duration);
        Debug.DrawRay(worldPosition, Vector3.left * size, color, duration);
        Debug.DrawRay(worldPosition, Vector3.forward * size, color, duration);
        Debug.DrawRay(worldPosition, Vector3.back * size, color, duration);
    }

    public static void DrawSquare(float top, float left, float bottom, float right, float y, Color color)
    {
        Color blue = color;
        blue.a = .1f;

        Color white = new Color(1, 1, 1, .2f);

        Vector3[] verts = new Vector3[] {
            new Vector3( left, y, bottom ),
            new Vector3( left, y, top ),
            new Vector3( right, y, top ),
            new Vector3( right, y, bottom ),
        };

        UnityEditor.Handles.DrawSolidRectangleWithOutline(verts, blue, white);
    }

    public static void DrawRoom(Vector3 position, Tile room, Color color, float y = 0)
    {
        color.a = .1f;

        var rX = room.x * Dungeon.Instance.TileSize;
        var rY = room.y * Dungeon.Instance.TileSize;

        DebugUtility.DrawSquare(
            (position.z + (rY + Dungeon.Instance.TileSize)),
            (position.x + (rX + Dungeon.Instance.TileSize)),
            (position.z + rY),
            (position.x + rX),
            y,
            color
            );
    }

    public static void DrawRotatedGizmoCube(Transform transform)
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix *= cubeTransform;

        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = oldGizmosMatrix;
    }

    public static void DrawRotatedWireCube(Transform transform)
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix *= cubeTransform;

        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = oldGizmosMatrix;
    }
}