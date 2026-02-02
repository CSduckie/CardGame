using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardManager : MonoBehaviour
{
    public PoolTool poolTool;
    public List<CardDataSO> cardDataList; //存储游戏中所有可能出现的卡牌

    [Header("卡牌库")]
    public CardLibrarySO newGameCardLibrary;//新游戏时玩家的牌库
    public CardLibrarySO currentLibrary;//游戏进行中时玩家当前牌库

    private int previousCardIndex;
    private void Awake()
    {
        InitializeAllCardDataToList();

        //根据游戏内容调整，后续需要创建一个new Game方法塞入其中
        foreach(var item in newGameCardLibrary.cardLibraryList)
        {
            currentLibrary.cardLibraryList.Add(item);
        }
    }

    private void OnDisable()
    {
        currentLibrary.cardLibraryList.Clear();
    }

    #region 获取项目资源内所有卡牌
    //程序添加资源内所有卡牌的方法
    private void InitializeAllCardDataToList()
    {
        Addressables.LoadAssetsAsync<CardDataSO>("CardData", null).Completed += OnCardDataLoaded;
    }

    private void OnCardDataLoaded(AsyncOperationHandle<IList<CardDataSO>> handle)
    {
        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            cardDataList = new List<CardDataSO>(handle.Result);
        }
        else
        {
            Debug.LogError("No Card Found!");
        }
    }
    #endregion

    /// <summary>
    /// 抽卡时调用函数让卡牌的GameObject设置为0scale
    /// </summary>
    /// <returns></returns>

    public GameObject GetCardObject()
    {
        var cardObj = poolTool.GetObjectFromPool();
        cardObj.transform.localScale = Vector3.zero;
        return cardObj;
    }

    public void DiscardCard(GameObject cardObj)
    {
        poolTool.ReturnObjectToPool(cardObj);
    }

    /// <summary>
    /// 获取可能作为奖励出现的卡牌
    /// </summary>
    public CardDataSO GetNewCardData()
    {
        var randomIndex = 0;
        do
        {
            randomIndex = Random.Range(0, cardDataList.Count);
        }while(previousCardIndex == randomIndex);
        previousCardIndex = randomIndex;
        return cardDataList[randomIndex];
    }

    //玩家获得新卡牌时调用函数将卡牌添加到牌库
    public void AddNewCardToLibrary(CardDataSO _cardData)
    {
        var newCard = new CardLibraryEntry()
        {
            cardData = _cardData,
            amount = 1
        };
        if(currentLibrary.cardLibraryList.Contains(newCard))
        {
            var target = currentLibrary.cardLibraryList.Find(t => t.cardData == _cardData);
            target.amount++;
        }
        else
        {
            currentLibrary.cardLibraryList.Add(newCard);
        }
    }
}
