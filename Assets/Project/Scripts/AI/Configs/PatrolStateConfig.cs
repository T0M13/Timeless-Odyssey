using UnityEngine;

[CreateAssetMenu(menuName = "AI/PatrolStateConfig", fileName = "_Patrol_State_Config")]
public class PatrolStateConfig : AIStateConfig
{
    public float patrolSpeed = 2f;
    public float patrolRadius = 10f;
    public float maxPatrolTime = 5f;
    public float chanceToIdle = 0.3f;
    public float targetReachThreshold = 0.5f;
    public float viewAngle = 90f;
    public Vector3 viewOffset = new Vector3(0, 1f, 0);
    public LayerMask targetMask;

    public override AIStateType GetStateType()
    {
        return AIStateType.Patrol;
    }

    public override AIState InitializeState(AIAgent agent)
    {
        PatrolState patrolState = new PatrolState();
        patrolState.SetPatrolConfig(patrolSpeed, patrolRadius, maxPatrolTime, chanceToIdle, targetReachThreshold, viewAngle, viewOffset, targetMask);
        return patrolState;
    }
}
