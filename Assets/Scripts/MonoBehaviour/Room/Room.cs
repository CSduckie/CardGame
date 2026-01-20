using UnityEngine;
using System.Collections.Generic;
public class Room : MonoBehaviour
{
    [Header("Room Location")]
    public int column;
    public int row;

    [Header("Room Data")]
    private SpriteRenderer spriteRenderer;

    public RoomDataSO roomData;

    public RoomState roomState;

    public List<Vector2Int> linkTo = new();

    [Header("事件广播")]
    public ObjectEventSO loadRoomEvent;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    private void OnMouseDown()
    {
        Debug.Log("Click!");
        if(roomState == RoomState.Active)
            loadRoomEvent.RaisEvent(this, this);
    }

    /// <summary>
    /// 设置房间
    /// </summary>
    /// <param name="column">列</param>
    /// <param name="row">行</param>
    /// <param name="roomData">房间数据</param>

    public void SetUpRoom(int column, int row, RoomDataSO roomData)
    {
        this.column = column;
        this.row = row;
        this.roomData = roomData;
        spriteRenderer.sprite = roomData.roomSprite;
        
        spriteRenderer.color = roomState switch
        {
            RoomState.Active => Color.white,
            RoomState.Locked => Color.gray,
            RoomState.Entered => Color.yellow,
            _ => Color.white
        };
    }
}
