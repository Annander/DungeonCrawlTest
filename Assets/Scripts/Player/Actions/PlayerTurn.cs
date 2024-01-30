using UnityEngine;

public class PlayerTurn : BaseState
{
    private readonly Transform transform;

    private float time;
    private float turn;

    private Quaternion origin;

    public PlayerTurn(Transform transform, float turn)
    {
        this.transform = transform;
        this.turn = turn;
    }

    public override void OnEnter()
    {
        origin = transform.localRotation;
        time = Player.Instance.TurnTime;
        
        var euler = Quaternion.AngleAxis(turn, Vector3.up) * transform.forward;
        Dungeon.Instance.UpdatePlayer(transform.position, euler);
    }

    public override void OnUndo()
    {
        time = Player.Instance.TurnTime;
        turn = -turn;
        
        var euler = Quaternion.AngleAxis(turn, Vector3.up) * transform.forward;
        Dungeon.Instance.UpdatePlayer(transform.position, euler);
    }

    public override StateReturn OnUpdate()
    {
        if (!(time > 0)) return StateReturn.Completed;
        
        time -= Time.deltaTime;

        var t = 1f - (time / Player.Instance.TurnTime);
        t = Easing.Expo_Out(t);

        var angle = Mathf.Lerp(0f, turn, t);

        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up) * origin;

        return StateReturn.Running;

    }

    public override void OnExit()
    {
        origin = transform.localRotation;
    }
}