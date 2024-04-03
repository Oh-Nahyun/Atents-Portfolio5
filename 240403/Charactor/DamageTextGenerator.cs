using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextGenerator : MonoBehaviour
{
    private void Start()
    {
        IBattler battler = GetComponentInParent<IBattler>();
        battler.onHit += DamageTextGenerate;
    }

    void DamageTextGenerate(int damage)
    {
        Factory.Instance.GetDamageText(damage, transform.position);
    }
}

/// 실습_2420402
/// 0. 이 스크립트를 가진 오브젝트의 부모가 데미지를 받으면 데미지 텍스트를 하니씩 생성하는 클래스
/// 1. 부모가 데미지를 받으면 DamageText 프리팹을 하나 생성한다. (얼마만큼의 데미지를 받았는지 생성하며 전달)
