using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 200f;
    private float currentHealth;
    public EnemyUIController enemyUI;
    [Header("事件广播")]
    public ObjectEventSO gameEndEvent;

    public void SetUpEnemy()
    {
        enemyUI = FindFirstObjectByType<EnemyUIController>();
        currentHealth = health;
        enemyUI.enemyController = this;
    }

    public void TakeDamage(float damage)
    {
        
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("敌人死亡");
            enemyUI.UpdateEnemyUI(currentHealth);
            enemyUI.enemyController = null;
            //启动游戏结束事件，进入游戏结束流程
            gameEndEvent.RaisEvent(null, this);
        }
        else
        {
            enemyUI.UpdateEnemyUI(currentHealth);
        }
    }
}
