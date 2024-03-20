using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconRotator : MonoBehaviour
{
    public float rotateeSpeed = 360.0f;
    public float moveSpeed = 2.0f;

    public float minHeight = 0.5f;
    public float maxHeight = 1.5f;

    private void Start()
    {
        transform.Rotate(0, Random.Range(0, 360), 0); // 초기 랜덤 설정
    }

    private void Update()
    {
        // 위아래로 계속 움직임 (높이 : minHeight ~ maxHeight), 삼각함수 활용
        // y축 기준으로 계속 회전
    }
}
