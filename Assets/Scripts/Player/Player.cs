using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput))]
public class Player : UnitySingleton<Player>,
    IRewindable
{
    public float TurnTime = 1f;
    public float MoveTime = 1f;

    private PlayerInput input;

    private QueueMachine<Player> queueMachine;

    private readonly Stack<IState> undoStack = new();

    protected override void Awake()
    {
        base.Awake();

        input = GetComponent<PlayerInput>();
        input.actions["Move"].started += OnPlayerMove;
        input.actions["Rewind"].started += OnPlayerRewind;

        queueMachine = new QueueMachine<Player>(this);

        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnDestroy()
    {
        input.actions["Move"].started -= OnPlayerMove;
        input.actions["Rewind"].started -= OnPlayerRewind;
    }

    private void Start()
    {
        // Subscribe to the QueueMachine's OnDequeue event to catch dequeued states and push them to the undo stack
        queueMachine.OnDequeue += (state) =>
        {
            undoStack.Push(state);
        };
    }

    private void Update()
    {
        queueMachine.Update();
    }

    private void OnPlayerRewind(InputAction.CallbackContext context)
    {
        Simulation.Instance.Rewind();
    }

    public void OnRewind()
    {
        // Clear the QueueMachine
        queueMachine.Clear();

        // Get all the previously used IStates from the stack and enqueue them directly into the QueueMachine
        foreach (var state in undoStack)
        {
            queueMachine.EnqueueState(state, true);
        }

        // Empty the undo stack
        undoStack.Clear();
    }

    private void OnPlayerMove(InputAction.CallbackContext context)
    {
        var readValue = context.ReadValue<Vector2>();

        PerformTurn(Mathf.RoundToInt(readValue.x));
        PerformMove(Mathf.RoundToInt(readValue.y));
    }

    private void PerformMove(int axis)
    {
        if (axis == 0) return;
        if (queueMachine.IsState<PlayerMove>()) return;
        
        var t = transform;
        var forward = t.forward;
        var move = axis * Dungeon.Instance.TileSize;
        var goal = t.position + (forward * move);
        var newPlayerMove = new PlayerMove(t, move);
        queueMachine.EnqueueState(newPlayerMove);
    }

    private void PerformTurn(int axis)
    {
        if (axis == 0) return;
        if (queueMachine.IsState<PlayerTurn>()) return;
        
        var t = transform;
        var angle = axis * 90f;
        var newPlayerTurn = new PlayerTurn(t, angle);
        queueMachine.EnqueueState(newPlayerTurn);
    }

    private void OnDrawGizmosSelected()
    {
        if (Dungeon.Instance == null)
            return;

        UnityEditor.Handles.Label(transform.position + (Vector3.up * 2f), Dungeon.Instance.FindEntityDirection(transform.forward).ToString());

        if (queueMachine != null)
            queueMachine.OnDrawGizmosSelected();
    }
}