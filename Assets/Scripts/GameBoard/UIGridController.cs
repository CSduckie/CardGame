using UnityEngine;

public class UIGridController : MonoBehaviour
{
    public GameObject gridPrefab;
    public GameObject gameBoardPrefab;
    public GameBoardDataSO gameBoardData;
    
    [Header("当前棋盘信息")]
    public int row, column;
    [Header("事件广播")]
    public ObjectEventSO gameStartEvent;
    void Start()
    {
        Invoke("CreateGrid",1);
    }

    void CreateGrid()
    {
        //创建棋盘空物体
        GameObject gameBoard = Instantiate(gameBoardPrefab, transform.position, Quaternion.identity);
        gameBoard.GetComponent<GameBoardController>().row = row;
        gameBoard.GetComponent<GameBoardController>().column = column;
        //遍历所有子物体，给每一个创建一个grid
        for(int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            //UI元素，进行坐标转换
            Vector3 pos = Camera.main.ScreenToWorldPoint(child.position);
            pos.z = 0;
            GameObject newGrid = Instantiate(gridPrefab, pos, Quaternion.identity);
            SlotController currentSlot = newGrid.GetComponent<SlotController>();
            currentSlot.Init((i / column) + 1, (i % column) + 1);

            //设置slot的value
            if(i % 5 == 0)
            {
                currentSlot.value = 0;
            }
            else
            {
                currentSlot.value = gameBoardData.columnDataList[(i % 5) - 1].value;
                currentSlot.isMultiply = gameBoardData.columnDataList[(i % 5) - 1].isMultiply;
            }

            newGrid.transform.SetParent(gameBoard.transform);
        }
        gameStartEvent.RaisEvent(null, this);
        Destroy(gameObject);
    }
}
