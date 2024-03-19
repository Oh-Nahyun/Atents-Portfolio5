using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_InventoryUI : TestBase
{
    public InventoryUI inventoryUI;
    Inventory inven;

    public ItemCode code = ItemCode.Ruby;

    [Range(0, 5)]
    public uint fromeIndex = 0;

    [Range(0, 5)]
    public uint toIndex = 0;

    public ItemSortBy sortBy = ItemSortBy.Code;
    public bool isAcending = true;

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
        inven.AddItem(ItemCode.Sapphire, 2);
        inven.AddItem(ItemCode.Sapphire, 4);
        inven.AddItem(ItemCode.Sapphire, 4);
        inven.AddItem(ItemCode.Sapphire, 5);
        inven.AddItem(ItemCode.Sapphire, 5);
        inven.AddItem(ItemCode.Sapphire, 5);
        inven.Test_InventoryPrint();

        inventoryUI.InitializeInventory(inven);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // 추가
        inven.AddItem(code, fromeIndex);
        inven.Test_InventoryPrint();
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // 삭제
        inven.RemoveItem(fromeIndex);
        inven.Test_InventoryPrint();
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // 이동
        inven.MoveItem(fromeIndex, toIndex);
        inven.Test_InventoryPrint();
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        // 정렬
        inven.SlotSorting(sortBy, isAcending);
        inven.Test_InventoryPrint();
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        // 인벤토리 정리
        inven.ClearInventory();
        inven.Test_InventoryPrint();
    }
#endif
}
