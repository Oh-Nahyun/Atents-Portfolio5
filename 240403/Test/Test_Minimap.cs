using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Minimap : TestBase
{
    private void Start()
    {
        Factory.Instance.MakeItem(ItemCode.IronSword);
        Factory.Instance.MakeItem(ItemCode.SilverSword);
        Factory.Instance.MakeItem(ItemCode.OldSword);

        Factory.Instance.MakeItem(ItemCode.KiteShield);
        Factory.Instance.MakeItem(ItemCode.RoundShield);

        Factory.Instance.MakeItems(ItemCode.HealingPotion, 10);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Factory.Instance.GetEnemy();
    }
}
