using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(DungeonRenderer))]
public class Dungeon : UnitySingleton<Dungeon>
{
    public float TileSize = 10f;
    public float CeilingHeight = 10f;

    [SerializeField]
    private int tileCount = 10;

    [SerializeField]
    [Range(2,6)][Tooltip("Number of steps for determining which tiles should be rendered.")]
    private int recursion = 2;

    [SerializeField]
    private TileFootprint[] footprints;

    private Tile[] tiles;

    private List<Tile> localSpace = new();

    private struct PlayerLocation
    {
        public int X, Y;
    }

    private PlayerLocation currentPlayerLocation;

    private Player player;

    private DungeonRenderer dungeonRenderer;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInChildren<Player>();
        dungeonRenderer = GetComponent<DungeonRenderer>();

        GenerateSingleTile();
        //GenerateDungeon_Roaming();
        //GenerateDungeon_TileBased();
    }
    
    private bool[] RotateFootprint(bool[] footprint)
    {
        var size = (int)Mathf.Sqrt(footprint.Length);
        var result = new bool[footprint.Length];

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                //result[i * size + j] = footprint[(size - j - 1) * size + i];
                result[j * size + i] = footprint[(size - i - 1) * size + j];
            }
        }

        return result;
    }

    private void Start()
    {
        UpdatePlayer(transform.position, transform.forward);
    }

    #region Generator

    private void GenerateSingleTile()
    {
        tiles = new Tile[tileCount];        
        
        var footprint = footprints[0];
        AddFootprint(0,0, footprint, Direction.North);
        
        for (int i = 0; i < tileCount; i++)
        {
            if(tiles[i] == null)
                continue;
            
            AssignConnections(tiles[i]);
        }

        for (int i = 0; i < tileCount; i++)
        {
            if(tiles[i] == null)
                continue;
            
            AssignType(tiles[i]);
        }        
    }
    
    private void GenerateDungeon_Roaming()
    {
        tiles = new Tile[tileCount];

        for(int i = 0; i < tileCount; i++)
            tiles[i] = new Tile();

        var x = 0;
        var y = 0;

        tiles[0].X = 0;
        tiles[0].Y = 0;

        for (int i = 1; i < tileCount; i++)
        {
            while(IsCoordinateOccupied(x,y))
            {
                var direction = (Direction)Random.Range(0, 4) + 1;

                if (direction == Direction.North) y++;
                if (direction == Direction.South) y--;
                if (direction == Direction.West) x--;
                if (direction == Direction.East) x++;
            }
            
            tiles[i].X = x;
            tiles[i].Y = y;
        }

        for(int i = 0; i < tileCount; i++) 
        {
            AssignConnections(tiles[i]);
        }

        for(int i = 0; i < tileCount; i++)
        {
            AssignType(tiles[i]);
        }
    }

    private int tileIndex = 0;

    private void GenerateDungeon_TileBased()
    {
        var footprintBudget = tileCount;

        tiles = new Tile[tileCount];

        var x = 0;
        var y = 0;

        var direction = (Direction)Random.Range(0, 4) + 1;

        // Spend footprintBudget on tiles
        //while (footprintBudget > 0)
        for(int i = 0; i < 2; i++)
        {
            var footprintCandidate = GetFootprintCandidate(footprintBudget);

            while(!GetFootprintIsValid(x, y, footprintCandidate, direction))
            {
                direction = (Direction)Random.Range(0, 4) + 1;
            }

            if (footprintCandidate != null) 
            { 
                AddFootprint(x, y, footprintCandidate, direction);
                footprintBudget -= footprintCandidate.TileCount;
            }

            if (footprintBudget < SmallestFootprintSize())
                break;
        }

        // Spend remaining footprintBudget, if any
        if (footprintBudget > 0)
        {
            for(int i = tileIndex; i < tileCount; i++)
            {
                footprintBudget--;
            }
        }

        for (int i = 0; i < tileCount; i++)
        {
            AssignConnections(tiles[i]);
        }

        for (int i = 0; i < tileCount; i++)
        {
            AssignType(tiles[i]);
        }
    }

    private TileFootprint GetFootprintCandidate(int currentBudget)
    {
        TileFootprint candidate = null;

        var candidateList = new List<TileFootprint>();

        foreach(var footprint in footprints)
        {
            if(footprint.TileCount < currentBudget)
                candidateList.Add(footprint);
        }

        if(candidateList.Count > 0)
        {
            var localIndex = Random.Range(0, candidateList.Count);
            candidate = candidateList[localIndex];
        }

        return candidate;
    }

    private int SmallestFootprintSize()
    {
        var size = int.MaxValue;

        foreach(var footprint in footprints)
        {
            if(footprint.TileCount < size)
                size = footprint.TileCount;
        }

        return size;
    }

    private void AddFootprint(int x, int y, TileFootprint footprint, Direction direction)
    {
        var localX = x;
        var localY = y;

        var copy = new bool[footprint.Footprint.Length];
        footprint.Footprint.CopyTo(copy, 0);

        //copy = RotateFootprint(copy);
        //copy = RotateFootprint(copy);
        //copy = RotateFootprint(copy);

        for (int u = 0; u < footprint.Size; u++)
        {
            for (int v = 0; v < footprint.Size; v++)            
            {
                var footprintIndex = u * footprint.Size + v;

                if (!copy[footprintIndex])
                    continue;
                
                var index = tileIndex++;
                
                tiles[index] = new Tile
                {
                    X = localX + u,
                    Y = localY + v
                };
            }
        }

        Debug.Log("Footprint Added: " + footprint.name);
    }

    private bool GetFootprintIsValid(int x, int y, TileFootprint footprint, Direction direction)
    {
        var localX = x;
        var localY = y;

        if (direction == Direction.North)
            localY += footprint.Size;

        if (direction == Direction.South)
            localY -= footprint.Size;

        if (direction == Direction.East)
            localX += footprint.Size;

        if (direction == Direction.West)
            localX -= footprint.Size;

        for (int v = 0; v < footprint.Size; v++)
        {
            for(int u = 0; u < footprint.Size; u++)
            {
                if (IsCoordinateOccupied(localX + u, localY + v))
                    return false;
            }
        }

        return true;
    }

    private void AssignConnections(Tile room)
    {
        if (room == null)
            return;

        room.Connections = new TileConnection[4];
        var x = room.X;
        var y = room.Y;

        // North
        if(IsCoordinateOccupied(x, y + 1))
        {
            var northRoom = GetRoomByCoordinate(x, y + 1);
            var newTileConnection = new TileConnection(Direction.North, northRoom);
            room.Connections[0] = newTileConnection;
        }
        else
        {
            room.Connections[0] = new TileConnection(Direction.North, null);
        }

        // South
        if (IsCoordinateOccupied(x, y - 1))
        {
            var southRoom = GetRoomByCoordinate(x, y - 1);
            var newTileConnection = new TileConnection(Direction.South, southRoom);
            room.Connections[1] = newTileConnection;
        }
        else
        {
            room.Connections[1] = new TileConnection(Direction.South, null);
        }

        // East
        if (IsCoordinateOccupied(x + 1, y))
        {
            var eastRoom = GetRoomByCoordinate(x + 1, y);
            var newTileConnection = new TileConnection(Direction.East, eastRoom);
            room.Connections[2] = newTileConnection;
        }
        else
        {
            room.Connections[2] = new TileConnection(Direction.East, null);
        }

        // West
        if (IsCoordinateOccupied(x - 1, y))
        {
            var westRoom = GetRoomByCoordinate(x - 1, y);
            var newTileConnection = new TileConnection(Direction.West, westRoom);
            room.Connections[3] = newTileConnection;
        }
        else
        {
            room.Connections[3] = new TileConnection(Direction.West, null);
        }
    }

    private void AssignType(Tile room)
    {
        if (room == null)
            return;

        if (room.Type != TileType.None)
            return;

        // No connections means this is an orphaned tile
        if (room.Connections.Length == 0)
            room.Type = TileType.Void;

        // Exactly 1 connection means this is a deadend
        if (room.Connections.Length == 1)
            room.Type = TileType.Deadend;

        // Exactly 2 connections means this is either a corridor or a corner
        if (room.Connections.Length == 2)
        {
            var xMatches = 0;
            var yMatches = 0;

            var x = room.X;
            var y = room.Y;

            foreach(var connection in room.Connections)
            {
                if (connection.Tile.X == x)
                    xMatches++;

                if(connection.Tile.Y == y)
                    yMatches++;
            }

            if (xMatches == 2 || yMatches == 2)
                room.Type = TileType.Corridor;
            else
                room.Type = TileType.Corner;
        }

        // Three connections means this is a wall tile
        if (room.Connections.Length == 3)
            room.Type = TileType.Wall;

        // Four connections means this is an empty tile
        if (room.Connections.Length == 4)
            room.Type = TileType.Floor;
    }
    #endregion

    private bool IsCoordinateOccupied(int x, int y)
    {
        foreach(var room in tiles)
        {
            if (room == null)
                continue;

            if (room.X == x && room.Y == y) 
                return true;
        }

        return false;
    }

    private Tile GetRoomByCoordinate(int x, int y)
    {
        foreach (var room in tiles)
        {
            if (room == null)
                continue;

            if (room.X == x && room.Y == y)
                return room;
        }

        return null;
    }

    private void CheckRecursion(int x, int y, Direction entityFacing = Direction.None)
    {
        localSpace.Clear();

        var sourceRoom = GetRoomByCoordinate(x, y);

        if (sourceRoom == null)
            return;

        var frontier = new List<Tile> { sourceRoom };

        var iterations = recursion;

        while(iterations > 0) 
        {
            localSpace.AddRange(frontier.ToArray());
            var trash = frontier.ToArray();

            foreach(var room in trash)
            {
                if(entityFacing == Direction.None)
                {
                    if (room.Connections.Length > 0)
                    {
                        foreach(var connection in room.Connections)
                            frontier.Add(connection.Tile);
                    }
                }
                else
                {
                    var xCheck = entityFacing is Direction.East or Direction.West ? (entityFacing == Direction.East ? -1 : 1 ) : 0;
                    var yCheck = entityFacing is Direction.North or Direction.South ? (entityFacing == Direction.North ? -1 : 1) : 0;
                    
                    foreach(var connection in room.Connections)
                    {
                        if (connection.Tile == null)
                            continue;
                        
                        if(connection.Tile.Type == TileType.Void)
                            continue;

                        if (xCheck < 0 && connection.Tile.X < x || xCheck > 0 && connection.Tile.X > x)
                            continue;

                        if (yCheck < 0 && connection.Tile.Y < y || yCheck > 0 && connection.Tile.Y > y)
                            continue;

                        frontier.Add(connection.Tile);
                    }
                }
            }

            foreach(var room in trash)
                frontier.Remove(room);

            iterations--;
        }

        localSpace = localSpace.Distinct().ToList();
    }

    public (int, int) ConvertCoordinates(Vector3 position)
    {
        var roundX = Mathf.RoundToInt(position.x / Instance.TileSize);
        var roundY = Mathf.RoundToInt(position.z / Instance.TileSize);
        return (roundX, roundY);
    }

    public bool TileIsValid(Vector3 position)
    {
        var coordinates = ConvertCoordinates(position);
        return GetRoomByCoordinate(coordinates.Item1, coordinates.Item2) != null;
    }

    public Direction FindEntityDirection(Vector3 forwardVector)
    {
        if (Vector3.Dot(Vector3.right, forwardVector.normalized) > .9f)
            return Direction.East;

        if (Vector3.Dot(Vector3.left, forwardVector.normalized) > .9f)
            return Direction.West;

        if (Vector3.Dot(Vector3.forward, forwardVector.normalized) > .9f)
            return Direction.North;

        if (Vector3.Dot(Vector3.back, forwardVector.normalized) > .9f)
            return Direction.South;

        return Direction.None;
    }

    public void UpdatePlayer(Vector3 futurePosition, Vector3 futureForwardDirection)
    {
        var coordinates = ConvertCoordinates(futurePosition);

        var position = transform.position;
        position.x -= TileSize * .5f;
        position.z -= TileSize * .5f;

        var entityDirection = FindEntityDirection(futureForwardDirection);
        CheckRecursion(coordinates.Item1, coordinates.Item2, entityDirection);

        currentPlayerLocation.X = coordinates.Item1;
        currentPlayerLocation.Y = coordinates.Item2;

        dungeonRenderer.UpdateDungeonTiles(localSpace.ToArray());
    }

    private void OnDrawGizmos()
    {
        if (tiles == null)
            return;

        var position = transform.position;
        position.x -= TileSize * .5f;
        position.z -= TileSize * .5f;

        var currentRoom = GetRoomByCoordinate(currentPlayerLocation.X, currentPlayerLocation.Y);

        if (currentRoom != null)
            DebugUtility.DrawRoom(position, currentRoom, Color.red);

        var label = currentRoom != null ? currentRoom.Type.ToString() : "null";

        UnityEditor.Handles.Label(player.transform.position + Vector3.up * 4f, label);

        ///* For drawing the whole maze and not just the local space
        foreach (var room in tiles)
        {
            if(room == null) 
                continue;

            if (room.Type == TileType.Void)
                continue;

            var labelPosition = position + new Vector3(room.X * TileSize + (TileSize * .5f), 0, room.Y * TileSize + (TileSize * .5f));
            UnityEditor.Handles.Label(labelPosition, room.X + "," + room.Y);
            DebugUtility.DrawRoom(position, room, Color.blue);
        }
        //*/

        foreach(var room in localSpace)
        {
            DebugUtility.DrawRoom(position, room, Color.green);
        }
    }
}