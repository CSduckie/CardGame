using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
public class MapGenerator : MonoBehaviour
{
    [Header("地图配置表")]
    public MapConfigSO mapConfig;

    [Header("地图数据保存")]
    public MapLayoutSO mapLayout;


    [Header("房间预制体")]
    public Room roomPrefab;
    public LineRenderer linePrefab;

    private float screenHeight;
    private float screenWidth;

    private float columnWidth;
    private Vector3 generatePoint;

    public float border = 1f;

    private List<Room> rooms = new();
    //保存的连线
    private List<LineRenderer> lines = new();

    public List<RoomDataSO> roomDataList = new();
    //创建字典用于随机地图
    private Dictionary<RoomType, RoomDataSO> roomDataDict = new();

    private void Awake()
    {
        //获取正交相机的画面高度，然后乘以对应的屏幕比例获取相机宽度
        screenHeight = Camera.main.orthographicSize * 2;
        screenWidth = screenHeight * Camera.main.aspect;

        columnWidth = screenWidth/(mapConfig.roomBluePrints.Count + 1);

        foreach(var roomData in roomDataList)
        {
            roomDataDict.Add(roomData.roomType, roomData);
        }
    }

    // private void Start()
    // {
    //     CreateMap();
    // }

    private void OnEnable() 
    {
        if(mapLayout.mapRoomDataList.Count > 0)
        {
            LoadMap();
        }
        else
        {
            CreateMap();
        }
    }

    public void CreateMap()
    {
        //创建前一列的房间列表，用作连线
        List<Room> previousColumnRooms = new();

        //列循环
        for (int column = 0; column < mapConfig.roomBluePrints.Count ; column ++)
        {
            var blueprint = mapConfig.roomBluePrints[column];

            var amount = Random.Range(blueprint.min,blueprint.max);

            var startHeight = screenHeight / 2 - screenHeight / (amount +1);

            generatePoint = new Vector3(-screenWidth/2 + border + columnWidth * column, startHeight , z : 0);

            var newPosition = generatePoint;

            //创建当前列的房间列表
            List<Room> currentColumnRooms = new();

            //当前列内的房间Y的间隔
            var roomGapY = screenHeight / (amount + 1);

            //列内行循环生成具体房间
            for(int i = 0; i < amount; i ++)
            {
                //判断是不是最后一个列，如果是则不作偏移
                if(column == mapConfig.roomBluePrints.Count - 1)
                {
                    newPosition.x = screenWidth / 2 - border * 2;
                }
                else if(column != 0)
                {
                    newPosition.x = generatePoint.x + Random.Range(-columnWidth/3,columnWidth/3);
                }

                newPosition.y = startHeight - roomGapY * i;
                //生成房间
                var room = Instantiate(roomPrefab, newPosition ,Quaternion.identity, transform);

                //先通过本地方法随机一个Type
                RoomType newType = GetRandomRoomType(mapConfig.roomBluePrints[column].roomType);

                //设置房间的初始状态
                if(column == 0)
                    room.roomState = RoomState.Active;
                else
                    room.roomState = RoomState.Locked;

                //然后根据Type索引房间DataSO，设置初始化
                room.SetUpRoom(column, i, GetRoomData(newType));

                rooms.Add(room);
                currentColumnRooms.Add(room);
            }

            //判断是否当前时第一列，如果不是则链接上一列
            if(previousColumnRooms.Count > 0)
            {
                //创建Line
                CreateConnections(previousColumnRooms, currentColumnRooms);
            }
            previousColumnRooms = currentColumnRooms;
        }

        //创建完成之后保存地图
        SaveMap();
    }

    private void CreateConnections(List<Room> column1, List<Room> column2)
    {
        //HashSet是不允许有重复项的列表
        HashSet<Room> connectedColumn2Rooms = new();

        foreach (var room in column1)
        {
            //使用自建方法获取一个目标房间
            var targetRoom = ConnectToRandomRoom(room, column2,false);
            connectedColumn2Rooms.Add(targetRoom);
        }

        //然后反过来检查第二列中的所有房间是否有连线了，如果有没有连线的房间，则再反向连线
        foreach(var room in column2)
        {
            if(!connectedColumn2Rooms.Contains(room))
            {
                ConnectToRandomRoom(room, column1,true);
            }
        }
    }

    private Room ConnectToRandomRoom(Room room, List<Room> column2,bool check)
    {
        Room targetRoom;

        targetRoom = column2[Random.Range(0, column2.Count)];

        if(check)
        {
            targetRoom.linkTo.Add(new(room.column, room.row));
        }
        else
        {
            room.linkTo.Add(new(targetRoom.column, targetRoom.row));
        }

        //创建不同房间之间的连线
        var line = Instantiate(linePrefab, transform);
        line.SetPosition(0, room.transform.position);
        line.SetPosition(1, targetRoom.transform.position);
        lines.Add(line);
        return targetRoom;
    }

    //重新生成地图
    [ContextMenu("ReGenerateRoom")]
    public void ReGenerateRoom()
    {
        foreach(var room in rooms)
        {
            Destroy(room.gameObject);
        }

        foreach(var line in lines)
        {
            Destroy(line.gameObject);
        }

        rooms.Clear();
        lines.Clear();
        CreateMap();
    }

    private RoomDataSO GetRoomData(RoomType roomType)
    {
        return roomDataDict[roomType];
    }

    private RoomType GetRandomRoomType(RoomType flags)
    {
        string[] options = flags.ToString().Split(',');

        string randomOption = options[Random.Range(0, options.Length)];

        RoomType roomType = (RoomType)Enum.Parse(typeof(RoomType),randomOption);

        return roomType;
    }

    #region 地图加载和保存
    private void SaveMap()
    {
        mapLayout.mapRoomDataList = new();

        //添加房间数据
        for (int i = 0; i < rooms.Count; i ++)
        {
            //创建数据
            var room = new MapRoomData()
            {
                posX = rooms[i].transform.position.x,
                posY = rooms[i].transform.position.y,
                colum = rooms[i].column,
                line = rooms[i].row,
                roomData = rooms[i].roomData,
                roomState = rooms[i].roomState,
                linkTo = rooms[i].linkTo,
            };
            //添加到SO列表中
            mapLayout.mapRoomDataList.Add(room);
        }

        mapLayout.linePositionsList = new();
        //添加连线数据
        for (int i = 0; i < lines.Count; i ++)
        {
            var linePosition = new LinePosition()
            {
                startPos = new SerializeVector3(lines[i].GetPosition(0)),
                endPos = new SerializeVector3(lines[i].GetPosition(1))
            };
            mapLayout.linePositionsList.Add(linePosition);
        }
    }

    private void LoadMap()
    {
        //读取房间数据
        for (int i = 0; i < mapLayout.mapRoomDataList.Count; i ++)
        {
            var newPos = new Vector3(mapLayout.mapRoomDataList[i].posX, mapLayout.mapRoomDataList[i].posY, 0);
            var newRoom = Instantiate(roomPrefab, newPos, Quaternion.identity, transform);
            newRoom.roomState = mapLayout.mapRoomDataList[i].roomState;
            newRoom.SetUpRoom(mapLayout.mapRoomDataList[i].colum, mapLayout.mapRoomDataList[i].line,mapLayout.mapRoomDataList[i].roomData);
            newRoom.linkTo = mapLayout.mapRoomDataList[i].linkTo;
            rooms.Add(newRoom);
        }

        //读取连线
        for (int i = 0; i < mapLayout.linePositionsList.Count; i++)
        {
            var line = Instantiate(linePrefab,transform);
            line.SetPosition(0,mapLayout.linePositionsList[i].startPos.ToVector3());
            line.SetPosition(1,mapLayout.linePositionsList[i].endPos.ToVector3());

            lines.Add(line);
        }
    }
    #endregion
}
