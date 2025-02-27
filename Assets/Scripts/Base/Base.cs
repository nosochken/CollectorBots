using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollectableScanner), typeof(DeliveryDepartment))]
public class Base : MonoBehaviour
{
    [SerializeField] private List<Bot> _bots;
    [SerializeField] private List<Transform> _placeOfRest;

    private CollectableScanner _collectableScanner;
    private DeliveryDepartment _deliveryDepartment;
    private Storage _storage;

    public event Action<int> WarehouseReplenished;

    private void Awake()
    {
        _collectableScanner = GetComponent<CollectableScanner>();
        _deliveryDepartment = GetComponent<DeliveryDepartment>();
        _storage = GetComponentInChildren<Storage>();

        SetPlacesForBots();
    }

    private void OnEnable()
    {
        _collectableScanner.Scanned += Distribute;
        _storage.Replenished += OnWarehouseReplenished;

        StartMonitorBots();
    }

    private void Start()
    {
        StartCoroutine(_collectableScanner.ScanSpace());
    }

    private void OnDisable()
    {
        _collectableScanner.Scanned -= Distribute;
        _storage.Replenished -= OnWarehouseReplenished;

        StopMonitorBots();
    }

    private void SetPlacesForBots()
    {
        if ((_bots.Count != 0) && (_placeOfRest.Count != 0))
        {
            for (int i = 0; i < _bots.Count; i++)
                _bots[i].Init(_placeOfRest[i].position);
        }
    }

    private void Distribute(IEnumerable<ICollectable> collectables)
    {
        int freeCollectables = _deliveryDepartment.Sort(collectables);

        for (int i = 0; i < freeCollectables; i++)
            SetBot();
    }

    private void SetBot()
    {
        foreach (Bot bot in _bots)
        {
            if (bot.IsWorking == false)
            {
                _deliveryDepartment.DeliverTo(_storage.transform.position, bot);
                return;
            }
        }
    }

    private void StartMonitorBots()
    {
        foreach (Bot bot in _bots)
            bot.Delivered += _deliveryDepartment.RemoveFromDeliveryList;
    }

    private void StopMonitorBots()
    {
        foreach (Bot bot in _bots)
            bot.Delivered -= _deliveryDepartment.RemoveFromDeliveryList;
    }

    private void OnWarehouseReplenished()
    {
        WarehouseReplenished?.Invoke(_storage.AmountOfCollectable);
    }
}