using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening.Plugins.Options;
public class DamageUIPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI addValueText;
    public TextMeshProUGUI multiplyValueText;
    public TextMeshProUGUI totalValueText;
    public TextMeshProUGUI floatValueText;

    [Header("伤害吊牌以及动画参数")]
    public GameObject damageBoard;
    public float maxAngle = 8f;            // 最大摆幅（度）
    public float kickVel = 200f;           // 推一下给的角速度强度（度/秒）
    public float returnStrength = 30f;     // 回正力度（越大越“弹”）
    public float damping = 6f;             // 阻尼（越大越快停）
    public float stopAngle = 0.05f;        // 停止阈值
    float angle;  // 当前角度（度）
    float vel;    // 当前角速度（度/秒）
    Coroutine co;
    int dir = -1;
    /// <summary>
    /// damage01: 建议传 0~1
    /// dir: 1右 / -1左（不传就默认交替或固定）
    /// </summary>
    public void damageBoardShakeAnim(float _damagePercentage)
    {
        if(co != null)
        {
            StopCoroutine(co);
            co = null;
        }
        _damagePercentage = Mathf.Clamp01(_damagePercentage);

        // 把“力道”映射到初始角速度：力越大，初始速度越大→摆幅越大
        vel += dir * kickVel * _damagePercentage;

        if (co == null) co = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        while (true)
        {
            float dt = Time.deltaTime;

            
            vel += (-returnStrength * angle) * dt;

            //让速度逐渐变小
            vel = Mathf.MoveTowards(vel, 0f, damping * dt);

            //速度推动角度变化
            angle += vel * dt;

            //限制最大角度
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

            //应用旋转
            damageBoard.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            //停止条件：角度几乎为0 且 速度也几乎为0
            if (Mathf.Abs(angle) < stopAngle && Mathf.Abs(vel) < stopAngle)
            {
                angle = 0f;
                vel = 0f;
                damageBoard.transform.rotation = Quaternion.identity;
                co = null;
                yield break;
            }

            yield return null;
        }
    }
}
