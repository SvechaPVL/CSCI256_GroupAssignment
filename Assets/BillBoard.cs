using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Face camera
        transform.rotation = Quaternion.LookRotation(
            transform.position - cam.transform.position
        );
    }
}
