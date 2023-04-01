using UnityEngine;

[RequireComponent(typeof(DungeonMesh))]
public class DungeonRenderer : MonoBehaviour
{
    private DungeonMesh dungeonMesh;

    private void Awake()
    {
        dungeonMesh = GetComponent<DungeonMesh>();
    }

    public void UpdateDungeonTiles(Tile[] tiles)
    {
        dungeonMesh.UpdateMesh(tiles);
    }
}