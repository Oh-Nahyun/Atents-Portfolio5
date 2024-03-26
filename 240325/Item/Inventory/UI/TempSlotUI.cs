using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempSlotUI : SlotUI_Base
{
    InvenTempSlot tempSlot;
    Player owner;

    public uint FromIndex => tempSlot.FromIndex;

    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();
    }

    public override void InitializeSlot(InvenSlot slot)
    {
        base.InitializeSlot(slot);
        tempSlot = slot as InvenTempSlot;
        owner = GameManager.Instance.InventoryUI.Owner;
        Close();
    }

    public void Open()
    {
        transform.position = Mouse.current.position.ReadValue();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    //public void SetFromIndex(uint index)
    //{
    //    tempSlot.SetFromIndex(index);
    //}

    /// <summary>
    /// 마우스 버튼이 인벤토리 영역 밖에서 떨어졌을 때 실행되는 함수
    /// </summary>
    /// <param name="screenPosition">마우스 커서의 스크린 좌표 위치</param>
    public void OnDrop(Vector2 screenPosition)
    {
        /// 실습_240321
        /// 일단 아이템이 있을 때만 처리
        /// 스크린 좌표를 이용해서 레이 생성
        /// 레이를 이용해서 레이캐스트 실행 (Ground 레이어에 있는 컬라이더랑만 체크)
        /// 충돌 지점에 아이템 생성
        /// 임시 슬롯 비우기

        // 일단 아이템이 있을 때만 처리
        if (!InvenSlot.IsEmpty)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition); // 스크린 좌표를 이용해서 레이 생성
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000.0f, LayerMask.GetMask("Ground")))
            {
                // 레이를 이용해서 레이캐스트 실행 (Ground 레이어에 있는 컬라이더랑만 체크)
                Vector3 dropPosition = hitInfo.point;
                dropPosition.y = 0;         ///// 평지인 경우, 사용 가능

                Vector3 dropDir = dropPosition - owner.transform.position;
                if (dropDir.sqrMagnitude > owner.ItemPickupRange * owner.ItemPickupRange) ///// 거리는 직접 쓰지 않고 이렇게 사용해야 한다!!!!!!!!!!!!!!
                {
                    dropPosition = dropDir.normalized * owner.ItemPickupRange + owner.transform.position;
                }

                // 충돌 지점에 아이템 생성
                Factory.Instance.MakeItems(InvenSlot.ItemData.code, InvenSlot.ItemCount,
                                            dropPosition, InvenSlot.ItemCount > 1);
                InvenSlot.ClearSlotItem(); // 임시 슬롯 비우기
            }
        }
    }
}
