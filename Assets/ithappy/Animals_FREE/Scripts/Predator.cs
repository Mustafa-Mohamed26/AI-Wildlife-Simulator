using UnityEngine;
using UnityEngine.AI;

public class Predator : MonoBehaviour
{
    public float hunger = 100f;
    public float hungerDecreaseRate = 3f;

    public float roamRadius = 35f;
    public float detectionRadius = 50f;   // محيط البحث عن الفريسة
    public string preyTag = "Animal";

    public float stamina = 100f;
    public float staminaDecreaseRate = 5f;
    public float staminaRecoveryRate = 10f;
    public float minStaminaToHunt = 20f;  // الحد الأدنى للقدرة على المطاردة

    private NavMeshAgent agent;
    private Animator animator;
    private AnimationHandler animationHandler;

    private enum State { Idle, Walking, Running, Hunting, Eating }
    private State currentState;

    private float idleDuration = 3f;
    private float walkDuration = 6f;
    private float runDuration = 4f;

    private float stateTimer = 0f;
    private Vector3 currentTargetPoint;

    private float eatingDuration = 3f;
    private float eatingTimer = 0f;
    private GameObject currentPrey;

    private float hungerTimer = 0f;       // عداد تقليل الجوع كل ثانية

    private int previousPreyCount = 0;    // لتتبع عدد الفريسة بين التحديثات

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animationHandler = new AnimationHandler(animator, "Vert", "State");

        SetState(State.Idle);
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

        hungerTimer += Time.deltaTime;
        if (hungerTimer >= 1f)
        {
            DecreaseHunger();
            hungerTimer = 0f;
        }

        // تتبع عدد الفريسة حالياً
        int currentPreyCount = GameObject.FindGameObjectsWithTag(preyTag).Length;

        // إذا انخفض عدد الفريسة يعني تم قتل واحدة -> إعادة تعبئة الجوع
        if (currentPreyCount < previousPreyCount)
        {
            hunger = 100f;
            Debug.Log("Prey killed! Hunger reset to 100.");
        }

        previousPreyCount = currentPreyCount;

        // عرض قيم الجوع والطاقة والحالة الحالية في Console
        Debug.Log($"Hunger: {hunger:F1}, Stamina: {stamina:F1}, CurrentState: {currentState}");

        // محاولة البحث عن فريسة فقط إذا ليست في حالة Hunting أو Eating
        if ((currentState != State.Hunting && currentState != State.Eating) && hunger <= 50f && stamina >= minStaminaToHunt)
        {
            FindAndHuntPrey();
        }

        switch (currentState)
        {
            case State.Idle:
                RecoverStamina();

                if (stateTimer >= GetStateDuration(currentState))
                {
                    SetState(GetRandomMoveState());
                }
                break;

            case State.Walking:
                RecoverStamina();

                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    MoveToRandomPoint();

                if (stateTimer >= GetStateDuration(currentState))
                    SetState(GetRandomMoveState());

                break;

            case State.Running:
                RecoverStamina();

                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    MoveToRandomPoint();

                if (stateTimer >= GetStateDuration(currentState))
                    SetState(GetRandomMoveState());

                break;

            case State.Hunting:
                stamina -= staminaDecreaseRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, 100);

                if (stamina <= 0f)
                {
                    currentPrey = null;
                    SetState(State.Idle);
                    return;
                }

                if (currentPrey == null)
                {
                    SetState(State.Idle);
                    return;
                }

                agent.SetDestination(currentPrey.transform.position);

                float distance = Vector3.Distance(transform.position, currentPrey.transform.position);
                if (distance < 2f)
                {
                    Destroy(currentPrey);
                    currentPrey = null;
                    SetState(State.Eating);
                }
                break;

            case State.Eating:
                eatingTimer += Time.deltaTime;
                if (eatingTimer >= eatingDuration)
                {
                    hunger = 100f; // إعادة تعبئة الجوع (احتياطي)
                    SetState(State.Idle);
                }
                break;
        }

        // تحديث التحريك والأنيميشن حسب الحالة
        AnimateState(currentState, Time.deltaTime);
    }

    void SetState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        stateTimer = 0f;

        switch (newState)
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
                agent.speed = 6f;
                agent.isStopped = false;
                MoveToRandomPoint();
                break;

            case State.Hunting:
                agent.speed = 7f;
                agent.isStopped = false;
                break;

            case State.Eating:
                agent.isStopped = true;
                eatingTimer = 0f;
                break;
        }
    }

    void FindAndHuntPrey()
    {
        GameObject[] preyList = GameObject.FindGameObjectsWithTag(preyTag);
        float minDistance = Mathf.Infinity;
        GameObject closest = null;

        //Debug.Log($"Finding prey. Prey count: {preyList.Length}");

        foreach (var prey in preyList)
        {
            float dist = Vector3.Distance(transform.position, prey.transform.position);
            if (dist < minDistance && dist <= detectionRadius)
            {
                minDistance = dist;
                closest = prey;
            }
        }

        if (closest != null && stamina >= minStaminaToHunt)
        {
            currentPrey = closest;
            //Debug.Log("Prey found. Switching to Hunting.");
            SetState(State.Hunting);
        }
        else
        {
            //Debug.Log("No suitable prey found or not enough stamina.");
        }
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
        switch (val)
        {
            case 0: return State.Idle;
            case 1: return State.Walking;
            case 2: return State.Running;
            default: return State.Idle;
        }
    }

    void DecreaseHunger()
    {
        if (hunger > 0)
        {
            hunger -= hungerDecreaseRate;
            hunger = Mathf.Clamp(hunger, 0, 100);
        }
    }

    void RecoverStamina()
    {
        stamina += staminaRecoveryRate * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, 100);
    }

    void AnimateState(State state, float deltaTime)
    {
        Vector2 animAxis = Vector2.zero;
        float animState = 0f;

        switch (state)
        {
            case State.Idle:
                animAxis = Vector2.zero;
                animState = 0f;
                break;
            case State.Walking:
                animAxis = new Vector2(0, 1);
                animState = 1f;
                break;
            case State.Running:
            case State.Hunting:
                animAxis = new Vector2(0, 2);
                animState = 2f;
                break;
            case State.Eating:
                animAxis = Vector2.zero;
                animState = 0f;
                break;
        }

        animationHandler.Animate(animAxis, animState, deltaTime);
    }

    // Animation Handler Class
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
            m_FlowState = Mathf.Clamp01(m_FlowState + k_InputFlow * deltaTime * Mathf.Sign(state - m_FlowState));
        }
    }
}
