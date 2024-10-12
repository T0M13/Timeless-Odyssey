using UnityEngine;

[CreateAssetMenu(menuName = "AI/IdleStateConfig", fileName = "_Idle_State_Config")]

public class IdleStateConfig : AIStateConfig
{
    public Vector2 idleDuration = new Vector2(5f, 10f);
    public float idleTime = 0f;
    public float chanceToPatrol = 0.5f;

    public override AIStateType GetStateType()
    {
        return AIStateType.Idle;
    }

    public override AIState InitializeState(AIAgent agent)
    {
        IdleState idleState = new IdleState();
        idleState.SetIdleConfig(idleTime, Random.Range(idleDuration.x, idleDuration.y), chanceToPatrol);
        return idleState;
    }
}
