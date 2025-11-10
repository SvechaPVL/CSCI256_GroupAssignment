using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement_MaleWarrior : MonoBehaviour
{
    // --- 이동 설정 ---
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSmoothTime = 0.1f;
    public float gravity = -9.8f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;

    // --- 공격 설정 및 상태 변수 ---
    [Header("Attack Settings")]
    public float comboDelay = 0.5f;             // 콤보 유효 시간 (이 시간 내에 다음 공격 입력)
    public float spinAttackCooldown = 10.0f;    // Q 공격 쿨타임
    public float attackRadius = 1.0f;           // 공격 판정 반경 (애니메이션 이벤트 사용)
    public int normalDamage = 10;               // 일반 공격 데미지 (애니메이션 이벤트로 전달)
    public int spinDamage = 30;                 // Q 공격 데미지
    public LayerMask enemyLayer;                // 적 오브젝트 Layer

    // ⭐ 추가: 공격 상태 플래그
    private bool isAttacking = false;

    private int comboIndex = 0;                 // 현재 콤보 단계 (0: 대기, 1~3: 공격)
    private float lastAttackTime;               // 마지막 공격 시간 기록
    private float nextSpinAttackTime = 0f;      // 다음 Q 공격 가능 시간

    float turnSmoothVelocity;
    Vector3 velocity; // 중력 적용을 위한 속도 벡터

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // 중력 초기화
        velocity.y = 0;
    }

    void Update()
    {
    
        // if (isAttacking) return; 

        // 1. 이동 입력 처리
        HandleMovementInput();

        // 2. 공격 입력 처리
        HandleNormalAttackInput();
        HandleSpinAttackInput();

        // 3. 콤보 리셋 체크
        CheckComboReset();

        // 4. 중력 적용
        ApplyGravity();
    }

    // --- 이동 및 중력 처리 메서드 ---
    // (HandleMovementInput, ApplyGravity 메서드는 수정 없음)

    void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        bool hasInput = direction.magnitude >= 0.1f;

        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = hasInput && isShiftPressed;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        float animatorSpeed = isRunning ? 1f : (hasInput ? 0.5f : 0f);

        // Animator parameter 세팅 (이동)
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isMoving", hasInput);
        animator.SetFloat("Speed", animatorSpeed);

       
        if (isAttacking && comboIndex > 0)
        {
            currentSpeed = 0f;
        }

        if (hasInput)
        {
            // 카메라 방향 기준 이동 방향 계산
            Vector3 moveDir = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * direction;

            // 캐릭터 이동
            controller.Move(moveDir * currentSpeed * Time.deltaTime);

            // 캐릭터 회전
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime / Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 바닥에 닿았을 때 미세하게 아래로 눌러주는 힘
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }


    // --- 공격 처리 메서드 ---

    void HandleNormalAttackInput()
    {
        // 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
           
            if (isAttacking) return;

            // 콤보 유효 시간 내에 재입력되었는지 확인
            if (Time.time < lastAttackTime + comboDelay && comboIndex > 0)
            {
                // 콤보 유효 시간 내에 입력 -> 다음 콤보로
                comboIndex++;
                if (comboIndex > 3) comboIndex = 1; // 3콤보를 초과하면 다시 1로 루프
            }
            else
            {
                // 콤보 유효 시간이 지났거나 첫 공격 -> 콤보를 1로 시작
                comboIndex = 1;
            }

            isAttacking = true;

            // Animator 파라미터 업데이트 (ComboIndex 1, 2, 3 중 하나로 설정)
            animator.SetInteger("ComboIndex", comboIndex);
            lastAttackTime = Time.time; // 마지막 공격 시간 갱신
        }
    }

    void CheckComboReset()
    {
        // 현재 콤보 중이고 & 마지막 공격 후 콤보 딜레이 시간이 지나면 콤보 리셋
        if (comboIndex > 0 && Time.time > lastAttackTime + comboDelay)
        {
            comboIndex = 0;
            animator.SetInteger("ComboIndex", comboIndex);

           
            isAttacking = false;
        }
    }

    void HandleSpinAttackInput()
    {
        // Q 키 입력 확인
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            if (isAttacking)
            {
                Debug.Log("Q 스킬: 다른 공격 중이므로 발동 불가.");
                return;
            }

            // 쿨타임 체크
            if (Time.time >= nextSpinAttackTime)
            {
                // 쿨타임이 지났으므로 공격 가능
                nextSpinAttackTime = Time.time + spinAttackCooldown; // 다음 공격 가능 시간 설정

                
                isAttacking = true;

                // 콤보 초기화 (Q 공격 중에는 일반 콤보 방지)
                comboIndex = 0;
                animator.SetInteger("ComboIndex", 0);

                // Attack4 애니메이션 실행
                animator.SetTrigger("SpinAttack");

                Debug.Log($"Q 공격 발동! 다음 사용 가능 시간: {nextSpinAttackTime - Time.time:F2}초 남음.");

                // Q 공격은 보통 즉시 판정이 나가므로 여기서 즉시 판정 로직을 호출합니다.
                ApplySpinAttackDamage();

                
                // 실제 애니메이션 길이에 맞춰 숫자를 조정하거나, 애니메이션 이벤트를 사용해야 합니다.
                Invoke("ResetAttackFlag", 1.0f);
            }
            else
            {
                // 쿨타임이 남아있음
                Debug.Log($"Q 공격 쿨타임이 아직 {nextSpinAttackTime - Time.time:F2}초 남았습니다.");
            }
        }
    }

 
    // 일반 공격 및 Q 스킬 애니메이션의 끝 프레임에 이 이벤트를 연결해야 합니다.
    public void ResetAttackFlag()
    {
        isAttacking = false;
        Debug.Log("공격 플래그 해제 완료.");
    }


    // --- 공격 판정 메서드 (수정 없음) ---

    // 일반 콤보 애니메이션 이벤트에서 호출될 함수 (Attack1, 2, 3) ⭐
    // 매개변수 damage는 애니메이션 이벤트 설정 시 전달한 값입니다.
    public void ApplyDamageToEnemies(int damage)
    {
        PerformHitCheck(damage, attackRadius, transform.forward * 0.5f);
    }

    // Q 공격용 즉시 판정 함수 (더 넓은 범위) ⭐
    void ApplySpinAttackDamage()
    {
        // Q 공격은 범위가 넓으므로, 공격 반경을 1.5배로 늘리고 오프셋은 0으로 설정
        PerformHitCheck(spinDamage, attackRadius * 1.5f, Vector3.zero);
    }

    // 실제 공격 판정 로직 (코드 중복 방지)
    private void PerformHitCheck(int damage, float radius, Vector3 offset)
    {
        Debug.Log($"<color=yellow>공격 판정 시도: Radius={radius}, Damage={damage}</color>");

        // Physics.OverlapSphere를 사용하여 범위 내의 적 감지
        Collider[] hitEnemies = Physics.OverlapSphere(
            transform.position + offset, // 플레이어 위치 + 오프셋
            radius, // 판정 반경
            enemyLayer // 적 Layer만 감지
        );

        Debug.Log($"<color=cyan>감지된 적 개수: {hitEnemies.Length}</color>");

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log($"<color=red>SUCCESS: {enemy.name} 에게 {damage} 피해 적용!</color>");
                enemyHealth.TakeDamage(damage);
            }
        }
    }

}