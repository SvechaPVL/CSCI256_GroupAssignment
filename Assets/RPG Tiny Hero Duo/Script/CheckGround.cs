using UnityEngine;
using Unity.AI.Navigation;

public class CheckGround : MonoBehaviour
{
    private RaycastHit m_hit;
    public NavMeshLink[] LinksUpArray;
    public NavMeshLink[] LinksDownArray;

    void FixedUpdate()
    {
        // Skip if arrays are not configured
        if (LinksUpArray == null || LinksDownArray == null || LinksUpArray.Length == 0)
            return;

        if (Physics.Raycast(transform.position, -Vector3.up, out m_hit))
        {
            if (m_hit.collider.CompareTag("Ground"))
            {
                for (int i = 0; i < LinksUpArray.Length; i++)
                {
                    if (LinksUpArray[i] != null && LinksDownArray[i] != null)
                    {
                        LinksUpArray[i].activated = false;
                        LinksDownArray[i].activated = true;
                    }
                }
            }
            if (m_hit.collider.CompareTag("Obstacle"))
            {
                for (int i = 0; i < LinksUpArray.Length; i++)
                {
                    if (LinksUpArray[i] != null && LinksDownArray[i] != null)
                    {
                        LinksUpArray[i].activated = true;
                        LinksDownArray[i].activated = false;
                    }
                }
            }
        }
    }
}

