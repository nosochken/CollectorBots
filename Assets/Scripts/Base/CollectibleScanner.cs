using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectableScanner : MonoBehaviour
{
    [SerializeField] private LayerMask _collectableLayerMask;
    [SerializeField, Min(1)] private float _scanRadius = 25f;
    [SerializeField, Min(1)] private float _scanDelay;

    private List<ICollectable> _discoveredCollectable;
    private Collider[] _colliders = new Collider[100];

    public event Action<IEnumerable<ICollectable>> Scanned;

    private void Awake()
    {
        _discoveredCollectable = new List<ICollectable>();
    }

    public IEnumerator ScanSpace()
    {
        WaitForSeconds wait = new WaitForSeconds(_scanDelay);

        while (isActiveAndEnabled)
        {
            int size = Physics.OverlapSphereNonAlloc(transform.position, _scanRadius, _colliders, _collectableLayerMask);
            Discover(_colliders.Take(size));

            yield return wait;
        }
    }

    private void Discover(IEnumerable<Collider> colliders)
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