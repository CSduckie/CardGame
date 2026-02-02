using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PickCardPanel : MonoBehaviour
{
    public CardManager cardManager;
    [Header("卡牌容器")]
    public GameObject cardContainer;
    [Header("卡牌预制体")]
    public CardTemplet cardPrefab;
    private CardDataSO currentCardData;
    private List<Button> cardButtonList = new();
    private void OnEnable()
    {
        for(int i = 0; i < cardContainer.transform.childCount; i++)
        {
            var card = Instantiate(cardPrefab, cardContainer.transform.GetChild(i).position, Quaternion.identity);
            CardDataSO newData = cardManager.GetNewCardData();
            card.InitCard(newData);
            cardButtonList.Add(card.cardButton);
            card.GetComponent<Transform>().SetParent(cardContainer.transform.GetChild(i));
            card.GetComponent<Transform>().localPosition = Vector3.zero;
            card.GetComponent<Transform>().localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 卡牌被点击时调用函数
    /// </summary>
    /// <param name="cardButton"></param>
    /// <param name="cardData"></param>
    public void OnCardClicked(Button cardButton, CardDataSO cardData)
    {
        currentCardData = cardData;
        for(int i = 0; i < cardButtonList.Count; i++)
        {
            if(cardButtonList[i] == cardButton)
                cardButtonList[i].interactable = false;
            else
                cardButtonList[i].interactable = true;
        }
    }

    public void OnConfirmButtonClicked()
    {
        cardManager.AddNewCardToLibrary(currentCardData);
        //调用GameManager，启动gamewinpanel，关闭当前panel，同时禁用选择卡牌按钮
        GameManager.Instance.gameWinPanel.SetActive(true);
        GameManager.Instance.gameWinPanel.GetComponent<GameWinPanel>().SelectCardButton.interactable = false;
        gameObject.SetActive(false);
    }
}
