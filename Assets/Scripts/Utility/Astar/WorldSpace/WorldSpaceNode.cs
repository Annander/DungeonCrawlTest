using UnityEngine;

public class WorldSpaceNode : MonoBehaviour, IGraphNode
{
    public int Index
    {
        get;
        set;
    }

    public int CompareTo(IGraphNode other)
    {
        if (other == null) return 1;
        return Index.CompareTo(other.Index);
    }

    public static bool operator < (WorldSpaceNode a, WorldSpaceNode b)
    {
        return a.CompareTo(b) < 0;
    }

    public static bool operator > (WorldSpaceNode a, WorldSpaceNode b)
    {
        return a.CompareTo(b) > 0;
    }

    public static bool operator >= (WorldSpaceNode a, WorldSpaceNode b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <= (WorldSpaceNode a, WorldSpaceNode b)
    {
        return a.CompareTo(b) <= 0;
    }
}