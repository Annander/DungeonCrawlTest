public struct TileConnection
{
    public Direction Direction;
    public Tile Tile;

    public TileConnection(Direction direction, Tile tile)
    {
        Direction = direction;
        Tile = tile;
    }
}