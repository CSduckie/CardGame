using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RewardRoomPanel : MonoBehaviour
{
    //在Reward场景中，当玩家进入奖励房间时，会显示奖励房间面板
    //奖励面板包含三个slot，分别对应三种奖励，奖励类型随机
    //奖励类型包括：卡牌 = 0，最大卡牌栏位 = 1，被动技能（类似崩铁模拟宇宙的方程） = 2
    public CardManager cardManager;
    [Header("奖励槽位")]
    public GameObject rewardContainer;

    [Header("奖励预制体")]
    //卡牌奖励预制体
    public CardTemplet cardRewardPrefab;
    //道具奖励预制体
    public ItemTemplet itemRewardPrefab;
    public GameObject maxCardSlotRewardPrefab;

    private List<Button> rewardButtonList = new();
    private GameObject currentRewardSelected;
    private void OnEnable()
    {
        CreateReward();
    }

    void CreateReward()
    {
        for(int i = 0; i < rewardContainer.transform.childCount; i++)
        {
            //TODO: 后续需要优化，现在只是测试，所以直接设置为1
            int rewardType = 2;
            switch(rewardType)
            {
                case 0:
                    var card = Instantiate(cardRewardPrefab, rewardContainer.transform.GetChild(i).position, Quaternion.identity);
                    //随机一个卡牌
                    CardDataSO newData = cardManager.GetNewCardData();
                    card.InitCard(newData);
                    rewardButtonList.Add(card.cardButton);
                    card.GetComponent<Transform>().SetParent(rewardContainer.transform.GetChild(i));
                    card.GetComponent<Transform>().localPosition = Vector3.zero;
                    card.GetComponent<Transform>().localScale = Vector3.one;
                    break;
                case 1:
                    Debug.Log("最大卡牌栏位奖励");
                    break;
                case 2:
                    Debug.Log("道具获得奖励");
                    //道具的作用是：给玩家提供一些被动效果，例如：如果玩家有卡牌死亡
                    var item = Instantiate(itemRewardPrefab, rewardContainer.transform.GetChild(i).position, Quaternion.identity);
                    //TODO:随机一个道具作用
                    rewardButtonList.Add(item.itemButton);
                    item.GetComponent<Transform>().SetParent(rewardContainer.transform.GetChild(i));
                    item.GetComponent<Transform>().localPosition = Vector3.zero;
                    item.GetComponent<Transform>().localScale = Vector3.one;
                    break;
                default:
                    break;
            }
        }
    }

    public void OnRewardSelected(GameObject _rewardSelected)
    {
        currentRewardSelected = _rewardSelected;
        for(int i = 0; i < rewardButtonList.Count; i++)
        {
            if(rewardButtonList[i] == _rewardSelected.GetComponentInChildren<Button>())
                rewardButtonList[i].interactable = false;
            else
                rewardButtonList[i].interactable = true;
        }
    }

    //TODO:添加确定按扭
}
