using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("地图布局")]
    public MapLayoutSO mapLayout;
    [Header("UI")]
    public GamePlayPanel gamePlayPanel;
    public GameObject gameWinPanel;
    public GameObject gameLosePanel;
    public GameObject pickCardPanel;
    
    [Header("其他Manager")]
    public GameBoardController gameBoardController;
    public CardLayoutManager cardLayoutManager;
    public CardDeck cardDeck;
    
    [Header("游戏是否失败")]
    public bool isGameFailed = false;
    public bool isFirstTurn = true;
    [Header("事件广播")]
    public ObjectEventSO gameFailedEvent;
    public static GameManager Instance;

    //动态难度计数器
    public int roomsEntered;
    private void Awake()
    {
        Instance = this;
        cardDeck = FindFirstObjectByType<CardDeck>();
        isGameFailed = false;
        roomsEntered = 0;
    }

    /// <summary>
    /// 检查游戏是否失败
    /// </summary>
    public void CheckGameFailed()
    {
        if(!gameBoardController.CheckHasSoilderOnBoard() && cardDeck.hasMoreCardToDraw)
        {
            if(isFirstTurn)
            {
                isFirstTurn = false;
                return;
            }
            //TODO:检查手牌是否还有卡牌，如果有，则不失败，同时禁用EndTurn按钮
            //通过检查CardLayoutManager中的CardPositionList是否还有卡牌，如果有，则不失败，同时禁用EndTurn按钮
            if(cardLayoutManager.cardPos.Count > 0)
            {
                gamePlayPanel.endTurnButton.interactable = false;
                return;
            }
            isGameFailed = true;
            Debug.Log("游戏失败");
            //启动游戏失败事件
            gameFailedEvent.RaisEvent(null, this);
        }
    }


    /// <summary>
    /// 更新房间的事件
    /// </summary>
    /// <param name="roomVector"></param>
    public void UpdateMapLayoutData(object value)
    {
        var roomVector = (Vector2Int)value;

        if(mapLayout.mapRoomDataList.Count == 0) return;

        //根据传输进来的roomVector查找符合某些属性的列表
        var currentRoom = mapLayout.mapRoomDataList.Find(r => r.colum == roomVector.x && r.line == roomVector.y);

        currentRoom.roomState = RoomState.Entered;
        //更新相邻房间数据
        var sameColumnRooms = mapLayout.mapRoomDataList.FindAll(r => r.colum == currentRoom.colum);

        foreach(var room in sameColumnRooms)
        {
            if(room.line != roomVector.y)
                room.roomState = RoomState.Locked;
        }

        //根据保存的linkto设置跟进入房间相邻的右边的房间的状态为可进入
        foreach(var link in currentRoom.linkTo)
        {
            var linkedRoom = mapLayout.mapRoomDataList.Find(r => r.colum == link.x && r.line == link.y);
            linkedRoom.roomState = RoomState.Active;
        }
    }

    #region UI控制
    public void ActiveGamePlayUI()
    {
        gamePlayPanel.gameObject.SetActive(true);
    }

    public void ActiveGameWinUI()
    {
        gameWinPanel.gameObject.SetActive(true);
    }

    public void ActiveGameLoseUI()
    {
        gameLosePanel.gameObject.SetActive(true);
        gamePlayPanel.gameObject.SetActive(false);
        gameWinPanel.gameObject.SetActive(false);
        pickCardPanel.gameObject.SetActive(false);
    }

    public void DisableAllUI()
    {
        gamePlayPanel.gameObject.SetActive(false);
        gameWinPanel.gameObject.SetActive(false);
        pickCardPanel.gameObject.SetActive(false);
        gameLosePanel.gameObject.SetActive(false);
    }

    public void ActivePickCardUI()
    {
        pickCardPanel.gameObject.SetActive(true);
        gameWinPanel.gameObject.SetActive(false);
        pickCardPanel.gameObject.SetActive(false);
        gameLosePanel.gameObject.SetActive(false);
    }

    #endregion


    //新游戏开始时，清除游戏地图数据
    public void OnNewGameEvent()
    {
        Debug.Log("新游戏开始，清除游戏地图数据");
        mapLayout.mapRoomDataList.Clear();
        mapLayout.linePositionsList.Clear();
    }
}
