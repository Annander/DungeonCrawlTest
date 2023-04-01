using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A queue-based state machine in very simple form.
/// </summary>
public class QueueMachine<T>
	where T : MonoBehaviour
{
	public event DequeueDelegate OnDequeue;
	public delegate void DequeueDelegate(IState dequeuedState);

	public const string NOSTATE = "None";

	private Queue<IState> queue = new Queue<IState>();

	private T owner;

	public QueueMachine(T owner)
	{
		this.owner = owner;
	}

	public void Update()
    {
		if (queue.Count > 0)
        {
			var state = queue.Peek();

			if(!state.HasEntered)
                state.OnEnter();

            if (state.OnUpdate() == StateReturn.Completed)
				DequeueState();
		}
	}

	public void Clear() 
	{
		queue.Clear();
	}

	public void EnqueueState(IState state)
	{
		if ( queue.Count > 0 )
		{
			if (queue.Peek() == state)
				return;
		}

		if(state.HasEntered)
			state.OnUndo();

        queue.Enqueue(state);
	}

	public void DequeueState()
	{
        if (queue.Count == 0)
            return;

		var state = queue.Dequeue();
		state.OnExit();

        OnDequeue?.Invoke(state);
    }

	public bool IsState(IState state)
	{
		if (queue.Count < 1)
			return false;

		if( state == queue.Peek() )
			return true;

		return false;
	}

#if UNITY_EDITOR
	public override string ToString()
	{
		return CurrentState;
	}

	public string CurrentState 
	{
        get {
            if (queue.Count > 0)
                return queue.Peek().ToString();

            return NOSTATE;
        }
    }

    public void OnDrawGizmosSelected()
    {
		UnityEditor.Handles.Label(owner.transform.position, CurrentState);
    }
#endif
}