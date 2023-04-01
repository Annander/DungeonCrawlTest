using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A stack-based state machine in very simple form.
/// </summary>
public class StackMachine : MonoBehaviour
{
	public const string NOSTATE = "None";

	private Stack<IState> stack = new Stack<IState>();

	protected virtual void Update()
    {
		if (stack.Count > 0)
        {
			var state = stack.Peek().OnUpdate();

			if (state == StateReturn.Completed)
				PopState();
		}
	}

	public void Clear() 
	{
		stack.Clear();
	}

	public void PushState(IState state, bool onEnter = true)
	{
		if ( stack.Count > 0 )
		{
			if ( stack.Peek() == state )
				return;
			else
				stack.Peek().OnExit();
		}

		if(onEnter && !state.HasEntered)
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
            
			if(!state.HasEntered)
				state.OnEnter();
        }
	}

	public bool IsState(IState state)
	{
		if (stack.Count < 1)
			return false;

		if( state == stack.Peek() )
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
            if (stack.Count > 0)
                return stack.Peek().ToString();

            return NOSTATE;
        }
    }

    private void OnDrawGizmosSelected()
    {
		UnityEditor.Handles.Label(transform.position, CurrentState);
    }
#endif
}