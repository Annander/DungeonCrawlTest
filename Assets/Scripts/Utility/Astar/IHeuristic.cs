public interface IHeuristic<N,E,G>
    where N : IGraphNode
    where E : IGraphEdge, new()
    where G : SparseGraph<N,E>
{
    public float Calculate(G graph, int a, int b);
}