using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class DungeonMesh : MonoBehaviour
{
    private MeshFilter meshFilter;

    private Mesh mesh;

    private readonly List<Vector3> vertices = new();
    private readonly List<Vector3> normals = new();
    private readonly List<Vector2> uv = new();
    private readonly List<int> triangles = new();

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
    }

    private void Update()
    {
        foreach(var vertex in vertices) 
        {
            DebugUtility.DrawDebugCross(vertex, Color.red);
        }

        for(int i = 0; i < vertices.Count; i++)
        {
            Debug.DrawRay(vertices[i], normals[i], Color.cyan);
        }
    }

    public void UpdateMesh(Tile[] currentTiles)
    {
        GenerateMesh(currentTiles);
    }

    private void GenerateMesh(Tile[] currentTiles)
    {
        mesh.Clear();

        vertices.Clear();
        normals.Clear();
        triangles.Clear();
        uv.Clear();

        for (int i = 0; i < currentTiles.Length; i++) 
        {
            AddTile(currentTiles[i]);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.normals = normals.ToArray();
    }

    private void AddTile(Tile tile)
    {
        var tileSize = Dungeon.Instance.TileSize;

        var position = transform.position;
        position.x -= tileSize * .5f;
        position.z -= tileSize * .5f;

        var rX = tile.x * tileSize;
        var rY = tile.y * tileSize;

        // Floor
        var bottomLeftCorner = position + new Vector3(rX, 0, rY);
        var topLeftCorner = position + new Vector3(rX, 0, rY + tileSize);
        var topRightCorner = position + new Vector3(rX + tileSize, 0, rY + tileSize);
        var bottomRightCorner = position + new Vector3(rX + tileSize, 0, rY);

        AddQuad(
            bottomLeftCorner,
            bottomRightCorner,
            topLeftCorner, 
            topRightCorner,
            Vector3.up
            );

        // Ceiling
        var ceiling = Vector3.up * Dungeon.Instance.CeilingHeight;

        AddQuad(
            bottomLeftCorner + ceiling,
            bottomRightCorner + ceiling,
            topLeftCorner + ceiling,
            topRightCorner + ceiling,
            Vector3.down
            );

        var heightBySize = Dungeon.Instance.CeilingHeight / Dungeon.Instance.TileSize;

        // Walls
        foreach (var connection in tile.Connections)
        {
            if (connection.Tile != null)
                continue;

            if(connection.Direction == Direction.North)
            {
                AddQuad(
                    topLeftCorner,
                    topRightCorner,
                    topLeftCorner + ceiling,
                    topRightCorner + ceiling,
                    Vector3.back,
                    1f,
                    heightBySize
                    );
            }

            if (connection.Direction == Direction.East)
            {
                AddQuad(
                    topRightCorner,
                    bottomRightCorner,
                    topRightCorner + ceiling,
                    bottomRightCorner + ceiling,
                    Vector3.left,
                    1f,
                    heightBySize
                    );
            }

            if (connection.Direction == Direction.South) 
            {
                AddQuad(
                    bottomRightCorner,
                    bottomLeftCorner,
                    bottomRightCorner + ceiling,
                    bottomLeftCorner + ceiling,
                    Vector3.forward,
                    1f,
                    heightBySize
                    );
            }

            if (connection.Direction == Direction.West)
            {
                AddQuad(
                    bottomLeftCorner,
                    topLeftCorner,
                    bottomLeftCorner + ceiling,
                    topLeftCorner + ceiling,
                    Vector3.right,
                    1f,
                    heightBySize
                    );
            }
        }
    }

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        var vertexIndex = vertices.Count;

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 normal, float uvXScale = 1f, float uvYScale = 1f)
    {
        int vertexIndex = vertices.Count;

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(uvXScale, 0));
        uv.Add(new Vector2(0, uvYScale));
        uv.Add(new Vector2(uvXScale, uvYScale));

        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        // Flip normals for the ceiling
        if(normal == Vector3.down)
        {
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex + 2);
        }
        else
        {
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex + 1);
        }
    }
}