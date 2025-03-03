using System.Collections.Generic;
using UnityEngine;

public class DeliveryDepartment : MonoBehaviour
{
    private List<ICollectable> _collectablesInDelivery;
    private Queue<ICollectable> _uncollectedСollectables;

    private void Awake()
    {
        _collectablesInDelivery = new List<ICollectable>();
        _uncollectedСollectables = new Queue<ICollectable>();
    }

    public int Sort(IEnumerable<ICollectable> collectables)
    {
        foreach (ICollectable collectable in collectables)
        {
            if (!_collectablesInDelivery.Contains(collectable) && !_uncollectedСollectables.Contains(collectable))
                _uncollectedСollectables.Enqueue(collectable);
        }
        return _uncollectedСollectables.Count;
    }

    public void DeliverTo(Vector3 deliveryPosition, Bot bot)
    {
        ICollectable currentUncollectedCollectible = _uncollectedСollectables.Dequeue();
        _collectablesInDelivery.Add(currentUncollectedCollectible);
        
        Debug.Log($"просто лежат {_uncollectedСollectables.Count}");
        Debug.Log($"в доставке {_collectablesInDelivery.Count}");
        Debug.Log($"{bot.name} доставляет {currentUncollectedCollectible.Name}");
        
        bot.DeliverTo(deliveryPosition, currentUncollectedCollectible.Position);
    }

    public void RemoveFromDeliveryList(ICollectable collectable)
    { 
        _collectablesInDelivery.Remove(collectable);
    }
}