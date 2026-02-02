using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Card currentCard;
    public bool canMove;
    private bool canUse;

    private void Awake()
    {
        currentCard = GetComponent<Card>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        switch(currentCard.cardData.cardType)
        {
            case CardType.Soldier:
            case CardType.Skill:
                if(currentCard.isPlaced) return;
                canMove = true;
                PlayerHandManager.instance.isDraggingCard = true;
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(canMove)
        {
            currentCard.isAnimating = true;
            Vector3 screenPos = new (Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPos);
            currentCard.transform.position = worldPosition;
            //PlayerHandManager.instance.isDraggingCard = true;
            //TODO:实现可以使用的判断逻辑
            canUse = worldPosition.y > 0;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        switch(currentCard.cardData.cardType)
        {
            case CardType.Soldier:
                // 检查是否有有效的 slot 可以放置
                if (PlayerHandManager.instance.hasSlot && 
                    PlayerHandManager.instance.currentHoveredSlot != null && 
                    PlayerHandManager.instance.currentHoveredSlot.isEmpty
                    && PlayerHandManager.instance.currentHoveredSlot.Column ==1)
                {
                    // 放置卡牌到 slot
                    PlayerHandManager.instance.currentHoveredSlot.PlaceCard(currentCard);
                    currentCard.isAnimating = false;
                    //Debug.Log("Place Card");

                    if(!currentCard.isPlaced)
                    {
                        currentCard.CardPlacedEffect(currentCard);
                        //Debug.Log("Card Placed Effect");
                    }
                    currentCard.isPlaced = true;
                    //启动事件告诉GameManager卡牌放置成功
                    //激活结束回合按钮
                    GameManager.Instance.gamePlayPanel.endTurnButton.interactable = true;
                }
                else
                {
                    if(currentCard.isPlaced) return;
                    // 没有有效的 slot，重置卡牌位置
                    currentCard.ResetCardTransform();
                    currentCard.isAnimating = false;
                }
                canMove = false;
                break;
            case CardType.Skill:
                //TODO:实现技能卡牌的使用逻辑
                //如果技能卡牌位移超过一定距离，则认为技能卡牌被使用
                if(Vector3.Distance(currentCard.transform.position, currentCard.originalPosition) > 2f)
                {
                    currentCard.CardPlacedEffect(currentCard);
                }
                else
                {
                    currentCard.ResetCardTransform();
                    currentCard.isAnimating = false;
                }
                canMove = false;
                break;
        }
        // 清除拖拽状态
        PlayerHandManager.instance.isDraggingCard = false;
        PlayerHandManager.instance.hasSlot = false;
        PlayerHandManager.instance.currentHoveredSlot = null;
    }
}
