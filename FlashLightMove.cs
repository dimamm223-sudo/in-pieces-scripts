using UnityEngine;

public class FlashLightMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float positionSpeed = 12f;
    [SerializeField] private Vector3 localOffset = new Vector3(0.2f, -0.2f, 0.5f);
    [Range(0f, 180f)]
    [SerializeField] private float maxAngleOffset = 180f;

    private void Awake()
    {
        if (target == null && Camera.main != null)
            target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(localOffset);
        Quaternion desiredRotation = target.rotation;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * positionSpeed
        );

        Quaternion smoothed = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            Time.deltaTime * rotationSpeed
        );

        if (maxAngleOffset < 180f)
        {
            float angle = Quaternion.Angle(desiredRotation, smoothed);
            if (angle > maxAngleOffset)
            {
                smoothed = Quaternion.RotateTowards(
                    desiredRotation,
                    smoothed,
                    maxAngleOffset
                );
            }
        }

        transform.rotation = smoothed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}