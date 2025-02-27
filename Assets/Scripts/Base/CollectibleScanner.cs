using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableScanner : MonoBehaviour
{
    [SerializeField] private LayerMask _collectableLayerMask;
    [SerializeField, Min(1)] private float _scanRadius = 20f;
    [SerializeField, Min(1)] private float _scanDelay;

    private List<ICollectable> _discoveredCollectable;

    public event Action<IEnumerable> Scanned;

    private void Awake()
    {
        _discoveredCollectable = new List<ICollectable>();
    }

    public IEnumerator ScanSpace()
    {
        WaitForSeconds wait = new WaitForSeconds(_scanDelay);

        while (isActiveAndEnabled)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _scanRadius, _collectableLayerMask);
            Discover(colliders);

            yield return wait;
        }
    }

    private void Discover(Collider[] colliders)
    {
        _discoveredCollectable.Clear();

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out ICollectable collectable))
                _discoveredCollectable.Add(collectable);
        }

        Scanned?.Invoke(_discoveredCollectable);
    }
}