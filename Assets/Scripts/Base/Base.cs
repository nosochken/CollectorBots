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
    private Warehouse _warehouse;

    public event Action<int> WarehouseReplenished;

    private void Awake()
    {
        _collectableScanner = GetComponent<CollectableScanner>();
        _deliveryDepartment = GetComponent<DeliveryDepartment>();
        _warehouse = GetComponentInChildren<Warehouse>();

        SetPlacesForBots();
    }

    private void OnEnable()
    {
        _collectableScanner.Scanned += _deliveryDepartment.DistributeResources;
        _deliveryDepartment.UncollectedCollectableFound += SetBot;
        _warehouse.Replenished += OnWarehouseReplenished;

        StartMonitorBots();
    }

    private void Start()
    {
        StartCoroutine(_collectableScanner.ScanSpace());
    }

    private void OnDisable()
    {
        _collectableScanner.Scanned -= _deliveryDepartment.DistributeResources;
        _deliveryDepartment.UncollectedCollectableFound -= SetBot;
        _warehouse.Replenished -= OnWarehouseReplenished;

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

    private void SetBot()
    {
        foreach (Bot bot in _bots)
        {
            if (bot.IsWorking == false)
            {
                _deliveryDepartment.DeliverTo(_warehouse.transform.position, bot);
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
        WarehouseReplenished?.Invoke(_warehouse.AmountOfCollectable);
    }
}