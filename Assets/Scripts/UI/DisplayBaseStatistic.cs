using TMPro;
using UnityEngine;

public class DisplayBaseStatistic : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _resourcesAmountInBae;

    private void OnEnable()
    {
        _base.StorageChanged += Display;
    }

    private void OnDisable()
    {
        _base.StorageChanged -= Display;
    }

    private void Display(int amount)
    {
        _resourcesAmountInBae.text = amount.ToString();
    }
}