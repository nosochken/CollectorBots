using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryDepartment : MonoBehaviour
{
    private List<ICollectable> _collectablesInDelivery;
    private Queue<ICollectable> _uncollectedСollectables;

    public event Action UncollectedCollectableFound;

    private void Awake()
    {
        _collectablesInDelivery = new List<ICollectable>();
        _uncollectedСollectables = new Queue<ICollectable>();
    }

    public void DistributeResources(IEnumerable collectables)
    {
        foreach (ICollectable collectable in collectables)
        {
            if (!_collectablesInDelivery.Contains(collectable) && !_uncollectedСollectables.Contains(collectable))
            {
                _uncollectedСollectables.Enqueue(collectable);
                UncollectedCollectableFound?.Invoke();
            }
        }
    }

    public void DeliverTo(Vector3 deliveryPosition, Bot bot)
    {
        ICollectable currentUncollectedCollectible = _uncollectedСollectables.Dequeue();
        _collectablesInDelivery.Add(currentUncollectedCollectible);

        bot.DeliverTo(deliveryPosition, currentUncollectedCollectible.Position);
    }

    public void RemoveFromDeliveryList(ICollectable collectable)
    {
        _collectablesInDelivery.Remove(collectable);
    }
}