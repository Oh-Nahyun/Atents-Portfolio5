using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// 이 UI가 보여줄 인벤토리
    /// </summary>
    Inventory inven;

    /// <summary>
    /// 인벤토리에 있는 slot UI들
    /// </summary>
    InvenSlotUI[] slotUIs;

    TempSlotUI tempSLotUI;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        slotUIs = child.GetComponentsInChildren<InvenSlotUI>();

        tempSLotUI = GetComponentInChildren<TempSlotUI>();
    }

    /// <summary>
    /// 인벤토리 초기화용 함수
    /// </summary>
    /// <param name="playerInventory">이 UI가 표시할 인벤토리</param>
    public void InitializeInventory(Inventory playerInventory)
    {
        inven = playerInventory; // 저장

        for (uint i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i].InitializeSlot(inven[i]); // 모든 슬롯 초기화
            slotUIs[i].onDragBegin += OnItemMoveBegin;
            slotUIs[i].onDragEnd += OnItemMoveEnd;
            slotUIs[i].onClick += OnSlotClick;
        }

        tempSLotUI.InitializeSlot(inven.TempSlot);
    }

    private void OnItemMoveBegin(uint index)
    {
        inven.MoveItem(index, tempSLotUI.Index); // 시작 -> 임시로 아이템 옮기기
        tempSLotUI.Open(); // 임시 슬롯 열기
    }

    private void OnItemMoveEnd(uint index, bool isSlotEnd)
    {
        //uint finalIndex = index;
        //if (!isSuccess)
        //{
        //    // 빈 곳을 찾아서 따로 준다.
        //    if (inven.FindEmptySlot(out uint emptySlotIndex))
        //    {
        //        finalIndex = emptySlotIndex;
        //    }
        //    else
        //    {
        //        // 바닥에 드랍
        //        Debug.LogWarning("바닥에 아이템을 드랍해야 한다.");
        //        return;
        //    }
        //}

        //inven.MoveItem(tempSLotUI.Index, finalIndex);
        inven.MoveItem(tempSLotUI.Index, index); // 임시 -> 도착으로 아이템 옮기기

        if (tempSLotUI.InvenSlot.IsEmpty)
        {
            tempSLotUI.Close(); // 임시 슬롯이 비면 닫는다.
        }
    }

    private void OnSlotClick(uint index)
    {
        if (!tempSLotUI.InvenSlot.IsEmpty)
        {
            OnItemMoveEnd(index, true); // 슬롯이 클릭되었을 때 실행되기 때문에 isSlotEnd는 true
        }
    }
}
