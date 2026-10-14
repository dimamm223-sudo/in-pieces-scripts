using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Точки патрулирования")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitAtPointTime = 2f;

    [Header("Обнаружение игрока")]
    [SerializeField] private Transform player;
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask; // стены, укрытия
    [SerializeField] private string playerTag = "Player";

    [Header("Погоня")]
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float reactionDelay = 1f; // "чуть постоит" перед погоней
    [SerializeField] private float stopDistance = 1.2f; // дистанция до игрока

    private NavMeshAgent agent;
    private int currentPointIndex;
    private float waitTimer;
    private float reactionTimer;
    private bool isReacting;

    private enum State { Patrol, Reacting, Chase }
    private State state = State.Patrol;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Автопоиск игрока, если не задан вручную
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }
    }

    private void Start()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.speed = patrolSpeed;
            GoToNextPatrolPoint();
        }
        else
        {
            state = State.Patrol;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Reacting: UpdateReacting(); break;
            case State.Chase: UpdateChase(); break;
        }
    }

    // ---------------- ПАТРУЛЬ ----------------
    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            StartReacting();
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtPointTime)
            {
                waitTimer = 0f;
                GoToNextPatrolPoint();
            }
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.speed = patrolSpeed;
        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentPointIndex].position);
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    // ---------------- РЕАКЦИЯ (стоит чуть-чуть) ----------------
    private void StartReacting()
    {
        state = State.Reacting;
        reactionTimer = 0f;
        agent.isStopped = true;
        agent.ResetPath();

        // Разворот в сторону игрока
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    private void UpdateReacting()
    {
        // Если игрок убежал из поля зрения — вернуться к патрулю
        if (!CanSeePlayer())
        {
            state = State.Patrol;
            agent.isStopped = false;
            if (patrolPoints != null && patrolPoints.Length > 0)
                GoToNextPatrolPoint();
            return;
        }

        reactionTimer += Time.deltaTime;
        if (reactionTimer >= reactionDelay)
        {
            state = State.Chase;
            agent.isStopped = false;
            agent.speed = chaseSpeed;
        }
    }

    // ---------------- ПОГОНЯ ----------------
    private void UpdateChase()
    {
        if (player == null) { state = State.Patrol; return; }

        // Если долго не видим игрока — теряем цель
        if (!CanSeePlayer())
        {
            state = State.Patrol;
            agent.speed = patrolSpeed;
            if (patrolPoints != null && patrolPoints.Length > 0)
                GoToNextPatrolPoint();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Дошёл до игрока — остановился
            agent.isStopped = true;
        }
    }

    // ---------------- ПРОВЕРКА ВИДИМОСТИ ----------------
    private bool CanSeePlayer()
    {
    if (player == null) return false;

    Vector3 origin = transform.position + Vector3.up * 1.5f;
    Vector3 target = player.position + Vector3.up * 1.0f;
    Vector3 dir = target - origin;

    // Дистанция
    if (dir.magnitude > viewDistance) return false;

    // Угол обзора
    float angle = Vector3.Angle(transform.forward, dir);
    if (angle > viewAngle * 0.5f) return false;

    return true;
    }

    // ---------------- ГИЗМО ----------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}