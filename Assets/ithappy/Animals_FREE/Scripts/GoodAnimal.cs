using UnityEngine;
using UnityEngine.AI;

public class GoodAnimal : MonoBehaviour
{
    public float hunger = 200f;
    public float thirst = 200f;

    public float hungerDecreaseRate = 0.8f;   // مناسب لـ 2-3 دقائق
    public float thirstDecreaseRate = 0.7f;   // مناسب لـ 2-3 دقائق

    public float hungerThreshold = 80f;
    public float thirstThreshold = 100f;

    public string foodTag = "Bush";
    public string waterTag = "Water";
    public string predatorTag = "Predator"; // Tag for predators

    public float roamRadius = 10f;
    public float observationRadius = 15f; // Radius to detect predators
    public float dangerRadius = 5f; // Radius for critical danger

    public float stamina = 100f; // Stamina for fleeing and running
    public float staminaDecreaseRate = 5f; // Stamina decrease rate during fleeing
    public float staminaRecoverRate = 10f; // Stamina recover rate when not fleeing
    public float minStaminaToRun = 10f; // Minimum stamina to allow running or fleeing

    private NavMeshAgent agent;
    private Animator animator;

    private enum State { Idle, Walking, Running, Eating, Drinking, Fleeing, Dead }
    private State currentState;

    private float stateTimer = 0f;
    private float idleDuration = 3f;
    private float walkDuration = 10f;
    private float runDuration = 6f;

    private Vector3 currentTargetPoint;

    private AnimationHandler animationHandler;

    private float fleeingDuration = 5f; // Duration of fleeing before returning to Idle
    private float fleeingTimer = 0f;   // Timer for fleeing state

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        animationHandler = new AnimationHandler(animator, "Vert", "State");

        SetState(State.Idle);
        InvokeRepeating("DecreaseNeeds", 1f, 1f);
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

        if (currentState != State.Dead)
        {
            CheckForPredators(); // Check for predators in every frame
        }

        switch (currentState)
        {
            case State.Idle:
            case State.Walking:
            case State.Running:
                if (stateTimer >= GetStateDuration(currentState))
                {
                    SetState(GetRandomMoveState());
                }

                if ((currentState == State.Walking || currentState == State.Running) && !agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    MoveToRandomPoint();
                }

                CheckNeeds();
                break;

            case State.Eating:
                if (!agent.pathPending && agent.remainingDistance < 1f)
                {
                    Eat();
                }
                break;

            case State.Drinking:
                if (!agent.pathPending && agent.remainingDistance < 1f)
                {
                    Drink();
                }
                break;

            case State.Fleeing:
                fleeingTimer += Time.deltaTime; // Update fleeing timer

                // Decrease stamina only when fleeing
                stamina -= staminaDecreaseRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0f, 100f);

                if (stamina <= 0f)
                {
                    stamina = 0f;
                    SetState(State.Walking); // Switch to walking if stamina is depleted
                    CheckForCriticalDanger();
                }

                // If fleeing duration is over and no predator is detected, return to Idle
                if (fleeingTimer >= fleeingDuration)
                {
                    SetState(State.Idle);
                }
                break;

            case State.Dead:
                // Do nothing, the animal is dead
                break;
        }

        // Recover stamina when not fleeing or dead
        if (currentState != State.Fleeing && currentState != State.Dead)
        {
            stamina += staminaRecoverRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, 100f);
        }

        Vector2 animAxis = Vector2.zero;
        float animState = 0f;

        switch (currentState)
        {
            case State.Idle:
                animState = 0f;
                animAxis = Vector2.zero;
                break;
            case State.Walking:
                animAxis = new Vector2(0, 1);
                animState = 1f;
                break;
            case State.Running:
            case State.Fleeing:
                animAxis = new Vector2(0, 2);
                animState = 2f;
                break;
            case State.Eating:
            case State.Drinking:
                animAxis = new Vector2(0, 1);
                animState = 1f;
                break;
        }

        animationHandler.Animate(animAxis, animState, Time.deltaTime);
    }

    void CheckNeeds()
    {
        if (currentState == State.Fleeing || currentState == State.Dead) return;

        if (thirst <= thirstThreshold)
        {
            SetState(State.Drinking);
            MoveToClosestTarget(waterTag);
        }
        else if (hunger <= hungerThreshold)
        {
            SetState(State.Eating);
            MoveToClosestTarget(foodTag);
        }
    }

    void CheckForPredators()
    {
        GameObject[] predators = GameObject.FindGameObjectsWithTag(predatorTag);

        foreach (GameObject predator in predators)
        {
            float distance = Vector3.Distance(transform.position, predator.transform.position);
            if (distance <= observationRadius && stamina > 0f) // Only flee if stamina > 0
            {
                SetState(State.Fleeing);
                MoveAwayFromPredator(predator.transform.position);
                fleeingTimer = 0f; // Reset fleeing timer when a predator is detected
                break;
            }
        }
    }

    void CheckForCriticalDanger()
    {
        GameObject[] predators = GameObject.FindGameObjectsWithTag(predatorTag);
        foreach (GameObject predator in predators)
        {
            float distance = Vector3.Distance(transform.position, predator.transform.position);
            if (distance <= dangerRadius)
            {
                SetState(State.Dead);
                Destroy(gameObject); // Remove the animal from the scene
                break;
            }
        }
    }

    void MoveAwayFromPredator(Vector3 predatorPosition)
    {
        Vector3 fleeDirection = (transform.position - predatorPosition).normalized * roamRadius;
        Vector3 fleeTarget = transform.position + fleeDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, roamRadius, NavMesh.AllAreas))
        {
            currentTargetPoint = hit.position;
            agent.SetDestination(currentTargetPoint);
        }
    }

    void SetState(State newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            stateTimer = 0f;

            Debug.Log("Current State: " + currentState.ToString());

            switch (currentState)
            {
                case State.Idle:
                    agent.isStopped = true;
                    break;

                case State.Walking:
                    agent.speed = 3f;
                    agent.isStopped = false;
                    MoveToRandomPoint();
                    break;

                case State.Running:
                    if (stamina < minStaminaToRun)
                    {
                        SetState(State.Walking);
                        return;
                    }
                    agent.speed = 6f;
                    agent.isStopped = false;
                    MoveToRandomPoint();
                    break;

                case State.Eating:
                case State.Drinking:
                    agent.speed = 6f;
                    agent.isStopped = false;
                    break;

                case State.Fleeing:
                    agent.speed = 8f; // Faster speed for fleeing
                    agent.isStopped = false;
                    break;

                case State.Dead:
                    agent.isStopped = true;
                    break;
            }
        }
    }

    float GetStateDuration(State state)
    {
        switch (state)
        {
            case State.Idle: return idleDuration;
            case State.Walking: return walkDuration;
            case State.Running: return runDuration;
            default: return 0f;
        }
    }

    State GetRandomMoveState()
    {
        int val = Random.Range(0, 3);

        if (val == 2 && stamina < minStaminaToRun)
        {
            val = Random.Range(0, 2); // Only Idle or Walking
        }

        switch (val)
        {
            case 0: return State.Idle;
            case 1: return State.Walking;
            case 2: return State.Running;
            default: return State.Idle;
        }
    }

    void DecreaseNeeds()
    {
        if (currentState == State.Dead) return;

        hunger -= hungerDecreaseRate;
        hunger = Mathf.Clamp(hunger, 0, 200);

        thirst -= thirstDecreaseRate;
        thirst = Mathf.Clamp(thirst, 0, 200);
    }

    void Eat()
    {
        hunger = 200f;
        SetState(State.Idle);
        agent.isStopped = true;
    }

    void Drink()
    {
        thirst = 200f;
        SetState(State.Idle);
        agent.isStopped = true;
    }

    void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            currentTargetPoint = hit.position;
            agent.SetDestination(currentTargetPoint);
        }
    }

    void MoveToClosestTarget(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = target;
            }
        }

        if (closest != null)
        {
            currentTargetPoint = closest.transform.position;
            agent.SetDestination(currentTargetPoint);
        }
    }

    // AnimationHandler class remains unchanged
    private class AnimationHandler
    {
        private readonly Animator m_Animator;
        private readonly string m_VerticalID;
        private readonly string m_StateID;

        private readonly float k_InputFlow = 4.5f;

        private float m_FlowState;
        private Vector2 m_FlowAxis;

        public AnimationHandler(Animator animator, string verticalID, string stateID)
        {
            m_Animator = animator;
            m_VerticalID = verticalID;
            m_StateID = stateID;
        }

        public void Animate(in Vector2 axis, float state, float deltaTime)
        {
            m_Animator.SetFloat(m_VerticalID, m_FlowAxis.magnitude);
            m_Animator.SetFloat(m_StateID, Mathf.Clamp01(m_FlowState));

            m_FlowAxis = Vector2.ClampMagnitude(m_FlowAxis + k_InputFlow * deltaTime * (axis - m_FlowAxis).normalized, 1f);
            m_FlowState = Mathf.Clamp01(m_FlowState + k_InputFlow * deltaTime * (state - m_FlowState));
        }
    }
}
