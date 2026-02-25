using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public Sprite enemyImage;
    public EnemyType enemyType;
    public float health;

    //敌人棋盘模式
    //特殊格子数
    public int specialGridCount;

    //从createGrid的时候读取specialGridTypes的索引
    public SpecialGridType specialGridTypes;
}


