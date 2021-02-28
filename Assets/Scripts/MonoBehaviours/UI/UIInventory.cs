using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    // UIItem later
    [SerializeField] GameObject _item;

    CanvasGroup _canvasGroup;

    public void Hide() => _canvasGroup.alpha = 0f;
    public void Show() => _canvasGroup.alpha = 1f;

    void OnPlayerInventoryUpdated(Dictionary<PickupType, int> inventory)
    {
        // only one pickup type for now, fix dirty implementation later
        foreach (var pair in inventory)
        {
            _item.GetComponentInChildren<Image>().sprite = pair.Key.Sprite;

            if (pair.Value > 0)
            {
                Show();
                _item.GetComponentInChildren<TextMeshProUGUI>().SetText(pair.Value.ToString());
            }
            else
                Hide();
        }
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();
        EnvironmentEvents.PlayerInventoryUpdated += OnPlayerInventoryUpdated;
    }

    void OnDestroy()
    {
        EnvironmentEvents.PlayerInventoryUpdated -= OnPlayerInventoryUpdated;
    }
}
