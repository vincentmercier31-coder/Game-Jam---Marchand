using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0f, 15f, -10f);

    [Range(0.01f, 1f)]
    public float smoothTime = 0.2f;   // Plus petit = plus rapide

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );
    }
}
