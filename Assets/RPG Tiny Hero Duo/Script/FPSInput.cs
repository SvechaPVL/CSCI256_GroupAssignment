using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float gravity = -9.8f;

    private CharacterController charController;
    private Animator anim;
    private Transform camTransform; // 메인 카메라 방향 참고용

    void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        camTransform = Camera.main.transform; // 카메라의 방향 참조
    }

    void Update()
    {
        // --- 입력 감지 ---
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(inputX, 0, inputZ).normalized;

        bool isMoving = inputDir.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        // --- 카메라 기준 이동 방향 ---
        Vector3 moveDir = Vector3.zero;
        if (isMoving)
        {
            // 카메라가 바라보는 방향 기준으로 이동 방향 계산
            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            moveDir = (camForward * inputZ + camRight * inputX).normalized;

            // 캐릭터가 이동 방향을 바라보게 회전
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDir),
                Time.deltaTime * 10f
            );
        }

        // --- 속도 결정 ---
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        // --- 실제 이동 ---
        Vector3 movement = moveDir * targetSpeed;
        movement.y = gravity;
        charController.Move(movement * Time.deltaTime);

        // --- 애니메이션 업데이트 ---
        if (anim != null)
        {
            anim.SetBool("isRunning", isRunning);
            anim.SetFloat("Speed", isMoving ? targetSpeed : 0);
        }
    }
}
