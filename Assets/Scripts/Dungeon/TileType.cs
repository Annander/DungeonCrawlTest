public enum TileType
{
    /// <summary>
    /// The default, which is used to determine if anything has changed the type along the way.
    /// </summary>
    None,
    
    /// <summary>
    /// A void tile is empty space and is used to make sure tile footprints don't intersect. It's the
    /// only form of a tile that is not "valid," by definition.
    /// </summary>
    Void,

    /// <summary>
    /// A wall tile has a single valid connection.
    /// </summary>
    Wall,

    /// <summary>
    /// A corner tile has two adjacent cardinal direction connections.
    /// </summary>
    Corner,

    /// <summary>
    /// A floor tile has all four connections into other valid tiles.
    /// </summary>
    Floor,

    /// <summary>
    /// A corridor tile has two connections on opposite sides.
    /// </summary>
    Corridor,

    /// <summary>
    /// A deadend has only a single valid connection.
    /// </summary>
    Deadend,

    /// <summary>
    /// A start tile has been flagged as a potential entrance.
    /// </summary>
    Start,

    /// <summary>
    /// An end tile has been flagged as a potential exit.
    /// </summary>
    End
}