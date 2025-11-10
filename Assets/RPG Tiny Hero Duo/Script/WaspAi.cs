using UnityEngine;
using UnityEngine.AI;

public class WaspAi : MonoBehaviour
{
    // update
    private NavMeshAgent m_agent;
    private Animator m_animator;
    private Transform m_target;

    // Override the NavMesh agent's speed
    public float MoveSpeed;
    // Set the base offset height
    public float FlyingHeight;

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
        m_agent.speed = MoveSpeed;
        m_agent.baseOffset = FlyingHeight;
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        m_agent.destination = m_target.position;

    }
}
