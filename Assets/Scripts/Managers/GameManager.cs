using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("地图布局")]
    public MapLayoutSO mapLayout;
    [Header("UI")]
    public GamePlayPanel gamePlayPanel;

    
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 更新房间的事件
    /// </summary>
    /// <param name="roomVector"></param>
    public void UpdateMapLayoutData(object value)
    {
        var roomVector = (Vector2Int)value;
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

}
