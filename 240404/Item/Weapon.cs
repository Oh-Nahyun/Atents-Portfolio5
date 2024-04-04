using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// 칼날 부분의 트리거
    /// </summary>
    CapsuleCollider bladeVolume;

    /// <summary>
    /// 칼을 휘두른 흔적용 파티클 시스템
    /// </summary>
    ParticleSystem ps;

    /// <summary>
    /// 무기를 가지고 있는 플레이어
    /// </summary>
    Player player;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        bladeVolume = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    /// <summary>
    /// 칼을 휘두른 흔적용 이펙트를 켜고 끄는 함수 (공격 애니메이션에서 실행 처리)
    /// </summary>
    /// <param name="enable">true면 켜고, false면 끈다.</param>
    public void EffectEnable(bool enable)
    {
        if (enable)
        {
            ps.Play();
        }
        else
        {
            ps.Stop();
        }
    }

    /// <summary>
    /// 칼의 충돌 영역을 켜고 끄는 함수 (애니메이션 동작 중에 이벤트로 실행됨)
    /// </summary>
    /// <param name="isEnable"></param>
    public void BladeVolumeEnable(bool isEnable)
    {
        bladeVolume.enabled = isEnable;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IBattler target = other.GetComponent<IBattler>();
            if (target != null)
            {
                player.Attack(target);

                Vector3 impactPoint = transform.position + transform.up * 0.8f; // 칼날 부분 중 한 군데를 임의로 지정
                Vector3 effectPoint = other.ClosestPoint(impactPoint);          // impactPoint 위치와 other가 가장 가까운 위치
                Factory.Instance.GetPlayerHitEffect(effectPoint);                // effectPoint 위치에 이팩트 생성

                //Time.timeScale = 0.0f;
            }
        }
    }
}

/// 실습_240329
/// Hit 이팩트를 풀에 추가한 후,
/// 무기가 몬스터를 때릴 때 하나씩 꺼내쓰기
