using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour, IBattler, IHealth
{
    /// <summary>
    /// 적이 가질 수 있는 상태의 종류
    /// </summary>
    protected enum EnemyState
    {
        Wait = 0,   // 대기
        Patrol,     // 순찰
        Chase,      // 추적
        Attack,     // 공격
        Dead        // 사망
    }

    /// <summary>
    /// 적의 현재 상태
    /// </summary>
    EnemyState state = EnemyState.Patrol;

    /// <summary>
    /// 상태를 설정하고 확인하는 프로퍼티
    /// </summary>
    protected EnemyState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                switch (state) // 상태에 진입할 때, 할 일들 처리
                {
                    case EnemyState.Wait:
                        // 일정 시간 대기
                        agent.isStopped = true;         // agent 정지
                        agent.velocity = Vector3.zero;  // agent에 남아있던 운동량 제거
                        animator.SetTrigger("Stop");    // 애니메이션 정지
                        WaitTimer = waitTime;           // 기다려야 하는 시간 초기화
                        onStateUpdate = Update_Wait;    // 대기 상태용 업데이트 함수 설정
                        break;

                    case EnemyState.Patrol:
                        // Debug.Log("Patrol State");
                        agent.isStopped = false;                       // agent 다시 켜기
                        agent.SetDestination(waypoints.NextTarget);    // 목적지 지정 (웨이포인트 지점)
                        animator.SetTrigger("Move");
                        onStateUpdate = Update_Patrol;
                        break;

                    case EnemyState.Chase:
                        agent.isStopped = false;
                        animator.SetTrigger("Move");
                        onStateUpdate = Update_Chase;
                        break;

                    case EnemyState.Attack:
                        onStateUpdate = Update_Attack;
                        break;

                    case EnemyState.Dead:
                        onStateUpdate = Update_Dead;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 대기 상태로 들어갔을 때, 기다리는 시간
    /// </summary>
    public float waitTime = 1.0f;

    /// <summary>
    /// 대기 시간 측정용 (계속 감소)
    /// </summary>
    float waitTimer = 1.0f;

    /// <summary>
    /// 측정용 시간 처리용 프로퍼티
    /// </summary>
    protected float WaitTimer
    {
        get => waitTimer;
        set
        {
            waitTimer = value;
            if (waitTimer < 0.0f)
            {
                State = EnemyState.Patrol;
            }
        }
    }

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 3.0f;

    /// <summary>
    /// 적이 순찰할 웨이포인트
    /// </summary>
    public Waypoints waypoints;

    /// <summary>
    /// 이동할 웨이포인트의 트랜스폼
    /// </summary>
    //protected Transform waypointTarget = null;

    /// <summary>
    /// 원거리 시야 범위
    /// </summary>
    public float farSightRange = 10.0f;

    /// <summary>
    /// 원거리 시야각의 절반
    /// </summary>
    public float sightHalfAngel = 50.0f;

    /// <summary>
    /// 근거리 시야 범위
    /// </summary>
    public float nearSightRange = 1.5f;

    /// <summary>
    /// 추적 대상의 트랜스폼
    /// </summary>
    protected Transform chaseTarget = null;

    /// <summary>
    /// 공격 대상
    /// </summary>
    protected IBattler attackTarget = null;

    /// <summary>
    /// 공격력 (변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float attackPower = 10.0f;
    public float AttackPower => attackPower;

    /// <summary>
    /// 방어력 (변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float defencePower = 3.0f;
    public float DefencePower => defencePower;

    /// <summary>
    /// 공격 속도
    /// </summary>
    public float attackSpeed = 1.0f;

    /// <summary>
    /// 남아있는 공격 쿨타임
    /// </summary>
    public float attackCoolTime = 0.0f;

    /// <summary>
    /// HP
    /// </summary>
    protected float hp = 100.0f;
    public float HP
    {
        get => hp;
        set
        {
            hp = value;
            if (State != EnemyState.Dead && hp <= 0) // 한 번만 죽기 용도
            {
                Die();
            }
            hp = Mathf.Clamp(hp, 0, MaxHP);
            onHealthChange?.Invoke(hp / MaxHP);
        }
    }

    /// <summary>
    /// 최대 HP (변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float maxHP = 100.0f;
    public float MaxHP => maxHP;

    /// <summary>
    /// HP 변경 시 실행되는 델리게이트
    /// </summary>
    public Action<float> onHealthChange { get; set; }

    /// <summary>
    /// 살았는지 죽엇는지 확인하기 위한 프로퍼티
    /// </summary>
    public bool IsAlive => hp > 0;

    /// <summary>
    /// 이 캐릭터가 죽었을 때, 실행되는 델리게이트
    /// </summary>
    public Action onDie { get; set; }

    [System.Serializable] // 이게 있어야 구조체 내용을 인스팩터 창에서 수정할 수 있다.
    public struct ItemDropInfo
    {
        public ItemCode code;       // 아이템 종류

        [Range(0, 1)]
        public float dropRatio;     // 드랍 확률 (1.0f = 100%)
    }

    /// <summary>
    /// 이 적이 죽을 때, 드랍하는 아이템 정보
    /// </summary>
    public ItemDropInfo[] dropItems;

    /// <summary>
    /// 상태별 업데이트 함수가 저장될 델리게이트 (함수 저장용)
    /// </summary>
    Action onStateUpdate;

    // 컴포넌트들
    Animator animator;
    NavMeshAgent agent;
    SphereCollider bodyCollider;
    Rigidbody rigid;
    ParticleSystem dieEffect;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        bodyCollider = GetComponent<SphereCollider>();
        rigid = GetComponent<Rigidbody>();
        // dieEffect = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        agent.speed = moveSpeed;
        //if (waypoints == null)
        //{
        //    waypointTarget = transform;
        //}
        //else
        //{
        //    waypointTarget = waypoints.NextTarget;
        //}

        State = EnemyState.Wait;
        animator.ResetTrigger("Stop"); // Wait 상태로 설정하면서 Stop 트리거가 쌓인 것을 제거하기 위해 필요!!
    }

    void Update()
    {
        onStateUpdate();
    }

    /// <summary>
    /// Wait 상태용 업데이트 함수
    /// </summary>
    void Update_Wait()
    {
        if (SearchPlayer())
        {
            State = EnemyState.Chase;
        }
        else
        {
            WaitTimer -= Time.deltaTime; // 기다리는 시간 감소 (0이 되면 Patrol로 변경)

            // 다음 목적지를 바라보게 만들기
            Quaternion look = Quaternion.LookRotation(waypoints.NextTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 2);
        }
    }

    /// <summary>
    /// Patrol 상태용 업데이트 함수
    /// </summary>
    void Update_Patrol()
    {
        if (SearchPlayer())
        {
            State = EnemyState.Chase;
        }
        else
        {
            if (agent.remainingDistance <= agent.stoppingDistance)  // 도착하면
            {
                waypoints.StepNextWaypoint();                       // 웨이포인트가 다음 지점을 설정하도록 실행
                State = EnemyState.Wait;                            // 대기 상태로 전환
            }
        }
    }

    void Update_Chase()
    {
        if (SearchPlayer())
        {
            agent.SetDestination(chaseTarget.position);
        }
        else
        {
            State = EnemyState.Wait;
        }
    }

    void Update_Attack()
    {

    }

    void Update_Dead()
    {

    }

    /// <summary>
    /// 시야 범위 안에 플레이어가 있는지 없는지 찾는 함수
    /// </summary>
    /// <returns>찾았으면 true, 못찾았으면 false</returns>
    bool SearchPlayer()
    {
        bool result = false;
        chaseTarget = null;

        // 일정 반경(farSightRange) 안에 있는 플레이어 레이어에 있는 오브젝트 전부 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, farSightRange, LayerMask.GetMask("Player"));
        if (colliders.Length > 0)
        {
            // 일정 반경(farSightRange) 안에 플레이어가 있다.
            Vector3 playerPos = colliders[0].transform.position;            // 0번이 무조건 플레이어다 (플레이어는 1명이니까)
            Vector3 toPlayerDir = playerPos - transform.position;           // 적 -> 플레이어로 가는 방향 벡터
            if (toPlayerDir.sqrMagnitude < nearSightRange * nearSightRange) // 플레이어는 nearSightRange보다 안쪽에 있다.
            {
                // 근접 범위(nearSightRange) 안쪽이다.
                chaseTarget = colliders[0].transform;
                result = true;
            }
            else
            {
                // 근접 범위 밖이다 => 시야각 확인
                if (IsInSightAngle(toPlayerDir))    // 시야각 안인지 확인
                {
                    if (IsSightClear(toPlayerDir))  // 적과 플레이어 사이에 시야를 가리는 오브젝트가 있는지 확인
                    {
                        chaseTarget = colliders[0].transform;
                        result = true;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 시야각(-sightHalfAngel ~ +sightHalfAngel)안에 플레이어가 있는지 없는지 확인하는 함수
    /// </summary>
    /// <param name="toTargetDirection">적에서 대상으로 향하는 방향 벡터</param>
    /// <returns>시야각 안에 있으면 true, 없으면 false</returns>
    bool IsInSightAngle(Vector3 toTargetDirection)
    {
        float angle = Vector3.Angle(transform.forward, toTargetDirection); // 적의 포워드와 적을 바라보는 방향 벡터 사이의 각을 구함
        return sightHalfAngel > angle;
    }

    /// <summary>
    /// 적이 다른 오브젝트에 의해 가려지는지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="toTargetDirection">적에서 대상으로 향하는 방향 벡터</param>
    /// <returns>true면 가려지지 않고, false면 가려진다.</returns>
    bool IsSightClear(Vector3 toTargetDirection)
    {
        bool result = false;
        Ray ray = new(transform.position + transform.up * 0.5f, toTargetDirection); // 래이 생성 (눈 높이 때문에 조금 높임)
        if (Physics.Raycast(ray, out RaycastHit hitInfo, farSightRange))
        {
            if (hitInfo.collider.CompareTag("Player"))  // 처음 충돌한 것이 플레이어라면
            {
                result = true;                          // 중간에 가리는 물체가 없다는 소리
            }
        }

        return result;
    }





    public void Attack(IBattler target)
    {
        target.Defence(AttackPower);
    }

    public void Defence(float damage)
    {
        if (IsAlive)
        {
            animator.SetTrigger("Hit");
            HP -= Mathf.Max(0, damage - DefencePower);
            Debug.Log($"적이 맞았다. 남은 HP = {HP}");
        }
    }

    public void Die()
    {
        Debug.Log("사망");
    }

    public void HealthRegenerate(float totalRegen, float duration)
    {
        throw new NotImplementedException();
    }

    public void HealthRegenerateByTick(float tickRegen, float tickInterval, uint totalTickCount)
    {
        throw new NotImplementedException();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        bool playerShow = SearchPlayer();
        Handles.color = playerShow ? Color.red : Color.green;

        Vector3 forward = transform.forward * farSightRange;
        Handles.DrawDottedLine(transform.position, transform.position + forward, 2.0f); // 중심선 그리기

        Quaternion q1 = Quaternion.AngleAxis(-sightHalfAngel, transform.up);            // 중심선 회전시키고
        Handles.DrawLine(transform.position, transform.position + q1 * forward);        // 선 긋기

        Quaternion q2 = Quaternion.AngleAxis(sightHalfAngel, transform.up);
        Handles.DrawLine(transform.position, transform.position + q2 * forward);

        Handles.DrawWireArc(transform.position, transform.up, q1 * forward, sightHalfAngel * 2, farSightRange, 2.0f);

        Handles.DrawWireDisc(transform.position, transform.up, nearSightRange);
    }
#endif
}

/// 실습_240329
/// 1. SearchPlayer() 함수 - Physics 클래스 이용하기
///     1-1. 일정 반경(farSightRange) 안에 있고, 시야각 안에 있는 플레이어를 찾으면 true
///     1-2. 가까이(nearSightRange)에 있는 플레이어를 찾으면 true
/// 2. IsInSightAngle() 함수
/// 3. IsSightClear() 함수
