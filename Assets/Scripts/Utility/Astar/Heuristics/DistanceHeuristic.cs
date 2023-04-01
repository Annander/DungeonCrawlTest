using UnityEngine;

public class DistanceHeuristic : IHeuristic<WorldSpaceNode, WorldSpaceEdge, WorldSpaceGraph>
{
    public float Calculate(WorldSpaceGraph graph, int a, int b)
    {
        var aPosition = graph.GetNode(a).transform.position;
        var bPosition = graph.GetNode(b).transform.position;

        return Vector3.Distance(aPosition, bPosition);
    }
}