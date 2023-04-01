using System;

public interface IGraphNode : IComparable<IGraphNode>
{
    public int Index
    {
        get; set;
    }
}