using UnityEngine;

[CreateAssetMenu(menuName = "AI/ChaseTargetStateConfig", fileName = "_Chase_Target_State_Config")]
public class ChaseTargetStateConfig : AIStateConfig
{
    public float chaseSpeed = 5f;
    public float maxChaseDistance = 20f;
    public float chasingViewAngle = 45f;
    public float maxTimeOutOfView = 5f;  // AI will stop chasing after this time

    public override AIStateType GetStateType()
    {
        return AIStateType.Chase;
    }

    public override AIState InitializeState(AIAgent agent)
    {
        ChaseTargetState chaseState = new ChaseTargetState();
        chaseState.SetChaseConfig(chaseSpeed, maxChaseDistance, chasingViewAngle, maxTimeOutOfView);
        return chaseState;
    }
}
