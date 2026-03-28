using UnityEngine;

public class CameraFollowLeader : MonoBehaviour
{
    [Header("Target")]
    public Transform leader;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, 8f, -12f);
    public float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (leader == null)
            return;

        Vector3 targetPosition = leader.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
