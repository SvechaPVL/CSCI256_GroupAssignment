using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("[EnemyAI] No object with tag 'Player' found. Set your player tag to 'Player'.");
            return;
        }

        player = playerObj.transform;
        playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("[EnemyAI] Player does not have PlayerHealth component.");
        }

        // Important: stoppingDistance should be slightly LESS than attackRange
        if (agent != null && agent.stoppingDistance >= attackRange)
        {
            Debug.LogWarning("[EnemyAI] NavMeshAgent.stoppingDistance >= attackRange. Adjusting automatically.");
            agent.stoppingDistance = attackRange - 0.3f;
        }
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Debug distance
        // Debug.Log("[EnemyAI] Distance to player: " + distance);

        if (distance <= detectionRange && distance > attackRange)
        {
            // Chase
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else if (distance <= attackRange)
        {
            // Attack
            agent.isStopped = true;
            FacePlayer();
            TryAttack(distance);
        }
        else
        {
            // Idle
            agent.isStopped = true;
        }
    }

    void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }

    void TryAttack(float distance)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        if (distance > attackRange + 0.2f)
        {
            // Safety check
            return;
        }

        Debug.Log("[EnemyAI] ATTACK triggered.");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning("[EnemyAI] Cannot damage player, PlayerHealth is missing.");
        }

        // Hook up to Animator if you have one:
        // animator.SetTrigger("Attack");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
