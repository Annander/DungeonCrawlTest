using System.Collections.Generic;

public class WorldSpaceAStar : AStar<WorldSpaceNode, WorldSpaceEdge, WorldSpaceGraph, DistanceHeuristic>
{
    public WorldSpaceAStar(WorldSpaceGraph graph, DistanceHeuristic heuristic) :
        base(graph, heuristic)
    {}
}

public class AStar<N,E,G,H>
    where N : IGraphNode
    where E : IGraphEdge, new()
    where G : SparseGraph<N,E>
    where H : IHeuristic<N,E,G>
{
    private G graph;
    private H heuristic;

    private float[] gCosts;
    private float[] fCosts;

    private E[] shortestPathTree;
    private E[] searchFrontier;

    private int source;
    private int target;

    public List<int> Path
    {
        get 
        {
            var path = new List<int>();

            if (target < 0)
                return path;

            var node = target;

            path.Insert(0, node);

            while((node != source) && (shortestPathTree[node] != null))
            {
                node = shortestPathTree[node].From;
                path.Insert(0, node);
            }

            return path;
        }
    }

    public AStar(G graph, H heuristic)
    {
        this.graph = graph;
        this.heuristic = heuristic;

        var count = graph.NodeCount;

        shortestPathTree = new E[count];
        searchFrontier = new E[count];

        gCosts = new float[count];
        fCosts = new float[count];
    }

    public void Search(int _source, int _target)
    {
        Clear();

        source = _source;
        target = _target;

        var priorityQueue = new IndexedPriorityQueue<float>(fCosts, graph.NodeCount);

        priorityQueue.Insert(source);

        while(!priorityQueue.Empty)
        {
            var nextClosestNode = priorityQueue.Pop();

            shortestPathTree[nextClosestNode] = searchFrontier[nextClosestNode];

            if (nextClosestNode == target)
                return;

            foreach(E edge in graph.Edges[nextClosestNode])
            {
                float hCost = heuristic.Calculate(graph, target, edge.To);
                float gCost = gCosts[nextClosestNode] + edge.Cost;

                if(searchFrontier[edge.To] == null)
                {
                    fCosts[edge.To] = gCost + hCost;
                    gCosts[edge.To] = gCost;

                    priorityQueue.Insert(edge.To);

                    searchFrontier[edge.To] = edge;
                }
                else if((gCost < gCosts[edge.To]) && (shortestPathTree[edge.To] == null))
                {
                    fCosts[edge.To] = gCost + hCost;
                    gCosts[edge.To] = gCost;

                    priorityQueue.ChangePriority(edge.To);

                    searchFrontier[edge.To] = edge;
                }
            }
        }
    }

    private void Clear()
    {
        for (int i = 0; i < graph.NodeCount; ++i)
        {
            shortestPathTree[i] = default(E);
            searchFrontier[i] = default(E);
            gCosts[i] = 0f;
            fCosts[i] = 0f;
        }
    }
}