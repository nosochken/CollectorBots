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

        Bot newBot = Instantiate(_botPrefab);
        newBot.Init(PlaceOfRest.position);

        return newBot;
    }

    public IEnumerator WaitResourcesForNewBot()
    {
        yield return new WaitUntil(() => _storage.AmountOfCollectable >= _resourcesAmountToCreateBot);
    }

    public IEnumerator WaitResourcesForNewBase(Action onComplete)
    {
        yield return new WaitUntil(() => _storage.AmountOfCollectable >= _resourcesAmountToCreateBase);

        onComplete?.Invoke();
    }
}