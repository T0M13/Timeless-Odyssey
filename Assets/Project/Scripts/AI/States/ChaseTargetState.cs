using UnityEngine;
using UnityEngine.AI;

public class ChaseTargetState : AIState
{
    private float chaseSpeed;
    private float maxChaseDistance;
    private float chasingViewAngle;
    private float timeOutOfView = 0f;
    private float maxTimeOutOfView;
    private Transform target;

    public void EnterState(AIAgent agent)
    {
        agent.AiAnimator.SetBool("Running", true);
        target = agent.targetTransform;
        agent.NavMeshAgent.speed = chaseSpeed;
        agent.NavMeshAgent.isStopped = false;
        timeOutOfView = 0f;  // Reset timer when chase starts
    }

    public void UpdateState(AIAgent agent)
    {
        if (target == null && agent.StateMachine.HasState(AIStateType.Idle))
        {
            agent.TransitionToState(AIStateType.Idle);
            return;
        }

        agent.NavMeshAgent.SetDestination(target.position);

        if (!CanSeeTarget(agent))
        {
            timeOutOfView += Time.deltaTime;

            if (timeOutOfView >= maxTimeOutOfView && agent.StateMachine.HasState(AIStateType.Patrol))
            {
                agent.targetTransform = null;  
                agent.TransitionToState(AIStateType.Patrol);
                return;
            }
        }
        else
        {
            timeOutOfView = 0f;  
        }

        float distanceToTarget = Vector3.Distance(agent.transform.position, target.position);
        if (distanceToTarget > maxChaseDistance && agent.StateMachine.HasState(AIStateType.Patrol))
        {
            agent.targetTransform = null;  
            agent.TransitionToState(AIStateType.Patrol);
            return;
        }
    }


    public void ExitState(AIAgent agent)
    {
        agent.AiAnimator.SetBool("Running", false);
        agent.NavMeshAgent.isStopped = true;
    }

    public AIStateType GetStateType()
    {
        return AIStateType.Chase;
    }

    // Set values from the config
    public void SetChaseConfig(float chaseSpeed, float maxChaseDistance, float chasingViewAngle, float maxTimeOutOfView)
    {
        this.chaseSpeed = chaseSpeed;
        this.maxChaseDistance = maxChaseDistance;
        this.chasingViewAngle = chasingViewAngle;
        this.maxTimeOutOfView = maxTimeOutOfView;
    }

    // Check if the target is within the chasing view angle
    private bool CanSeeTarget(AIAgent agent)
    {
        Vector3 directionToTarget = (target.position - agent.transform.position).normalized;
        float angleToTarget = Vector3.Angle(agent.transform.forward, directionToTarget);

        return angleToTarget <= chasingViewAngle / 2;
    }

    public void DrawGizmos(AIAgent agent)
    {
        if (agent.targetTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(agent.transform.position, agent.targetTransform.position);  // Draw line to the target
        }

        // Draw chase range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(agent.transform.position, maxChaseDistance);

        // Draw the chasing view angle
        Vector3 forwardDirection = agent.transform.forward * maxChaseDistance;
        Vector3 leftBoundary = Quaternion.Euler(0, -chasingViewAngle / 2, 0) * forwardDirection;
        Vector3 rightBoundary = Quaternion.Euler(0, chasingViewAngle / 2, 0) * forwardDirection;

        Gizmos.DrawLine(agent.transform.position, agent.transform.position + leftBoundary);
        Gizmos.DrawLine(agent.transform.position, agent.transform.position + rightBoundary);
    }
}
