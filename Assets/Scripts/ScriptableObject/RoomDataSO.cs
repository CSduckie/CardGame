using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "RoomData", menuName = "Map/RoomDataSO")]
public class RoomDataSO : ScriptableObject
{
    public Sprite roomSprite;
    public RoomType roomType;
    public AssetReference sceneToLoad;
}
