public interface IState
{
    public StateReturn OnUpdate();

    public void OnEnter();

    public void OnExit();

    public void OnUndo();

    public void OnDrawGizmos();
};