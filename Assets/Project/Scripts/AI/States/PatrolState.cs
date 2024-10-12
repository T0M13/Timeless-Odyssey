using UnityEngine;

public class PatrolState : AIState
{
    private Vector3 targetPosition;
    private float patrolSpeed;
    private float patrolRadius;
    private float patrolTime;
    private float maxPatrolTime;
    private float chanceToIdle;
    private float targetReachThreshold;
    private float viewAngle;
    private Vector3 viewOffset;
    private LayerMask targetMask;

    public void SetPatrolConfig(float patrolSpeed, float patrolRadius, float maxPatrolTime, float chanceToIdle, float targetReachThreshold, float viewAngle, Vector3 viewOffset, LayerMask targetMask)
    {
        this.patrolSpeed = patrolSpeed;
        this.patrolRadius = patrolRadius;
        this.maxPatrolTime = maxPatrolTime;
        this.chanceToIdle = chanceToIdle;
        this.targetReachThreshold = targetReachThreshold;
        this.viewAngle = viewAngle;
        this.viewOffset = viewOffset;
        this.targetMask = targetMask;
    }

    public void EnterState(AIAgent agent)
    {
        SetNewTargetPosition(agent);
        agent.AiAnimator.SetBool("Walking", true);
        agent.NavMeshAgent.speed = patrolSpeed;
        agent.NavMeshAgent.isStopped = false;
        agent.NavMeshAgent.SetDestination(targetPosition);
        patrolTime = 0f;
    }

    public void UpdateState(AIAgent agent)
    {
        patrolTime += Time.deltaTime;

        // Patrol timeout and chance to idle
        if (patrolTime >= maxPatrolTime)
        {
            patrolTime = 0f;
            if (Random.value < chanceToIdle && agent.StateMachine.HasState(AIStateType.Idle))
            {
                agent.TransitionToState(AIStateType.Idle);
                return;
            }
        }

        // Check for target and transition to Chase state if found
        if (LookForTarget(agent) && agent.StateMachine.HasState(AIStateType.Chase))
        {
            agent.TransitionToState(AIStateType.Chase);
            return;
        }

        // Continue patrolling
        if (IsTargetWithinView(agent))
        {
            if (agent.NavMeshAgent.remainingDistance <= targetReachThreshold)
            {
                SetNewTargetPosition(agent);
                agent.NavMeshAgent.SetDestination(targetPosition);
            }
        }

        UpdateMovementVector(agent);
    }

    public void ExitState(AIAgent agent)
    {
        agent.AiAnimator.SetBool("Walking", false);
        agent.movementVector = Vector2.zero;
        agent.NavMeshAgent.isStopped = true;
    }

    // Check if a target is within view and assign it to the AI
    private bool LookForTarget(AIAgent agent)
    {
        if (agent.targetTransform != null) return true;

        Vector3 viewPosition = agent.transform.position + viewOffset;

        // Detect targets in the patrol area
        Collider[] targetsInView = Physics.OverlapSphere(viewPosition, patrolRadius, targetMask);

        foreach (Collider col in targetsInView)
        {
            Vector3 directionToTarget = (col.transform.position - viewPosition).normalized;
            float angleToTarget = Vector3.Angle(agent.transform.forward, directionToTarget);

            // Check if the target is within the patrol's view angle
            if (angleToTarget < viewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(viewPosition, directionToTarget, out hit, patrolRadius))
                {
                    if (hit.transform == col.transform)
                    {
                        agent.targetTransform = hit.transform;
                        return true;  // Target found
                    }
                }
            }
        }

        return false;  // No target found
    }

    public AIStateType GetStateType()
    {
        return AIStateType.Patrol;
    }

    // Set a new patrol target position
    private void SetNewTargetPosition(AIAgent agent)
    {
        float randomAngle = Random.Range(-viewAngle / 2, viewAngle / 2);
        Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * agent.transform.forward;
        targetPosition = agent.transform.position + direction * patrolRadius;
    }

    // Update the movement vector based on velocity
    private void UpdateMovementVector(AIAgent agent)
    {
        Vector3 localVelocity = agent.transform.InverseTransformDirection(agent.NavMeshAgent.velocity);
        agent.movementVector = new Vector2(localVelocity.x, localVelocity.z);
    }

    // Check if the patrol target position is within view
    private bool IsTargetWithinView(AIAgent agent)
    {
        Vector3 directionToTarget = (targetPosition - agent.transform.position).normalized;
        float angleToTarget = Vector3.Angle(agent.transform.forward, directionToTarget);

        return angleToTarget <= viewAngle / 2;
    }

    // Draw Gizmos for visualization
    public void DrawGizmos(AIAgent agent)
    {
        Gizmos.color = Color.green;

        // Draw patrol radius
        Gizmos.DrawWireSphere(agent.transform.position, patrolRadius);

        // Draw view angles
        Vector3 forwardDirection = agent.transform.forward * patrolRadius;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forwardDirection;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forwardDirection;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(agent.transform.position, agent.transform.position + leftBoundary);
        Gizmos.DrawLine(agent.transform.position, agent.transform.position + rightBoundary);

        // Draw target position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.3f);

        UnityEditor.Handles.Label(agent.transform.position + Vector3.up * 2f, $"Chance to Idle: {chanceToIdle * 100}%");
    }
}
