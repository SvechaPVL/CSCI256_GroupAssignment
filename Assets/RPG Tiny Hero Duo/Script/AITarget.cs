using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AITarget : MonoBehaviour
{
    public Transform Target;
    public float AttackDistance = 2f;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Target == null) return;

        float distance = Vector3.Distance(transform.position, Target.position);

        if (distance <= AttackDistance)
        {
            m_Agent.isStopped = true;
            m_Animator.SetBool("Attack", true);
 
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.SetDestination(Target.position);

            m_Animator.SetBool("Attack", false);

        }
    }
}
