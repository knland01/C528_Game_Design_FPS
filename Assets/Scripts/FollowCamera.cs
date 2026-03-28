using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    Transform _target;


    [SerializeField] float distance = 10;
    [SerializeField] float height = 1;

    [SerializeField] float followSpeed = 5;


    private void LateUpdate()
    {
        float targetscale = _target.localScale.magnitude; // Assuming uniform scaling, use x as reference
        Vector3 targetPosition = _target.position; // Start from target's position
        targetPosition += -_target.forward * distance * targetscale; // Move distance units behind the target
        targetPosition += _target.up * height * targetscale; // Move height units above the target

        // Smoothly move to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);


        Vector3 lookPosition = _target.position; // Start from target's position
        lookPosition += _target.forward * 10 * targetscale; // Look ahead of the target

        transform.LookAt(lookPosition); // Look at the target's forward position
    }
}
