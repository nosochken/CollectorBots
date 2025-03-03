using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollectableScanner), typeof(DeliveryDepartment), typeof(Creator))]
public class Base : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader; 
    [SerializeField] private List<Bot> _bots;

    private CollectableScanner _collectableScanner;
    private DeliveryDepartment _deliveryDepartment;
    private Storage _storage;
    private Creator _creator;
    
    private Coroutine _botCreationCoroutine;
    
    private int _botsAmountToCreateBase = 2;

    public event Action<int> StorageChanged;

    private void Awake()
    {
        _collectableScanner = GetComponent<CollectableScanner>();
        _deliveryDepartment = GetComponent<DeliveryDepartment>();
        _storage = GetComponentInChildren<Storage>();
        _creator = GetComponent<Creator>();
    }

    private void OnEnable()
    {
        _inputReader.FlagWasPutUp += CreateNewBase;

        _collectableScanner.Scanned += Distribute;
        _storage.Changed += OnStorageChanged;
    }

    private void Start()
    {
        _creator.Init(_storage);
        
        StartCoroutine(_collectableScanner.ScanSpace());
        
        foreach (Bot bot in _bots)
            bot.Init(_creator.PlaceOfRest.position);
            
        _botCreationCoroutine = StartCoroutine(CreateBots());
    }

    private void OnDisable()
    {
        _inputReader.FlagWasPutUp -= CreateNewBase;
        
        _collectableScanner.Scanned -= Distribute;
        _storage.Changed -= OnStorageChanged;

        foreach(Bot bot in _bots)
            StopMonitor(bot);
    }

    public void Init(Bot bot)
    {
        _bots.Add(bot);
        StartMonitor(bot);
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
            yield return StartCoroutine(_creator.WaitResourcesForNewBot(() => 
            {
                StartMonitor(_creator.CreateNewBot());
            }));
        }
    }
    
    private void CreateNewBase()
    {
        if (_bots.Count < _botsAmountToCreateBase)
            return;
            
        StartCoroutine(_creator.WaitResourcesForNewBase(() =>
        {
            Bot bot = null;
            
            while (bot == null)
                bot = GetAvailableBot();
                
            _creator.CreateNewBase(bot, _inputReader.FlagPosition);
            StopMonitor(bot);
            _bots.Remove(bot);
        }));
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

    private void StartMonitor(Bot bot)
    {
        bot.Delivered += _deliveryDepartment.RemoveFromDeliveryList;
        bot.BaseCreated += OnBaseCreated;
        
        _bots.Add(bot);
    }

    private void StopMonitor(Bot bot)
    {
        bot.Delivered -= _deliveryDepartment.RemoveFromDeliveryList;
        bot.BaseCreated -= OnBaseCreated;
    }

    private void OnStorageChanged()
    {
        StorageChanged?.Invoke(_storage.AmountOfCollectable);
    }
    
    private void OnBaseCreated(Bot bot)
    {
        StopMonitor(bot);
        _bots.Remove(bot);
    }
}