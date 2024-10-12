using UnityEngine;

[CreateAssetMenu(menuName = "AI/WatchOutStateConfig", fileName = "_Watch_Out_State_Config")]

public class WatchOutIdleStateConfig : AIStateConfig
{
    [Header("Idle Settings")]
    public Vector2 idleDuration = new Vector2(5f, 10f);
    public float idleTime = 0f;
    public float chanceToPatrol = 0.5f;

    [Header("Watch Settings")]
    public float viewDistance = 10f;
    public float viewAngle = 45f;
    public Vector3 viewOffset = new Vector3(0, 1f, 0);
    public LayerMask targetMask;

    public override AIStateType GetStateType()
    {
        return AIStateType.WatchOut;
    }

    public override AIState InitializeState(AIAgent agent)
    {
        WatchOutIdleState watchOutState = new WatchOutIdleState();
        watchOutState.SetWatchConfig(Random.Range(idleDuration.x, idleDuration.y), viewOffset, viewDistance, viewAngle, targetMask);
        return watchOutState;
    }
}
