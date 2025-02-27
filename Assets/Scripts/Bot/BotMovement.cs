using System;
using System.Collections;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;

    public IEnumerator GoTo(Vector3 targetPosition, Action onComplete = null)
    {
        float tolerance = 0.1f;
        targetPosition.y += transform.position.y;

        while ((transform.position - targetPosition).sqrMagnitude >= tolerance * tolerance)
        {
            Vector3 direction = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

            transform.LookAt(direction);
            transform.position = Vector3.MoveTowards(
                transform.position, direction, _movementSpeed * Time.deltaTime);

            yield return null;
        }

        onComplete?.Invoke();
    }
}