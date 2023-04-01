public class LocationEdge : IGraphEdge
{
    public LocationEdge()
    {
        From = 0;
        To = 0;
        Cost = 0f;
    }

    public LocationEdge(int from, int to, float cost)
    {
        From = from;
        To = to;
        Cost = cost;
    }

    public int From
    {
        get; set;
    }

    public int To
    {
        get; set;
    }

    public float Cost
    {
        get; set;
    }
}