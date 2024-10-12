using UnityEngine;

public class WatchOutIdleState : AIState
{
    private float idleTime;
    private float maxIdleTime;
    private float chanceToPatrol;

    [Header("Watch Config")]
    private Vector3 viewOffset;
    private float viewDistance;
    private float viewAngle;
    private LayerMask targetMask;

    public void EnterState(AIAgent agent)
    {
        agent.AiAnimator.SetBool("Idle", true);
        idleTime = 0f;
    }

    public void UpdateState(AIAgent agent)
    {
        idleTime += Time.deltaTime;

        if (LookForTarget(agent) && agent.StateMachine.HasState(AIStateType.Chase))
        {
            agent.TransitionToState(AIStateType.Chase);
            return;
        }

        if (idleTime >= maxIdleTime)
        {
            idleTime = 0f;

            if (Random.value < chanceToPatrol && agent.StateMachine.HasState(AIStateType.Patrol))
            {
                agent.TransitionToState(AIStateType.Patrol);
                return;
            }
        }
    }

    public void ExitState(AIAgent agent)
    {
        agent.AiAnimator.SetBool("Idle", false);
    }

    public AIStateType GetStateType()
    {
        return AIStateType.WatchOut;
    }

    public void SetWatchConfig(float maxIdleTime, Vector3 viewOffset, float viewDistance, float viewAngle, LayerMask targetMask)
    {
        this.maxIdleTime = maxIdleTime;
        this.viewOffset = viewOffset;
        this.viewDistance = viewDistance;
        this.viewAngle = viewAngle;
        this.targetMask = targetMask;
    }

    private bool LookForTarget(AIAgent agent)
    {
        if (agent.targetTransform != null) return true;

        Vector3 viewPosition = agent.transform.position + viewOffset;

        Collider[] targetsInView = Physics.OverlapSphere(viewPosition, viewDistance, targetMask);

        foreach (Collider col in targetsInView)
        {
            Vector3 directionToTarget = (col.transform.position - viewPosition).normalized;
            float angleToTarget = Vector3.Angle(agent.transform.forward, directionToTarget);

            if (angleToTarget < viewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(viewPosition, directionToTarget, out hit, viewDistance))
                {
                    if (hit.transform == col.transform)
                    {
                        agent.targetTransform = hit.transform;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void DrawGizmos(AIAgent agent)
    {
        Gizmos.color = Color.red;

        Vector3 viewPosition = agent.transform.position + viewOffset;

        Gizmos.DrawWireSphere(viewPosition, viewDistance);

        Vector3 forwardDirection = agent.transform.forward * viewDistance;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forwardDirection;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forwardDirection;

        Gizmos.DrawLine(viewPosition, viewPosition + leftBoundary);
        Gizmos.DrawLine(viewPosition, viewPosition + rightBoundary);

        UnityEditor.Handles.Label(agent.transform.position + Vector3.up * 2f, $"Viewing for target within {viewDistance} units.");
    }
}
