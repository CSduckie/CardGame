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

    //初始化所有UI
    public void InitializeEnemyUI()
    {
        enemyHealthGreenBar.fillAmount = 1;
        enemyHealthOrangeBar.fillAmount = 0;
        enemyHealthRedBar.fillAmount = 0;
    }

    //更新敌人UI
    public void UpdateEnemyUI(float remainingHealth)
    {
        if(enemyController == null) return;
        enemyHealthGreenBar.fillAmount = remainingHealth / enemyController.health;
        enemyHealthRedBar.fillAmount = 1- enemyHealthGreenBar.fillAmount;
    }

    //预测敌人剩余血量
    public void UpdateEnemyPredictHealth(float damage)
    {
        enemyHealthOrangeBar.fillAmount = enemyHealthRedBar.fillAmount + (damage / enemyController.health);
    }
}
