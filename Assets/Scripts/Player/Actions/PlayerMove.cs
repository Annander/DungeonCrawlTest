using UnityEngine;

public class PlayerMove : BaseState
{
    private Transform transform;
    private float move;

    private float time;
    private float bounceTime;

    private Vector3 origin;
    private Vector3 goal;

    private bool hasEntered;
    private bool didBounce;

    public PlayerMove(Transform transform, float move)
    {
        this.transform = transform;
        this.move = move;
    }

    public override void OnEnter()
    {
        var futurePosition = transform.position + (transform.forward * move);

        var roundX = Mathf.RoundToInt((futurePosition.x / Dungeon.Instance.TileSize));
        var roundY = Mathf.RoundToInt((futurePosition.z / Dungeon.Instance.TileSize));

        var tryMove = Dungeon.Instance.TryMove(roundX, roundY);

        origin = transform.position;
        goal = futurePosition;

        if (tryMove!= null) 
        {
            // If there is a valid room, perform the move
            time = Player.Instance.MoveTime;
            didBounce = false;
        }
        else
        {
            // If there is no valid room, bounce instead
            time = bounceTime = Player.Instance.MoveTime * .5f;
            didBounce = true;
        }
        
        hasEntered = true;
    }

    public override void OnUndo()
    {
        var newGoal = origin;
        var newOrigin = goal;

        origin = newOrigin;
        goal = newGoal;

        if(didBounce)
            time = bounceTime = Player.Instance.MoveTime * .5f;
        else
            time = Player.Instance.MoveTime;
    }

    public override StateReturn OnUpdate()
    {
        var frameGoal = didBounce ? origin + ((goal - origin) * .15f) : goal;

        if (time > 0)
        {
            time -= Time.deltaTime;

            var t = 1f - (time / Player.Instance.MoveTime);
            t = Easing.Quint_In(t);

            var vector = Vector3.Lerp(origin, frameGoal, t);

            transform.position = vector;

            return StateReturn.Running;
        }

        if (bounceTime > 0)
        {
            bounceTime -= Time.deltaTime;

            var t = 1f - (bounceTime / Player.Instance.MoveTime);
            t = Easing.Quint_Out(t);

            var vector = Vector3.Lerp(frameGoal, origin, t);

            transform.position = vector;

            DebugUtility.DrawDebugCross(origin, Color.green);
            DebugUtility.DrawDebugCross(frameGoal, Color.red);

            return StateReturn.Running;
        }

        Dungeon.Instance.UpdatePlayer();

        return StateReturn.Completed;
    }

    public override void OnExit()
    {
        if(didBounce)
        {
            var newGoal = origin;
            var newOrigin = goal;

            origin = newOrigin;
            goal = newGoal;
        }
    }

    public override bool HasEntered => hasEntered;
}