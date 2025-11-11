using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    // 사망 애니메이션 클립의 길이 (몇 초 후에 파괴할지)
    public float destroyDelay = 1.2f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Death Settings")]
    public GameObject deathEffectPrefab; // 

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); // Animator 컴포넌트를 가져옵니다.

    }

    // 플레이어의 공격 판정에서 호출되는 함수
    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return; // 이미 죽었으면 처리하지 않음

        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} 이/가 {damageAmount} 피해를 입음. 남은 체력: {currentHealth}");

        // 체력 UI 업데이트 등 다른 로직을 여기에 추가할 수 있습니다.

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        // 1. 사망 애니메이션/이펙트 재생
        if (deathEffectPrefab != null)
        {
            animator.SetBool("IsDead", true);
        }

        // 2. 콜라이더/리지드바디 비활성화 (죽었으니 더 이상 충돌 및 이동하지 않도록)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // CharacterController 사용 시
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 3. destroyDelay 시간 후에 오브젝트를 파괴
        Debug.Log($"{gameObject.name} 이/가 사망 애니메이션 재생 후 파괴 대기 중...");
        Destroy(gameObject, destroyDelay);
    }
}