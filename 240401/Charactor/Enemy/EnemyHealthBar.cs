using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    /// <summary>
    /// fill의 피봇이 될 트랜스폼
    /// </summary>
    Transform fillPivot;

    private void Awake()
    {
        fillPivot = transform.GetChild(1); // fill의 피봇 찾기

        IHealth target = GetComponentInParent<IHealth>();
        target.onHealthChange += Refresh; // 부모에서 IHealth 찾아서 델리게이트에 함수 연결
    }

    /// <summary>
    /// 부모의 HP가 변경되면 실행되는 함수
    /// </summary>
    /// <param name="ratio">HP 비율 (hp / maxHP)</param>
    private void Refresh(float ratio)
    {
        fillPivot.localScale = new(ratio, 1, 1); // 로컬 스케일 조절해서 HP 변화 표시
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation; // 빌보드로 만들기 ((카메라의 회전과 일치시켜서) 항상 카메라에 정면으로 비치게 만들기)
        //transform.forward = Camera.main.transform.forward;
    }
}

/// 실습_240401
/// 1. 적의 HP가 변경되면 이 HealthBar를 이용해 남은 HP를 표시한다. (비율로 표시)
/// 2. 색이 정상적으로 보이도록 머티리얼 변경
/// 3. HP는 왼쪽으로 줄어드는 듯이 보여야 한다.
/// 4. 빌보드이어야 한다. (항상 카메라를 정면으로 바라보아야 한다.)
