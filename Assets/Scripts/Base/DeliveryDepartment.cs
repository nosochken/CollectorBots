using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryDepartment : MonoBehaviour
{
    private Dictionary<Bot, ICollectable> _collectablesInDelivery;
    private Queue<ICollectable> _uncollectedСollectables;

    private void Awake()
    {
        _collectablesInDelivery = new Dictionary<Bot, ICollectable>();
        _uncollectedСollectables = new Queue<ICollectable>();
    }

    public int Sort(IEnumerable<ICollectable> collectables)
    {
        UpdateData(collectables);

        foreach (ICollectable collectable in collectables)
        {
            if (!_collectablesInDelivery.Values.Any(value => value == collectable)
                && !_uncollectedСollectables.Contains(collectable))
                _uncollectedСollectables.Enqueue(collectable);
        }
        return _uncollectedСollectables.Count;
    }

    public void DeliverTo(Vector3 deliveryPosition, Bot bot)
    {
        ICollectable currentUncollectedCollectible = _uncollectedСollectables.Dequeue();
        _collectablesInDelivery.Add(bot, currentUncollectedCollectible);

        bot.DeliverTo(deliveryPosition, currentUncollectedCollectible.Position);
    }

    public void RemoveFromDeliveryList(Bot bot)
    {
        _collectablesInDelivery.Remove(bot);
    }

    private void UpdateData(IEnumerable<ICollectable> collectables)
    {
        HashSet<ICollectable> collectablesSet = new HashSet<ICollectable>(collectables);

        _uncollectedСollectables = new Queue<ICollectable>(
            _uncollectedСollectables.Where(item => collectablesSet.Contains(item)));
    }
}