using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : RecycleObject
{
    /// <summary>
    /// 이동 변화를 나타내는 커브
    /// </summary>
    public AnimationCurve movement;

    /// <summary>
    /// 투명 정도와 크기 변화를 나타내는 커브
    /// </summary>
    public AnimationCurve fade;

    /// <summary>
    /// 최대로 올라가는 정도 (최대 높이 = baseHeight + maxHeight)
    /// </summary>
    public float maxHeight = 1.5f;

    /// <summary>
    /// 전체 재생 시간
    /// </summary>
    public float duration = 1.0f;

    /// <summary>
    /// 현재 진행 시간
    /// </summary>
    float elapsedTime = 0.0f;

    /// <summary>
    /// 기본 높이 (생성되었을 때의 높이)
    /// </summary>
    float baseHeight = 0.0f;

    // 컴포넌트
    TextMeshPro damageText;

    private void Awake()
    {
        damageText = GetComponent<TextMeshPro>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 각종 초기화
        elapsedTime = 0.0f;                 // 진행시간 초기화
        damageText.color = Color.white;     // 색상 초기화
        baseHeight = transform.position.y;  // 기본 높이 설정
        transform.localScale = Vector3.one; // 스케일 초기화
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;              // 시간 누적시키고
        float timeRatio = elapsedTime / duration;   // 시간 진행률 계산

        float curveMove = movement.Evaluate(timeRatio);                                                 // 커브에서 높이 정도 가져오기
        float currentHeight = baseHeight + maxHeight * curveMove;                                       // 새 높이 계산
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);    // 새 높이 지정
        // transform.position.Set(transform.position.x, currentHeight, transform.position.z);

        float curveAlpha = fade.Evaluate(timeRatio);                    // 커브에서 투명도 및 스케일 값 가져오기
        damageText.color = new Color(1, 1, 1, curveAlpha);              // 색상 설정
        transform.localScale = new(curveAlpha, curveAlpha, curveAlpha); // 스케일 설정

        if (elapsedTime > duration) // 진행 시간이 다 되면
        {
            gameObject.SetActive(false); // 스스로 비활성화
        }
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation; // 빌보드 만들기
    }

    /// <summary>
    /// 출력될 숫자 설정
    /// </summary>
    /// <param name="damage">출력될 데미지</param>
    public void SetDamage(int damage)
    {
        damageText.text = damage.ToString();
    }
}

/// 실습_240402
/// 풀이 있다.
/// 활성화되면 위로 올라간다.
/// 활성화 된 후 시간이 지날수록 점점 투명해진다. (커브 사용)
/// 완전히 투명해지면 비활성화 된다.
