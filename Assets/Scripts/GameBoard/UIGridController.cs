using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;
public class UIGridController : MonoBehaviour
{
    public GameObject gridPrefab;
    public GameObject gameBoardPrefab;
    public GameBoardDataSO gameBoardData;
    
    [Header("当前棋盘信息")]
    public int row, column;
    [Header("事件广播")]
    public ObjectEventSO gameStartEvent;

    [Header("特殊地格")]
    public int specialGridCount;
    private int loopTime = 0;
    private int maxTryTime = 100;
    //记录特殊格子索引
    public List<int> specialGridList = new();
    public EnemyController enemyCurrentLevel;
    void Start()
    {
        enemyCurrentLevel = FindFirstObjectByType<EnemyController>();
        Invoke("CreateGrid",1);
    }

    void CreateGrid()
    {
        //创建棋盘空物体
        GameObject gameBoard = Instantiate(gameBoardPrefab, transform.position, Quaternion.identity);
        gameBoard.GetComponent<GameBoardController>().row = row;
        gameBoard.GetComponent<GameBoardController>().column = column;

        CreateSpecialGridList();


        //遍历所有子物体，给每一个创建一个grid
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            //UI元素，进行坐标转换
            Vector3 pos = Camera.main.ScreenToWorldPoint(child.position);
            pos.z = 0;
            GameObject newGrid = Instantiate(gridPrefab, pos, Quaternion.identity);
            SlotController currentSlot = newGrid.GetComponent<SlotController>();
            currentSlot.Init((i / column) + 1, (i % column) + 1);

            //设置slot的value
            if (i % 5 == 0)
            {
                currentSlot.value = 0;
            }
            else
            {
                currentSlot.value = gameBoardData.columnDataList[(i % 5) - 1].value;
                currentSlot.isMultiply = gameBoardData.columnDataList[(i % 5) - 1].isMultiply;
            }

            newGrid.transform.SetParent(gameBoard.transform);

            //设置slot是否为特殊格子
            foreach (int index in specialGridList)
            {
                if (index == i)
                {
                    currentSlot.isSpecial = true;

                    //随机一个格子类型
                    SpecialGridType randomType = GetRandomGridType(enemyCurrentLevel.enemyData.specialGridTypes);
                    Debug.Log("随机格子类型：" + randomType);

                    switch(randomType)
                    {
                        case SpecialGridType.Cold:
                            currentSlot.GetComponent<SpriteRenderer>().color = Color.blue;
                            currentSlot.specialGridType = SpecialGridType.Cold;
                            break;
                        case SpecialGridType.Tower:
                            currentSlot.GetComponent<SpriteRenderer>().color = Color.red;
                            currentSlot.specialGridType = SpecialGridType.Tower;
                            break;
                        case SpecialGridType.Posion:
                            currentSlot.GetComponent<SpriteRenderer>().color = Color.green;
                            currentSlot.specialGridType = SpecialGridType.Posion;
                            break;
                        case SpecialGridType.Trap:
                            currentSlot.GetComponent<SpriteRenderer>().color = Color.yellow;
                            currentSlot.specialGridType = SpecialGridType.Trap;
                            break;
                    }
                }
            }
        }
        gameStartEvent.RaisEvent(null, this);
        specialGridList.Clear();
        Destroy(gameObject);
    }

    private void CreateSpecialGridList()
    {
        //赋值特殊格子数量，后续用于随机选择特殊格子
        specialGridCount = FindFirstObjectByType<EnemyController>().enemyData.specialGridCount;
        //随机选择特殊格子，
        while (loopTime < specialGridCount && loopTime < maxTryTime)
        {
            loopTime++;
            int randomIndex = Random.Range(0, transform.childCount);
            int x = randomIndex % column;
            //如果是第一行，则跳过
            if (x == 0) continue;
            //如果列表中已经存在，则跳过
            if (specialGridList.Contains(randomIndex)) continue;

            specialGridList.Add(randomIndex);
            // Debug.Log("特殊格子索引：" + randomIndex);
        }
    }

    //使用本地方法随机选择特殊格子
    private SpecialGridType GetRandomGridType(SpecialGridType flags)
    {
        string[] options = flags.ToString().Split(',');

        string randomOption = options[Random.Range(0, options.Length)];

        SpecialGridType gridType = (SpecialGridType)Enum.Parse(typeof(SpecialGridType),randomOption);

        return gridType;
    }
}
