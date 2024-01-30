using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A stack-based state machine in very simple form.
/// </summary>
public class StackMachine : MonoBehaviour
{
	private const string NoState = "None";

	private readonly Stack<IState> stack = new();

	protected virtual void Update()
	{
		if (stack.Count <= 0) return;
		
		var state = stack.Peek().OnUpdate();

		if (state == StateReturn.Completed)
			PopState();
	}

	public void Clear() => stack.Clear();

	public void PushState(IState state, bool onEnter = true)
	{
		if (stack.Count > 0)
		{
			if (stack.Peek() == state)
				return;
			
			stack.Peek().OnExit();
		}

		if(onEnter) 
			state.OnEnter();

		stack.Push(state);
	}

	public void PopState()
	{
        if (stack.Count == 0)
            return;

        stack.Peek().OnExit();
		stack.Pop();

        if(stack.Count > 0)
		{
			var state = stack.Peek();
			state.OnEnter();
        }
	}

	public bool IsState(IState state)
	{
		if (stack.Count < 1)
			return false;

		return state == stack.Peek();
	}

#if UNITY_EDITOR
	public override string ToString()
	{
		return CurrentState;
	}

	public string CurrentState 
	{
        get {
            if (stack.Count > 0)
                return stack.Peek().ToString();

            return NoState;
        }
    }

    private void OnDrawGizmosSelected()
    {
		UnityEditor.Handles.Label(transform.position, CurrentState);
    }
#endif
}