using UnityEngine;

public class MonoState : MonoBehaviour, IState
{
    public virtual bool HasEntered => false;

    public virtual void OnDrawGizmos()
    {}

    public virtual void OnEnter()
    {}

    public virtual void OnExit()
    {}

    public virtual void OnUndo()
    {}

    public virtual StateReturn OnUpdate()
    {
        return StateReturn.Completed;
    }
}