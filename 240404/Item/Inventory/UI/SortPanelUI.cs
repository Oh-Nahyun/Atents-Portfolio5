using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SortPanelUI : MonoBehaviour
{
    public Action<ItemSortBy> onSortRequest;

    private void Awake()
    {
        TMP_Dropdown dropDown = GetComponentInChildren<TMP_Dropdown>();
        Button run = GetComponentInChildren<Button>();

        run.onClick.AddListener(() =>
        {
            onSortRequest?.Invoke((ItemSortBy)dropDown.value);
        });
    }
}
