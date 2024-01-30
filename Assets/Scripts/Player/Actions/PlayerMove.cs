using UnityEngine;

public class PlayerMove : BaseState
{
    private readonly Transform transform;
    private readonly float move;

    private float time;
    private float bounceTime;

    private Vector3 origin;
    private Vector3 goal;

    private bool didBounce;

    public PlayerMove(Transform transform, float move)
    {
        this.transform = transform;
        this.move = move;
    }

    public override void OnEnter()
    {
        origin = transform.position;
        goal = origin + (transform.forward * move);

        if (Dungeon.Instance.TileIsValid(goal)) 
        {
            // If there is a valid room, perform the move
            time = Player.Instance.MoveTime;
            
            if(Dungeon.Instance.TileIsValid(goal))
                Dungeon.Instance.UpdatePlayer(goal, transform.forward);
            
            didBounce = false;
        }
        else
        {
            // If there is no valid room, bounce instead
            time = bounceTime = Player.Instance.MoveTime * .5f;
            didBounce = true;
        }
    }

    public override void OnUndo()
    {
        var newGoal = origin;
        var newOrigin = goal;

        origin = newOrigin;
        goal = newGoal;

        if (didBounce)
        {
            time = bounceTime = Player.Instance.MoveTime * .5f;
        }
        else
        {
            time = Player.Instance.MoveTime;            
            
            if(Dungeon.Instance.TileIsValid(goal))
                Dungeon.Instance.UpdatePlayer(goal, transform.forward);
        }
    }

    public override StateReturn OnUpdate()
    {
        var frameGoal = didBounce ? origin + ((goal - origin) * .15f) : goal;

        if (time > 0)
        {
            time -= Time.deltaTime;

            var t = 1f - (time / Player.Instance.MoveTime);
            t = Easing.Expo_Out(t);

            var vector = Vector3.Lerp(origin, frameGoal, t);

            transform.position = vector;

            return StateReturn.Running;
        }

        if (bounceTime > 0)
        {
            bounceTime -= Time.deltaTime;

            var t = 1f - (bounceTime / Player.Instance.MoveTime);
            t = Easing.Expo_Out(t);

            var vector = Vector3.Lerp(frameGoal, origin, t);

            transform.position = vector;

            DebugUtility.DrawDebugCross(origin, Color.green);
            DebugUtility.DrawDebugCross(frameGoal, Color.red);

            return StateReturn.Running;
        }

        return StateReturn.Completed;
    }

    public override void OnExit()
    {
        if (!didBounce) return;
        
        var newGoal = origin;
        var newOrigin = goal;

        origin = newOrigin;
        goal = newGoal;
    }
}