using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // 따라갈 캐릭터
    public float distance = 4.0f;
    public float height = 2.0f;
    public float rotationSpeed = 3.0f;

    private float yaw;
    private float pitch;

    void Start()
    {
        if (target == null)
            Debug.LogWarning("Camera target not set!");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 마우스 입력으로 카메라 회전
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        // 카메라 위치 계산
        Vector3 offset = new Vector3(0, height, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // 캐릭터 상체 쪽을 바라보게

    }
}
