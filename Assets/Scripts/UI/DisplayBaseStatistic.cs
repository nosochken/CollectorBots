using TMPro;
using UnityEngine;

public class DisplayBaseStatistic : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _resourcesAmountInBae;

    private void OnEnable()
    {
        _base.WarehouseReplenished += Display;
    }

    private void OnDisable()
    {
        _base.WarehouseReplenished -= Display;
    }

    private void Display(int amount)
    {
        _resourcesAmountInBae.text = amount.ToString();
    }
}