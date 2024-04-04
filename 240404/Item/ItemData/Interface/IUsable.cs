using UnityEngine;

public interface IUsable
{
    /// <summary>
    /// 아이템을 사용하는 함수
    /// </summary>
    /// <param name="target">아이템에 효과를 받을 대상</param>
    /// <returns>사용 성공 여부</returns>
    bool Use(GameObject target);
}