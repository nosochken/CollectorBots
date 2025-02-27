using System;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] private float _holdDistance;

    private ICollectable _currentTarget;
    private Vector3 _currentTargetPosition;

    private bool _didReachTarget;

    public event Action<ICollectable> CollectibleDelivered;

    public void SetTargetPosition(Vector3 collectablePosition)
    {
        _currentTargetPosition = collectablePosition;
    }

    public bool TryCollectTarget(Collision collision)
    {
        if (!_didReachTarget)
        {
            if (collision.collider.TryGetComponent(out ICollectable collectable))
            {
                if (collectable.Position == _currentTargetPosition)
                {
                    _didReachTarget = true;
                    _currentTarget = collectable;
                    PickUp(_currentTarget);

                    return true;
                }
            }
        }
        return false;
    }

    public void PutInRightPlace()
    {
        _currentTarget.BePlaced();
        CollectibleDelivered?.Invoke(_currentTarget);

        _currentTarget = null;
        _didReachTarget = false;
    }

    private void PickUp(ICollectable collectable)
    {
        collectable.BePickUp(transform, _holdDistance);
    }
}