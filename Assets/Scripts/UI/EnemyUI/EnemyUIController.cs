using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;
public class EnemyUIController : MonoBehaviour
{
    [Header("UI 组件")]
    public Image enemyImage;
    public Image enemyHealthGreenBar;
    public Image enemyHealthOrangeBar;
    public Image enemyHealthRedBar;

    public TextMeshProUGUI enemyHealthText;
    public EnemyController enemyController;

    private bool isAnimating = false;
    public float animSpeed = 10;
    private float targetFillAmount = 0;

    //当前动画
    private Sequence currentAnimation;
    
    //初始化所有UI
    public void InitializeEnemyUI()
    {
        enemyHealthGreenBar.fillAmount = 1;
        enemyHealthOrangeBar.fillAmount = 0;
        enemyHealthRedBar.fillAmount = 0;
        enemyHealthText.text = enemyController.enemyMaxHealth.ToString();
    }

    public void Update()
    {
        //更新敌人的血量UI动画
        if(isAnimating)
        {
            enemyHealthRedBar.fillAmount = Mathf.Lerp(enemyHealthRedBar.fillAmount, targetFillAmount, Time.deltaTime * animSpeed);
            if(Mathf.Abs(enemyHealthRedBar.fillAmount - targetFillAmount) < 0.01f)
            {
                enemyHealthRedBar.fillAmount = targetFillAmount;
                isAnimating = false;
                targetFillAmount = 0;
            }
        }
    }

    //更新敌人UI
    public void UpdateEnemyUI(float remainingHealth)
    {
        if (enemyController == null) return;
        enemyHealthGreenBar.fillAmount = remainingHealth / enemyController.enemyMaxHealth;
        targetFillAmount = 1 - enemyHealthGreenBar.fillAmount;
        isAnimating = true;

        UpdateTextUIAnimation(remainingHealth);

    }

    //更新文本UI动画
    private void UpdateTextUIAnimation(float remainingHealth)
    {
        //使用一个DOtween动画，让TotalDamage Text飞向当前敌人血量Text的位置，之后，OnComplete执行UpdateEnemyHealthText
        currentAnimation.Kill();
        //设置好floatValueText的text
        var totalDamageText = GameManager.Instance.gamePlayPanel.damageCalculationUI.floatValueText;
        //设置好floatValueText的初始位置
        var originalPosition = totalDamageText.transform.position;
        //启动当前游戏物体
        totalDamageText.gameObject.SetActive(true);
        //设置好floatValueText的text为totalValueText的text
        totalDamageText.text = GameManager.Instance.gamePlayPanel.damageCalculationUI.totalValueText.text;

        //更新当前的damageText为0
        GameManager.Instance.gamePlayPanel.damageCalculationUI.totalValueText.text = "0";

        //开始动画
        currentAnimation = DOTween.Sequence();


        currentAnimation.Append(totalDamageText.transform.DOMove(enemyHealthText.transform.position, 0.5f));
        currentAnimation.Play();
        currentAnimation.onComplete = () =>
        {
            StartCoroutine(UpdateEnemyHealthText(remainingHealth, 1f));
            //归位到初始位置
            totalDamageText.transform.position = originalPosition;
            totalDamageText.gameObject.SetActive(false);
        };
    }

    //预测敌人剩余血量
    public void UpdateEnemyPredictHealth(float damage)
    {
        enemyHealthOrangeBar.fillAmount = enemyHealthRedBar.fillAmount + (damage / enemyController.enemyMaxHealth);
    }


    //更新敌人血量文本携程
    private IEnumerator UpdateEnemyHealthText(float remainingHealth,float animationTime)
    {
        float animationTimePerStep = animationTime / (int.Parse(enemyHealthText.text) - remainingHealth);
        while(int.Parse(enemyHealthText.text) > remainingHealth)
        {
            enemyHealthText.text = (int.Parse(enemyHealthText.text) - 1).ToString();
            yield return new WaitForSeconds(animationTimePerStep);
        }
        enemyHealthText.text = remainingHealth.ToString();
    }
}
