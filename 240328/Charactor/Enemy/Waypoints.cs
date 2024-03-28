using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 웨이포인트를 저장하고 처리하는 클래스
/// </summary>
public class Waypoints : MonoBehaviour
{
    public Transform Current => null;
}

/// 실습_240328
/// 1. 적이 순찰 상태에 들어가면 일정 시간 대기한다.
/// 2. 순찰 상태의 대기 시간이 다 되면 순찰상태로 변경되고, 다음 지점으로 이동한다.
/// 3. 순잘 상태에서 목적지에 도착하면 다시 대기 상태로 들어간다.
