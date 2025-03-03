using System;
using System.Collections;
using UnityEngine;

public class Creator : MonoBehaviour
{
    [SerializeField] private Bot _botPrefab;
    [SerializeField] private Transform _placeOfRest;
    
    private Storage _storage;
    
    private int _resourcesAmountToCreateBot = 3;
    private int _resourcesAmountToCreateBase = 5;
    
    public Transform PlaceOfRest => _placeOfRest;

    public void Init(Storage storage)
    {
        _storage = storage;
    }
    
    public void CreateNewBase(Bot bot, Vector3 position)
    {
        _storage.UseResources(_resourcesAmountToCreateBase);
                       
        bot.CreateNewBase(position);
    }
    
    public Bot CreateNewBot()
    {
        _storage.UseResources(_resourcesAmountToCreateBot);
        
        return Instantiate(_botPrefab, PlaceOfRest.position, Quaternion.identity);
    }
    
    public IEnumerator WaitResourcesForNewBot(Action onComplete)
    {
        int requiredAmount = GetRequiredResourcesAmount(_resourcesAmountToCreateBot);
        yield return new WaitUntil (() => _storage.AmountOfCollectable >= requiredAmount);
        
        onComplete?.Invoke();
    }
    
    public IEnumerator WaitResourcesForNewBase(Action onComplete)
    {
        int requiredAmount = GetRequiredResourcesAmount(_resourcesAmountToCreateBase);
        yield return new WaitUntil (() => _storage.AmountOfCollectable >= requiredAmount);
        
        onComplete?.Invoke();
    }
    
    private int GetRequiredResourcesAmount(int resourcesAmountForCreate)
    {
        return _storage.AmountOfCollectable + resourcesAmountForCreate;
    }
}