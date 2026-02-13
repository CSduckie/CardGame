using UnityEngine;
using UnityEngine.UI;
public class EnemyUIController : MonoBehaviour
{
    [Header("UI 组件")]
    public Image enemyImage;
    public Image enemyHealthGreenBar;
    public Image enemyHealthOrangeBar;
    public Image enemyHealthRedBar;

    public EnemyController enemyController;

    private bool isAnimating = false;
    public float animSpeed = 10;
    private float targetFillAmount = 0;
    //初始化所有UI
    public void InitializeEnemyUI()
    {
        enemyHealthGreenBar.fillAmount = 1;
        enemyHealthOrangeBar.fillAmount = 0;
        enemyHealthRedBar.fillAmount = 0;
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
        if(enemyController == null) return;
        enemyHealthGreenBar.fillAmount = remainingHealth / enemyController.enemyMaxHealth;
        targetFillAmount = 1 - enemyHealthGreenBar.fillAmount;
        isAnimating = true;
    }

    //预测敌人剩余血量
    public void UpdateEnemyPredictHealth(float damage)
    {
        enemyHealthOrangeBar.fillAmount = enemyHealthRedBar.fillAmount + (damage / enemyController.enemyMaxHealth);
    }
}
