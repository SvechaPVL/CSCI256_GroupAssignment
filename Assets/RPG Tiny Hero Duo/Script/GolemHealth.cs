using UnityEngine;

public class GolemHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage! HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 피격 리액션이 있다면
            animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("Die", true);
        GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        GetComponent<Collider>().enabled = false; // 충돌 비활성화
        Destroy(gameObject, 3f); // 3초 후 삭제
    }
}
