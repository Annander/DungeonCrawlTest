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
    [Range(2,6)]
    private int recursion = 2;

    [SerializeField]
    private TileFootprint[] footprints;

    private Tile[] tiles;

    private List<Tile> localSpace = new List<Tile>();

    private struct PlayerLocation
    {
        public int x, y;
    }

    private PlayerLocation currentPlayerLocation;

    private Player player;

    private DungeonRenderer dungeonRenderer;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInChildren<Player>();
        dungeonRenderer = GetComponent<DungeonRenderer>();

        //GenerateDungeon_Roaming();
        GenerateDungeon_TileBased();
    }

    private void Start()
    {
        UpdatePlayer();
    }

    #region Generator
    private void GenerateDungeon_Roaming()
    {
        tiles = new Tile[tileCount];

        for(int i = 0; i < tileCount; i++)
            tiles[i] = new Tile();

        var x = 0;
        var y = 0;

        tiles[0].x = 0;
        tiles[0].y = 0;

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
            
            tiles[i].x = x;
            tiles[i].y = y;
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

        var selectedDirection = (Direction)Random.Range(0, 4) + 1;

        // Spend footprintBudget on tiles
        //while (footprintBudget > 0)
        for(int i = 0; i < 4; i++)
        {
            TileFootprint newFootprint = null;

            GetFootprintCandidate(x, y, footprintBudget, out newFootprint, selectedDirection);

            while (!GetFootprintIsValid(x, y, newFootprint, selectedDirection))
            {
                GetFootprintCandidate(x, y, footprintBudget, out newFootprint, selectedDirection);
            }

            if (newFootprint != null)
            {
                AddFootprint(x, y, newFootprint, selectedDirection);
                footprintBudget -= newFootprint.TileCount;

                var localX = x;
                var localY = y;

                if(selectedDirection == Direction.North)
                    y = localY + newFootprint.Height;

                if (selectedDirection == Direction.South)
                    y = localY - newFootprint.Height;

                if (selectedDirection == Direction.West)
                    x = localX + newFootprint.Width;

                if (selectedDirection == Direction.East)
                    x = localX - newFootprint.Width;

                selectedDirection = (Direction)Random.Range(0, 4) + 1;
            }

            if (footprintBudget < SmallestFootprintSize())
                break;
        }

        // Spend remaining footprintBudget, if any
        if (footprintBudget > 0)
        {
            Debug.Log("Remaining Budget: " + footprintBudget.ToString());

            for(int i = tileIndex; i < tileCount; i++)
            {
                //tiles[i].Type = TileType.Void;
                footprintBudget--;
            }
        }

        Debug.Log("Remaining Budget: " + footprintBudget.ToString());

        for (int i = 0; i < tileCount; i++)
        {
            AssignConnections(tiles[i]);
        }

        for (int i = 0; i < tileCount; i++)
        {
            AssignType(tiles[i]);
        }
        
    }

    private void GetFootprintCandidate(int x, int y, int currentBudget, out TileFootprint candidate, Direction direction)
    {
        candidate = null;

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
        var index = tileIndex;

        tiles[index] = new Tile();

        tiles[index].x = x;
        tiles[index].y = y;

        for (int i = 0; i < footprint.Footprint.Length; i++)
        {
            index = tileIndex++;

            tiles[index] = new Tile();

            var tileXY = footprint.IndexToXY(i);

            if(direction != Direction.West)
                tiles[index].x = x + tileXY[0];
            else
                tiles[index].x = x - tileXY[0];

            if(direction == Direction.South)
                tiles[index].y = y + tileXY[1];
            else
                tiles[index].y = y - tileXY[1];
        }

        Debug.Log("Footprint Added: " + footprint.name);
    }

    private bool GetFootprintIsValid(int x, int y, TileFootprint footprint, Direction direction)
    {
        if (footprint == null)
            return false;

        if (IsCoordinateOccupied(x, y))
            return false;

        for(int i = 0; i < footprint.Footprint.Length; i++)
        {
            var localX = x;
            var xy = footprint.IndexToXY(i);

            if (direction != Direction.West)
                localX += xy[0];
            else
                localX -= xy[0];

            var localY = y;

            if (direction != Direction.South)
                localY += xy[1];
            else
                localY -= xy[1];

            if (IsCoordinateOccupied(localX, localY))
                return false;
        }

        return true;
    }

    private void AssignConnections(Tile room)
    {
        if (room == null)
            return;

        room.Connections = new TileConnection[4];
        var x = room.x;
        var y = room.y;

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

            var x = room.x;
            var y = room.y;

            foreach(var connection in room.Connections)
            {
                if (connection.Tile.x == x)
                    xMatches++;

                if(connection.Tile.y == y)
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

            if (room.x == x && room.y == y) 
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

            if (room.x == x && room.y == y)
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

        var frontier = new List<Tile>();
        frontier.Add(sourceRoom);

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
                    var xCheck = (entityFacing == Direction.East) || (entityFacing == Direction.West) ? (entityFacing == Direction.East ? -1 : 1 ) : 0;
                    var yCheck = (entityFacing == Direction.North) || (entityFacing == Direction.South) ? (entityFacing == Direction.North ? -1 : 1) : 0;
                    
                    foreach(var connection in room.Connections)
                    {
                        if (connection.Tile == null)
                            continue;

                        if (xCheck < 0 && connection.Tile.x < x || xCheck > 0 && connection.Tile.x > x)
                            continue;

                        if (yCheck < 0 && connection.Tile.y < y || yCheck > 0 && connection.Tile.y > y)
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

    public Tile TryMove(int x, int y)
    {
        return GetRoomByCoordinate(x, y);
    }

    public Direction FindEntityDirection(Vector3 forwardVector)
    {
        if (Vector3.Dot(Vector3.right, forwardVector) > .9f)
            return Direction.East;

        if (Vector3.Dot(Vector3.left, forwardVector) > .9f)
            return Direction.West;

        if (Vector3.Dot(Vector3.forward, forwardVector) > .9f)
            return Direction.North;

        if (Vector3.Dot(Vector3.back, forwardVector) > .9f)
            return Direction.South;

        return Direction.None;
    }

    public void UpdatePlayer()
    {
        var roundX = Mathf.RoundToInt((player.transform.position.x / TileSize));
        var roundY = Mathf.RoundToInt((player.transform.position.z / TileSize));

        var x = roundX * TileSize;
        var y = roundY * TileSize;

        var position = transform.position;
        position.x -= TileSize * .5f;
        position.z -= TileSize * .5f;

        CheckRecursion(roundX, roundY, FindEntityDirection(player.transform.forward));

        currentPlayerLocation.x = roundX;
        currentPlayerLocation.y = roundY;

        dungeonRenderer.UpdateDungeonTiles(localSpace.ToArray());
    }

    private void OnDrawGizmos()
    {
        if (tiles == null)
            return;

        var position = transform.position;
        position.x -= TileSize * .5f;
        position.z -= TileSize * .5f;

        var currentRoom = GetRoomByCoordinate(currentPlayerLocation.x, currentPlayerLocation.y);

        if (currentRoom != null)
            DebugUtility.DrawRoom(position, currentRoom, Color.red);

        var label = currentRoom != null ? currentRoom.Type.ToString() : "null";

        UnityEditor.Handles.Label(player.transform.position + Vector3.up * 4f, label);

        foreach (var room in tiles)
        {
            if(room == null) 
                continue;

            if (room.Type == TileType.Void)
                continue;

            var labelPosition = position + new Vector3(room.x * TileSize + (TileSize * .5f), 0, room.y * TileSize + (TileSize * .5f));
            UnityEditor.Handles.Label(labelPosition, room.x.ToString() + "," + room.y.ToString());
            DebugUtility.DrawRoom(position, room, Color.blue);
        }

        foreach(var room in localSpace)
        {
            DebugUtility.DrawRoom(position, room, Color.green);
        }
    }
}