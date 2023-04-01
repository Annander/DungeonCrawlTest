public class Simulation : UnitySingleton<Simulation>
{
    // All entities that can be rewound
    private IRewindable[] rewindables;

    protected override void Awake()
    {
        base.Awake();
        rewindables = GetComponentsInChildren<IRewindable>();
    }

    public void Tick()
    {
        // Should tick everything in the game world based on player interaction.
    }

    public void Rewind()
    {
        foreach(var rewindable in rewindables) 
        { 
            rewindable.OnRewind();
        }
    }
}