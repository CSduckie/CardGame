using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
public class SceneLoadManager : MonoBehaviour
{
    AssetReference currentScene;
    public AssetReference map;
    public AssetReference startMenu;

    //当前房间地图坐标
    private Vector2Int currentRoomVector;

    [SerializeField]private Room currentRoom;

    [Header("事件广播")]
    public ObjectEventSO afterRoomLoadedEvent;
    public ObjectEventSO UpdateRoomEvent;


    private void Start()
    {
        currentRoomVector = Vector2Int.one * -1;
        LoadStartMenu();
    }
 
    /// <summary>
    /// 房间加载监听,传递Room对象
    /// </summary>
    /// <param name="data"></param>
    public async void OnLoadRoomEvent(object data)
    {
        if(data is Room)
        {
            currentRoom = data as Room;

            var currentRoomData = currentRoom.roomData;
            currentRoomVector = new(currentRoom.column, currentRoom.row);

            currentScene = currentRoomData.sceneToLoad;
        }
        //Debug.Log(currentRoomData.roomType);
        await UnloadSceneTask();
        //加载房间
        await LoadSceneTask();

        afterRoomLoadedEvent.RaisEvent(currentRoom, this);
    }

    //使用新方法执行加载场景，该方法允许等待若干秒后执行
    private async Awaitable LoadSceneTask()
    {

        var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
        //该指令可以返回该任务的执行状态
        await s.Task;
        //如果当前加载操作完全成功
        if(s.Status == AsyncOperationStatus.Succeeded)
        {
            //使用内置API激活当前场景
            SceneManager.SetActiveScene(s.Result.Scene);
        }
    }


    private async Awaitable UnloadSceneTask()
    {
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    public async void LoadMap()
    {
        await UnloadSceneTask();
        if(currentRoomVector != Vector2.one * -1)
        {
            UpdateRoomEvent.RaisEvent(currentRoomVector, this);
        }
        currentScene = map;
        await LoadSceneTask();
    }

    //加载开始菜单场景    
    public async void LoadStartMenu()
    {
        if(currentScene != null)
            await UnloadSceneTask();
        currentScene = startMenu;
        await LoadSceneTask();
    }
}
