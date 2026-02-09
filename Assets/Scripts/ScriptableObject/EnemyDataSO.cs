using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public Sprite enemyImage;
    public EnemyType enemyType;
    public float health;

    //敌人棋盘模式
    //特殊格子数
    public int specialGridCount;
}
