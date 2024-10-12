using UnityEngine;
using UnityEngine.WSA;

public class IdleState : AIState
{
    private float idleTime;
    private float maxIdleTime;
    private float chanceToPatrol;


    public void EnterState(AIAgent agent)
    {
        agent.AiAnimator.SetBool("Idle", true);
        idleTime = 0f;
    }

    public void UpdateState(AIAgent agent)
    {
        idleTime += Time.deltaTime;

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
        return AIStateType.Idle;
    }

    public void SetIdleConfig(float idleTime, float maxIdleTime, float chanceToPatrol)
    {
        this.idleTime = idleTime;
        this.maxIdleTime = maxIdleTime;
        this.chanceToPatrol = chanceToPatrol;
    }

    public void DrawGizmos(AIAgent agent)
    {
        Gizmos.color = Color.blue;

        float idleProgress = idleTime / maxIdleTime;
        float idleRadius = Mathf.Lerp(0.2f, 1f, idleProgress);
        Gizmos.DrawSphere(agent.transform.position, idleRadius);

        UnityEditor.Handles.Label(agent.transform.position + Vector3.up * 2f, $"Chance to Patrol: {chanceToPatrol * 100}%");
    }
}
