using UnityEngine;
using System.Collections;

public class TCaveController : MonoBehaviour
{
    [Header("‰ñ“]ŠÖ˜A")]
    public Transform point;          // ‰ñ“]’†S
    public float turnAngle = 90f;
    public float turnDuration = 1.5f;

    private bool isTurning = false;
    private bool turnRight = true;
    private bool triggered = false;

    public bool IsTurning() => isTurning;

    public void SetTurnDirection(bool isRight)
    {
        turnRight = isRight;
    }

    // Õ“Ë‚É‰ñ“]ŠJn
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TriggerEnter: {other.name}, tag={other.tag}");

        if (triggered) return;
        if (!other.CompareTag("Tram")) return;

        triggered = true;
        StartCoroutine(TurnRoutine());
    }


    private IEnumerator TurnRoutine()
    {
        isTurning = true;
        Debug.Log("[TCaveController] Turn Start");

        float elapsed = 0f;
        float totalAngle = turnRight ? turnAngle : -turnAngle;

        while (elapsed < turnDuration)
        {
            float step = (totalAngle / turnDuration) * Time.deltaTime;
            transform.RotateAround(point.position, Vector3.up, step);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isTurning = false;
        Debug.Log("[TCaveController] Turn Finished");
    }


}
