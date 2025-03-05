using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollectableScanner), typeof(DeliveryDepartment), typeof(Creator))]
public class Base : MonoBehaviour
{
    [SerializeField] private List<Bot> _bots;

    [SerializeField] private CollectableScanner _collectableScanner;
    [SerializeField] private DeliveryDepartment _deliveryDepartment;
    [SerializeField] private Storage _storage;
    [SerializeField] private Creator _creator;

    private Coroutine _botCreationCoroutine;

    private int _botsAmountToCreateBase = 2;

    public event Action<int> StorageChanged;
    public event Action<int> BotsAmountChanged;

    public event Action WasChosen;
    public event Action NewBaseCreated;

    public bool CanCreateNewBase => _bots.Count >= _botsAmountToCreateBase;

    private void OnEnable()
    {
        _collectableScanner.Scanned += Distribute;
        _storage.Changed += OnStorageChanged;
    }

    private void Start()
    {
        _creator.Init(_storage);

        foreach (Bot bot in _bots)
            Prepare(bot);

        BotsAmountChanged?.Invoke(_bots.Count);

        StartCoroutine(_collectableScanner.ScanSpace());
        _botCreationCoroutine = StartCoroutine(CreateBots());
    }

    private void OnDisable()
    {
        _collectableScanner.Scanned -= Distribute;
        _storage.Changed -= OnStorageChanged;

        foreach (Bot bot in _bots)
            StopMonitor(bot);
    }

    public void Init(Bot bot)
    {
        Prepare(bot);
        _bots.Add(bot);

        BotsAmountChanged?.Invoke(_bots.Count);
    }

    public void CreateNewBase(Flag flag)
    {
        StopCoroutine(_botCreationCoroutine);

        StartCoroutine(_creator.WaitResourcesForNewBase(() =>
        {
            Bot builderBot = GetAvailableBot();

            if (builderBot == null)
            {
                int firstBot = 0;
                _bots[firstBot].SuspendWork();
                builderBot = _bots[firstBot];
            }

            _creator.CreateNewBase(builderBot, flag.transform.position);
        }));
    }

    public void OnWasChosen()
    {
        WasChosen?.Invoke();
    }

    private void Distribute(IEnumerable<ICollectable> collectables)
    {
        int freeCollectables = _deliveryDepartment.Sort(collectables);

        for (int i = 0; i < freeCollectables; i++)
            SetBotForDelivery();
    }

    private IEnumerator CreateBots()
    {
        while (isActiveAndEnabled)
        {
            yield return _creator.WaitResourcesForNewBot();

            Bot newBot = _creator.CreateNewBot();
            StartMonitor(newBot);
            _bots.Add(newBot);

            BotsAmountChanged?.Invoke(_bots.Count);
        }
    }

    private Bot GetAvailableBot()
    {
        foreach (Bot bot in _bots)
        {
            if (bot.IsWorking == false)
                return bot;
        }

        return null;
    }

    private void SetBotForDelivery()
    {
        Bot bot = GetAvailableBot();

        if (bot != null)
            _deliveryDepartment.DeliverTo(_storage.transform.position, bot);
    }

    private void Prepare(Bot bot)
    {
        bot.Init(_creator.PlaceOfRest.position);
        StartMonitor(bot);
    }

    private void StartMonitor(Bot bot)
    {
        bot.DeliveredBy += _deliveryDepartment.RemoveFromDeliveryList;
        bot.NotDeliveredBy += _deliveryDepartment.RemoveFromDeliveryList;
        bot.BaseCreatedBy += OnBaseCreated;
    }

    private void StopMonitor(Bot bot)
    {
        bot.DeliveredBy -= _deliveryDepartment.RemoveFromDeliveryList;
        bot.NotDeliveredBy -= _deliveryDepartment.RemoveFromDeliveryList;
        bot.BaseCreatedBy -= OnBaseCreated;
    }

    private void OnStorageChanged()
    {
        StorageChanged?.Invoke(_storage.AmountOfCollectable);
    }

    private void OnBaseCreated(Bot bot)
    {
        NewBaseCreated?.Invoke();

        StopMonitor(bot);
        _bots.Remove(bot);
        BotsAmountChanged?.Invoke(_bots.Count);

        _botCreationCoroutine = StartCoroutine(CreateBots());
    }
}