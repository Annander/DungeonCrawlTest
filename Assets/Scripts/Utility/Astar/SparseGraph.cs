using UnityEngine;
using System.Collections.Generic;

public class WorldSpaceGraph : SparseGraph<WorldSpaceNode, WorldSpaceEdge>
{
    public WorldSpaceGraph(bool digraph = false) : base(digraph)
    {}

    public void AddNode(int index, WorldSpaceNode node, Vector3 position)
    {
        node.Index = index;
        node.transform.position = position;
        AddNode(node);
    }
}

public class SparseGraph<N, E>
    where N : IGraphNode
    where E : IGraphEdge, new()
{
    private const int invalidNodeIndex = -1;
    private bool digraph = false;

    public int NextNodeIndex
    {
        get;
        private set;
    }

    public SparseGraph(bool digraph = false)
    {
        this.digraph = digraph;
        NextNodeIndex = 0;

        Edges = new List<List<E>>();
        Nodes = new List<N>();
    }

    #region Node Operations

    public List<N> Nodes
    {
        get;
        private set;
    }

    public void Clear()
    {
        NextNodeIndex = 0;
        Nodes.Clear();
        Edges.Clear();
    }

    public int AddNode(N node)
    {
        if(node.Index < Nodes.Count)
        {
            Nodes[node.Index] = node;
            return NextNodeIndex;
        }
        else
        {
            Nodes.Add(node);
            Edges.Add(new List<E>());
            
            return ++NextNodeIndex;
        }
    }

    public void RemoveNode(int index)
    {
        Nodes[index].Index = invalidNodeIndex;

        if(!digraph)
        {
            foreach(var edge in Edges[index])
            {
                foreach(var currentEdge in Edges[edge.To])
                {
                    Edges[currentEdge.To].Remove(currentEdge);
                    break;
                }
            }

            Edges[index].Clear();
        }
        else
        {
            CullInvalidEdges();
        }
    }

    public N GetNode(int index)
    {
        return Nodes[index];
    }

    public int NodeCount
    {
        get { return Nodes.Count; }
    }

    public int ActiveNodeCount
    {
        get
        {
            var count = 0;

            for (int i = 0; i < Nodes.Count; ++i)
            {
                if (Nodes[i].Index != invalidNodeIndex)
                    ++count;
            }

            return count;
        }

    }

    public bool isNodePresent(int index)
    {
        return (index >= Nodes.Count) || Nodes[index].Index == invalidNodeIndex;
    }

    #endregion

    #region Edge Operations

    public List<List<E>> Edges
    {
        get;
        private set;
    }

    public void SetEdgeCost(int from, int to, float newCost)
    {
        foreach(var edge in Edges[from])
        {
            if(edge.To == to)
            {
                edge.Cost = newCost;
                break;
            }
        }
    }

    public void AddEdge(E edge)
    {
        if(Nodes[edge.To].Index != invalidNodeIndex && Nodes[edge.From].Index != invalidNodeIndex)
        {
            if(UniqueEdge(edge.From, edge.To))
            {
                Edges[edge.From].Add(edge);
            }

            if (digraph)
            {
                if(UniqueEdge(edge.From, edge.To))
                {
                    var newEdge = edge;
                    newEdge.To = edge.From;
                    newEdge.From = edge.To;

                    Edges[edge.To].Add(newEdge);
                }
            }
        }
    }

    public void RemoveEdge(int from, int to)
    {
        if(!digraph)
        {
            foreach(var edge in Edges[to])
            {
                if(edge.To == from)
                {
                    Edges[to].Remove(edge);
                    break;
                }
            }
        }

        foreach(var edge in Edges[from])
        {
            if(edge.To == to)
            {
                Edges[from].Remove(edge);
                break;
            }
        }
    }

    public E GetEdge(int from, int to)
    {
        foreach (var edge in Edges[from])
        {
            if (edge.To == to)
                return edge;
        }

        return default(E);
    }

    public int EdgeCount
    {
        get 
        {
            var count = 0;

            foreach(var edgeList in Edges)
                count += edgeList.Count;

            return count;
        }
    }

    public bool isEdgePresent(int from, int to)
    {
        foreach(var edge in Edges[from])
        {
            if (edge.To == to)
                return true;
        }

        return false;
    }

    private bool UniqueEdge(int from, int to)
    {
        foreach (var edge in Edges[from])
        {
            if (edge.To == to)
                return false;
        }

        return true;
    }

    private void CullInvalidEdges()
    {
        foreach(var currentEdgeList in Edges)
        {
            foreach(var currentEdge in currentEdgeList)
            {
                if (Nodes[currentEdge.To].Index == invalidNodeIndex || Nodes[currentEdge.From].Index == invalidNodeIndex)
                    currentEdgeList.Remove(currentEdge);
            }
        }
    }

    #endregion
}