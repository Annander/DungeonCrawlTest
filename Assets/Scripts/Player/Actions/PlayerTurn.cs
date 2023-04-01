using UnityEngine;

public class PlayerTurn : BaseState
{
    private Transform transform;

    private float time;
    private float turn;

    private Quaternion origin;

    private bool hasEntered;

    public PlayerTurn(Transform transform, float turn)
    {
        this.transform = transform;
        this.turn = turn;
    }

    public override void OnEnter()
    {
        origin = transform.localRotation;
        time = Player.Instance.TurnTime;
        hasEntered = true;
    }

    public override void OnUndo()
    {
        time = Player.Instance.TurnTime;
        turn = -turn;
    }

    public override StateReturn OnUpdate()
    {
        if(time > 0) 
        {
            time -= Time.deltaTime;

            var t = 1f - (time / Player.Instance.TurnTime);
            t = Easing.Cubic_In(t);

            var angle = Mathf.Lerp(0f, turn, t);

            transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up) * origin;

            return StateReturn.Running;
        }

        Dungeon.Instance.UpdatePlayer();

        return StateReturn.Completed;
    }

    public override void OnExit()
    {
        origin = transform.localRotation;
    }

    public override bool HasEntered => hasEntered;
}