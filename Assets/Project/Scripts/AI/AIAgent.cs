using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AIStateMachine stateMachine;
    [SerializeField] private Animator aiAnimator;
    [SerializeField] private Rigidbody aiBody;
    [SerializeField] private CapsuleCollider aiCollider;
    [SerializeField] private NavMeshAgent navMeshAgent;

    [Header("State Configurations")]
    [SerializeField] private List<AIStateConfig> stateConfigs;
    [SerializeField][ShowOnly] private AIStateType currentStateType;

    [Header("Movement")]
    public Vector2 movementVector;

    [Header("Follow/Chase Target")]
    public Transform targetTransform;


    public AIStateMachine StateMachine { get => stateMachine; set => stateMachine = value; }
    public Animator AiAnimator { get => aiAnimator; set => aiAnimator = value; }
    public Rigidbody AiBody { get => aiBody; set => aiBody = value; }
    public CapsuleCollider AiCollider { get => aiCollider; set => aiCollider = value; }
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; set => navMeshAgent = value; }


    private void OnValidate()
    {
        GetReferences();
    }

    private void Awake()
    {
        GetReferences();
        InitializeStates();
    }

    private void Start()
    {
        StartState();
    }

    private void Update()
    {
        stateMachine.Update(this);
        UpdateAnimator();
    }

    private void GetReferences()
    {
        if (aiAnimator == null)
        {
            try { aiAnimator = GetComponentInChildren<Animator>(); }
            catch { Debug.Log("AIAnimator Missing from AIAgent"); }
        }

        if (aiBody == null)
        {
            try { aiBody = GetComponent<Rigidbody>(); }
            catch { Debug.Log("Rigidbody Missing from AIAgent"); }
        }

        if (aiCollider == null)
        {
            try { aiCollider = GetComponent<CapsuleCollider>(); }
            catch { Debug.Log("CapsuleCollider Missing from AIAgent"); }
        }

        if (navMeshAgent == null)
        {
            try { navMeshAgent = GetComponent<NavMeshAgent>(); }
            catch { Debug.Log("NavMeshAgent Missing from AIAgent"); }
        }
    }

    private void InitializeStates()
    {
        var states = new Dictionary<AIStateType, AIState>();

        foreach (var config in stateConfigs)
        {
            AIState state = config.InitializeState(this);
            states.Add(config.GetStateType(), state);
        }

        stateMachine = new AIStateMachine(states);
    }

    private void StartState()
    {
        currentStateType = stateConfigs[0].GetStateType();
        stateMachine.Initialize(currentStateType, this);
    }

    private void UpdateAnimator()
    {
        aiAnimator.SetFloat("xPos", movementVector.x);
        aiAnimator.SetFloat("yPos", movementVector.y);
    }

    public void TransitionToState(AIStateType newState)
    {
        stateMachine.ChangeState(newState, this);
        currentStateType = newState;
    }

    private void OnDrawGizmosSelected()
    {
        if (stateMachine != null)
        {
            stateMachine.DrawGizmos(this);
        }
    }
}
