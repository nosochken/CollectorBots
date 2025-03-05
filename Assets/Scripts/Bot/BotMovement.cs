using System.Collections;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;

    public IEnumerator GoTo(Vector3 targetPosition)
    {
        float tolerance = 0.1f;
        Vector3 qdjustedTargetPosition = Vector3.zero;

        while ((transform.position - qdjustedTargetPosition).sqrMagnitude >= tolerance * tolerance)
        {
            qdjustedTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

            transform.LookAt(qdjustedTargetPosition);
            transform.position = Vector3.MoveTowards(
                transform.position, qdjustedTargetPosition, _movementSpeed * Time.deltaTime);

            yield return null;
        }
    }
}