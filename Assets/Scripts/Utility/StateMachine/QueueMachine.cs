using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A queue-based state machine in very simple form.
/// </summary>
public class QueueMachine<T>
	where T : MonoBehaviour
{
	public event QueueDelegate OnDequeue;
	public delegate void QueueDelegate(IState state);

	private const string NoState = "None";

	private readonly Queue<IState> queue = new();

	private readonly T owner;

	public QueueMachine(T owner)
	{
		this.owner = owner;
	}

	public void Update()
	{
		if (queue.Count <= 0) return;
		
		var state = queue.Peek();

		if (state.OnUpdate() == StateReturn.Completed)
			DequeueState();
	}

	public void Clear() 
	{
		queue.Clear();
	}

	public void EnqueueState(IState state, bool undo = false)
	{
		if ( queue.Count > 0 )
		{
			if (queue.Peek() == state)
				return;
		}

		if(undo)
			state.OnUndo();
		else
			state.OnEnter();

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

	public bool IsState<TState>()
	{
		if (queue.Count <= 0) return false;
		return queue.Peek().GetType() == typeof(TState);
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

            return NoState;
        }
    }

    public void OnDrawGizmosSelected()
    {
		UnityEditor.Handles.Label(owner.transform.position, CurrentState);
    }
#endif
}