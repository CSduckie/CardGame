using UnityEngine;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour
{
    [Header("敌人数据")]
    public EnemyDataSO enemyData;
    public float currentHealth;
    public EnemyUIController enemyUI;
    public float enemyMaxHealth;
    [Header("事件广播")]
    public ObjectEventSO gameEndEvent;

    public virtual void SetUpEnemy()
    {

        enemyUI = FindFirstObjectByType<EnemyUIController>();
        currentHealth = enemyData.health  + GameManager.Instance.roomsEntered * 2;
        enemyMaxHealth = enemyData.health;
        Debug.Log("敌人血量：" + currentHealth);
        enemyUI.enemyController = this;
        enemyUI.InitializeEnemyUI();
        enemyUI.enemyImage.sprite = enemyData.enemyImage;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            // Debug.Log("敌人死亡");
            enemyUI.UpdateEnemyUI(currentHealth);
            enemyUI.enemyController = null;
            //启动游戏结束事件，进入游戏结束流程
            gameEndEvent.RaisEvent(null, this);
            GameManager.Instance.isFirstTurn = true;
        }
        else
        {
            enemyUI.UpdateEnemyUI(currentHealth);
        }
    }
}
