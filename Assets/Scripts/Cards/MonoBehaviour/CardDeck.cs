using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using DG.Tweening;

public class CardDeck : MonoBehaviour
{
    public CardManager cardManager;
    public CardLayoutManager layoutManager;
    public List<CardDataSO> drawDeck = new(); //抽牌堆
    private List<CardDataSO> discardDeck = new();   //弃牌堆

    private List<Card> handCardObjectList = new(); //每回合手牌

    public Vector3 cardDeckPosition = new(0, 0, 0);

    //是否还有待抽的牌
    public bool hasMoreCardToDraw = true;

    //TODO:测试用，后续删除
    private void Start()
    {
        InitializeDeck();
    }

    public void InitializeDeck()
    {
        //先清空抽卡堆
        drawDeck.Clear();
        foreach(var entry in cardManager.currentLibrary.cardLibraryList)
        {
            for (int i = 0; i < entry.amount;i++)
            {
                drawDeck.Add(entry.cardData);
            }
        }

        //TODO :洗牌，更新抽牌堆和弃牌堆数字
        ShuffleDeck();
    }
    [ContextMenu("DrawCard")]
    public void TestDrawCard()
    {
        DrawCard(1);
    }

    public void OnNewTurnDrawCard()
    {
        DrawCard(2);
    }

    private void DrawCard(int amount)
    {
        if(GameManager.Instance.isGameFailed) return;
        int drawAmount;
        //如果抽牌堆为空，则不抽牌，且
        if(drawDeck.Count == 0)
        {
            if(!hasMoreCardToDraw)
            {
                hasMoreCardToDraw = false;
                //启动事件告诉GameManager没有待抽的牌了
                //于GameManager中调用
            }
            Debug.Log("No more card to draw");
            return;
        }


        if(amount > drawDeck.Count)
        {
            drawAmount = drawDeck.Count;
        }
        else
        {
            drawAmount = amount;
        }

        for (int i = 0; i < drawAmount; i++)
        {
            if (drawDeck.Count == 0)
            {
                //TODO: 洗牌
                foreach(var item in discardDeck)
                {
                    drawDeck.Add(item);
                }
                ShuffleDeck();
            }
            //拿出牌面抽牌堆牌面最上方一张卡
            CardDataSO currentCardData = drawDeck[0];
            drawDeck.RemoveAt(0);

            var card = cardManager.GetCardObject().GetComponent<Card>();

            //初始化
            card.Init(currentCardData);
            card.transform.position = cardDeckPosition;

            handCardObjectList.Add(card);
            var delay = i * 0.2f;
            SetCardLayout(delay);
        }
    }

    //设置卡片布局
    private void SetCardLayout(float _delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i ++)
        {
            Card currentCard = handCardObjectList[i];
            if(!currentCard.isPlaced)
            {
                CardTransform cardTransform = layoutManager.GetCardTransform(i, handCardObjectList.Count);

                currentCard.isAnimating = true;

                currentCard.transform.DOScale(Vector3.one, 0.2f).SetDelay(_delay).onComplete = () =>
                {
                    currentCard.transform.DOMove(cardTransform.pos, 0.5f).onComplete = () =>currentCard.isAnimating = false;
                    currentCard.transform.DORotateQuaternion(cardTransform.rotation, 0.5f);
                };


                //设置卡片层级
                currentCard.GetComponent<SortingGroup>().sortingOrder = i;
                currentCard.SaveOriginalData(cardTransform.pos, cardTransform.rotation);
            }
        }
    }
    /// <summary>
    /// 洗牌
    /// </summary>
    private void ShuffleDeck()
    {
        discardDeck.Clear();

        //TODO:更新UI 

        for(int i = 0; i < drawDeck.Count; i++)
        {
            CardDataSO temp = drawDeck[i];
            int randomIndex = Random.Range(i, drawDeck.Count);
            drawDeck[i] = drawDeck[randomIndex];
            drawDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 弃牌
    /// </summary>
    public void DiscardCard(object obj)
    {
        Card card = obj as Card;
        handCardObjectList.Remove(card);
        //cardManager.DiscardCard(card.gameObject);
        card.gameObject.SetActive(true);
        SetCardLayout(0);
        //TODO:更新UI
    }

    /// <summary>
    /// 弃掉所有的手牌，并返回抽牌堆
    /// 用于游戏结束时调用
    /// </summary>
    public void ReleaseAllCards(object obj)
    {
        foreach(var card in handCardObjectList)
        {
            cardManager.DiscardCard(card.gameObject);
        }
        handCardObjectList.Clear();
        hasMoreCardToDraw = true;
        InitializeDeck();
    }


}
