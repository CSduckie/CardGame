using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public List<ItemDataSO> itemDataList; //存储游戏中所有可能出现的道具
    [Header("道具库")]
    public ItemLibrarySO newGameItemLibrary;//新游戏时玩家的道具库
    public ItemLibrarySO currentItemLibrary;//游戏进行中时玩家当前道具库

    private void Awake()
    {

    }

    private void InitializeAllItemDataToList()
    {

    }

    //玩家获得新卡牌时调用函数将道具添加到道具库
    public void AddNewCardToLibrary(ItemDataSO _itemData)
    {
        var newItem = new ItemLibraryEntry()
        {
            itemData = _itemData
        };
        if(!currentItemLibrary.itemLibraryList.Contains(newItem))
        {
            currentItemLibrary.itemLibraryList.Add(newItem);
        }
    }
}
