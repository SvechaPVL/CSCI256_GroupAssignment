using UnityEngine;
using UnityEngine.AI;

public class WaspFlying : MonoBehaviour
{
    [Header("Attack Damage")]
    public int AttackDamage = 10; 

    private NavMeshAgent m_agent;
    private Animator m_animator;
    private Transform m_target;
    

    // Override the NavMesh agent's speed
    public float MoveSpeed;
    // Set the base offset height dynamically
    public float MaxHeight;
    public float MinHeight;
    public float AscentSpeed = 3.5f;
    public float DescentSpeed = 3.5f;
    public float AttackDistance = 1.5f;
    private int m_attackRate = 2;
    private bool m_isDescending;
    private bool m_isAscending;
    private float m_distanceToPlayer;
    private float m_animSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_target = GameObject.FindGameObjectWithTag("Player").transform;
        // Random flapping wings animatrion timing
        float randomAnimSpeed = Random.Range(0.1f, 0.5f);
        float newAnimSpeed = Mathf.Round(randomAnimSpeed * 10) / 10;
        m_animator.speed = newAnimSpeed;
        m_animSpeed = newAnimSpeed;
        m_agent.speed = MoveSpeed;
        m_agent.baseOffset = MaxHeight;
        // Set a random attack rate to add variety to multiple wasps
        m_attackRate = Random.Range(2,4);
        InvokeRepeating("StartAttack", m_attackRate, m_attackRate);

    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistance();
        MoveToTarget();
        Descending();
        Ascending();
    }
    private void CalculateDistance()
    {
        m_distanceToPlayer = Vector3.Distance(transform.position, m_target.position);
    }
    private void MoveToTarget()
    {
        m_agent.destination = m_target.position;

    }
    private void Descending()
    {
        if (m_distanceToPlayer < AttackDistance + m_agent.baseOffset)
        {
            if (m_isDescending)
            {
                if (m_agent.baseOffset > MinHeight)
                {
                    m_agent.baseOffset -= DescentSpeed * Time.deltaTime;
                }
                if (m_agent.baseOffset <= MinHeight)
                {
                    m_animator.SetBool("IsAttacking", true);
                    m_animator.speed = 1;
                    m_isDescending = false;
                }
            }
        }
    }
    private void Ascending()
    {
        if (m_isAscending)
        {
            if (m_agent.baseOffset < MaxHeight)
            {
                m_animator.SetBool("IsAttacking", false);
                m_animator.speed = m_animSpeed;
                m_agent.baseOffset += AscentSpeed * Time.deltaTime;
            }
            if (m_agent.baseOffset >= MaxHeight)
            {
                m_isAscending = false;
            }
        }
    }

    public void ApplyDamageToPlayer()
    {
        
        if (m_distanceToPlayer <= AttackDistance + m_agent.baseOffset + 0.5f)
        {
            // Connect player Health
            PlayerHealth playerHealth = m_target.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Apply Damage
                playerHealth.TakeDamage(AttackDamage);
                Debug.Log($"Wasp damaged {AttackDamage} ");
            }
        }
    }

    // Set by Invoke Repeating and allows the wasp to attack at set intervals
    private void StartAttack ()
    {
        if (!m_isAscending)
        {
            m_isDescending = true;
        }
    }

    // Call this function from the attack animation
    public void FlyUp()
    {
        m_isAscending = true;
    }

}
