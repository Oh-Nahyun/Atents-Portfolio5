using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Inventory2 : TestBase
{
    public ItemCode code = ItemCode.Ruby;

    [Range(0, 5)]
    public uint index = 0;

    public ItemSortBy sortBy = ItemSortBy.Code;
    public bool isAcending = true;

    Inventory inven;

#if UNITY_EDITOR
    private void Start()
    {
        inven = new Inventory(null);
        inven.AddItem(ItemCode.Ruby);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Emerald);
        inven.AddItem(ItemCode.Emerald);
        inven.AddItem(ItemCode.Emerald);
        inven.MoveItem(2, 3);
        inven.Test_InventoryPrint();
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // 이동
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // 나누기
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // 정렬
        inven.SlotSorting(sortBy, isAcending);
        inven.Test_InventoryPrint();
    }
#endif
}
