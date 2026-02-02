using UnityEngine;

public class TurnBaseManager : MonoBehaviour
{
    private bool isPlayerTurn = false;
    private bool isEnemyTurn = false;
    public bool battleEnd = true;

    private float timeCounter;
    public float enemyTurnDuration;
    public float playerTurnDuration;

    [Header("事件广播")]
    public ObjectEventSO playerTurnStartEvent;
    public ObjectEventSO enemyTurnStartEvent;
    public ObjectEventSO enemyTurnEndEvent;
    public ObjectEventSO gameStartEvent;
    private void Update()
    {
        if(battleEnd) return;

        if(isEnemyTurn)
        {
            timeCounter += Time.deltaTime;
            if(timeCounter >= enemyTurnDuration)
            {
                timeCounter = 0;
                //敌人回合结束
                EnemyTurnEnd();
                //玩家回合开始
                isPlayerTurn = true;
            }
        }

        if(isPlayerTurn)
        {
            timeCounter += Time.deltaTime;
            if(timeCounter >= playerTurnDuration)
            {
                //玩家回合真的开始了
                timeCounter = 0;
                PlayerTurnStart();
                isPlayerTurn = false;
            }
        }
    }

    [ContextMenu("Game Start")]
    public void GameStart()
    {
        isPlayerTurn = true;
        isEnemyTurn = false;
        battleEnd = false;
        timeCounter = 0;
        GameManager.Instance.ActiveGamePlayUI();
    }


    public void PlayerTurnStart()
    {
        playerTurnStartEvent.RaisEvent(null, this);
        Debug.Log("Player Turn Start");
    }


    public void EnemyTurnStart()
    {
        isEnemyTurn = true;
        enemyTurnStartEvent.RaisEvent(null, this);
        Debug.Log("Enemy Turn Start");
    }

    public void EnemyTurnEnd()
    {
        isEnemyTurn = false;
        enemyTurnEndEvent.RaisEvent(null, this);
        Debug.Log("Enemy Turn End");
    }

    public void GameEnd()
    {
        battleEnd = true;
        Debug.Log("Game End");
    }

    /// <summary>
    /// 房间加载后的逻辑
    /// </summary>
    public void OnRoomLoadedEvent(object obj)
    {
        Room room = obj as Room;
        Debug.Log(room.roomData.roomType);
        switch(room.roomData.roomType)
        {
            case RoomType.MinorEnemy:
            case RoomType.EliteEnemy:
            case RoomType.Boss:
                Debug.Log("战斗开始");
                GameStart();
                break;
            case RoomType.Shop:
                break;
            case RoomType.Reward:
                break;
        }
    }

    public void OnLoadMapEvent()
    {
        battleEnd = true;
    }
}
