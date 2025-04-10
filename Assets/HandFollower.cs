using UnityEngine;

public class HandFollower : MonoBehaviour
{
    public AstraInputController inputController;

    public float gameMinX = -8f;
    public float gameMaxX = 8f;
    public float gameMinY = -4.5f;
    public float gameMaxY = 4.5f;

    public float realMinX = -0.19f;
    public float realMaxX = 0.35f;
    public float realMinY = -0.14f;
    public float realMaxY = 0.2f;

    public float moveSpeed = 20f;

    private void Start()
    {
        transform.position = Vector3.zero;

        if (inputController != null)
        {
            inputController.onDetectBody += MoveWithHand;
        }
    }

    private void OnDestroy()
    {
        if (inputController != null)
        {
            inputController.onDetectBody -= MoveWithHand;
        }
    }

    private void MoveWithHand(bool isDetected, Vector3 handPosition)
    {
        if (!isDetected) return;

        float mappedX = MapValue(handPosition.x, realMinX, realMaxX, gameMinX, gameMaxX);
        float mappedY = MapValue(handPosition.y, realMinY, realMaxY, gameMinY, gameMaxY);

        Vector2 targetPosition = new Vector2(mappedX, mappedY);
        Vector2 smoothPosition = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        transform.position = new Vector3(smoothPosition.x, smoothPosition.y, transform.position.z);
    }

    private float MapValue(float value, float inMin, float inMax, float outMin, float outMax)
    {
        float t = Mathf.InverseLerp(inMin, inMax, value);
        return Mathf.Lerp(outMin, outMax, t);
    }
}
