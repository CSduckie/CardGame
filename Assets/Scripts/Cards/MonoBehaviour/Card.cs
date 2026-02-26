using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardDataSO cardData;
    public CardType cardType;
    public bool isMultiply;
    public SpriteRenderer cardSprite;
    public SpriteRenderer cardBack;
    public TextMeshPro attackText, multiplyText, cardName, cardDescription,typeText;

    private bool isDragging = false;

    [Header("卡牌原始坐标数据")]
    public Vector3 originalPosition{get; private set;}
    private Quaternion originalRotation;
    private int originalLayerOrder;
    public Sequence currentSequence;
    public bool isAnimating = false;
    public bool isPlaced = false;

    [Header("卡牌是否被眩晕")]
    public bool isFreeze = false;
    public bool isPoison = false;
    public bool onTower = false;

    //卡牌是否处于不受任何modifier影响状态
    public bool isIgnoreModifier = false;
    public bool isIgnoreSlotEffect = false;

    [Header("事件")]
    public ObjectEventSO discardCardEvent;
    //卡牌死亡事件
    public ObjectEventSO cardDestroyEvent;

    [Header("卡牌临时数值")]
    public int cardAttackModifier = 0;
    private void Start()
    {
        GetComponent<SortingGroup>().sortingOrder = 10;
        Init(cardData);
    }

    public void Init(CardDataSO _cardData)
    {
        cardData = _cardData;
        cardName.text = _cardData.cardName;
        cardSprite.sprite = _cardData.cardImage;
        attackText.text = _cardData.Attack.ToString();
        cardDescription.text = _cardData.description;
        cardType = _cardData.cardType;
        isMultiply = _cardData.isMultiply;
        multiplyText.text = isMultiply? "X" : "+";
        cardAttackModifier = 0;
        isIgnoreSlotEffect = false;
        isIgnoreModifier = false;
        typeText.text = _cardData.cardType switch
        {
            CardType.Soldier => "Soldier",
            CardType.Skill => "Skill",
            _ => throw new System.NotImplementedException(),
        };
    }

    private void Update()
    {
        if(isDragging && !isPlaced) 
        {
            transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        }
    }


    public void SaveOriginalData(Vector3 _position, Quaternion _rotation)
    {
        originalPosition = _position;
        originalRotation = _rotation;
        originalLayerOrder = GetComponent<SortingGroup>().sortingOrder;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isAnimating) return;
        if(isPlaced) return;
        if(PlayerHandManager.instance.isDraggingCard) return;
        //Debug.Log("OnPointerEnter");
        //动画序列
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        currentSequence.Join(transform.DOMove(originalPosition + Vector3.up * 0.3f, 0.1f));
        currentSequence.Join(transform.DORotate(Quaternion.identity.eulerAngles, 0.1f));
        currentSequence.Play();
        currentSequence.onComplete += () => {
            currentSequence.Kill();
        };
        //transform.position = originalPosition + Vector3.up * 0.5f;
        //transform.rotation = Quaternion.identity;
        GetComponent<SortingGroup>().sortingOrder = 20;
    }

    

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isAnimating) return;
        if(isPlaced) return;
        if(PlayerHandManager.instance.isDraggingCard) return;
        //Debug.Log("OnPointerExit");
        ResetCardTransform();
    }

    #region 卡牌的动画
    //卡牌的动画
    private void OnMouseDown() 
    {
        if(isAnimating) return;
        if(isPlaced) return;
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        currentSequence.Join(transform.DOScale(Vector3.one * 0.8f, 0.1f));
        currentSequence.Play();
        currentSequence.onComplete += () => {
            currentSequence.Kill();
        };
    }
    private void OnMouseUp() 
    {
        if(isAnimating) return;
        if(isPlaced) return;

        ResetCardTransform();
    }
    #endregion

    public void ResetCardTransform()
    {
        //Debug.Log("ResetCardTransform");
        isAnimating = true;
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        currentSequence.Join(transform.DOScale(Vector3.one, 0.1f));
        currentSequence.Join(transform.DOMove(originalPosition, 0.1f));
        currentSequence.Join(transform.DORotate(originalRotation.eulerAngles, 0.1f));
        GetComponent<SortingGroup>().sortingOrder = originalLayerOrder;
        currentSequence.Play();
        currentSequence.onComplete += () => {
            currentSequence.Kill();
            isAnimating = false;
        };
    }

    //每个卡牌监听的事件，
    public void MoveRight()
    {
        if(!isPlaced) return;
        if(isFreeze) 
        {
            Debug.Log("卡牌被冻结，不能移动");
            //启动一个延迟携程，清除冻结效果
            StartCoroutine(ClearFreezeEffect());
            IEnumerator ClearFreezeEffect()
            {
                yield return new WaitForSeconds(1f);
                isFreeze = false;
            }
            return;
        }
        GameBoardController gameBoard = GetComponentInParent<GameBoardController>();
        int myRow = transform.parent.GetComponent<SlotController>().Raw;
        int myColumn = transform.parent.GetComponent<SlotController>().Column;
        if(gameBoard.isRightHaveObject(myRow, myColumn))
        {
            Debug.Log("isRightHaveObject");
            var currentSlot = transform.parent.GetComponent<SlotController>();
            currentSlot.currentCard = null;
            currentSlot.isEmpty = true;
            //检查是处于最后一列，还是右侧有实际卡牌
            if(myColumn == gameBoard.column)
            {
                DestroyCard(this);
            }
            else
            {
                //如果是右侧有别的卡牌，那么删除那个地方的卡牌，然后执行移动
                var targetSlot = gameBoard.transform.GetChild((myRow-1) * gameBoard.column + myColumn).GetComponent<SlotController>();
                Card targetCard = targetSlot.currentCard;
                DestroyCard(targetCard);
                gameBoard.MoveCardToRight(myRow, myColumn,this);
            }
        }
        else
        {
            gameBoard.MoveCardToRight(myRow, myColumn,this);
        }
        //TODO:更新UI
    }


    ///卡牌放置效果
    public void CardPlacedEffect(Card _card)
    {
        switch(_card.cardType)
        {
            case CardType.Soldier:
                //TODO:卡牌放置效果,
                discardCardEvent.RaisEvent(this, this);
                foreach(var effect in cardData.effects)
                {
                    effect.Execute(this);
                }
                //更新UI
                GameManager.Instance.gamePlayPanel.UpdateDamageUI();
                GameBoardController gameBoard = GetComponentInParent<GameBoardController>();
                gameBoard.UpdateEnemyPredictHealth();
                break;
            case CardType.Skill:
                discardCardEvent.RaisEvent(this, this);
                foreach(var effect in cardData.effects)
                {
                    effect.Execute(this);
                    //UI已经在Skill effect中更新了
                }
                break;
        }
    }

    public void CardOnTurnEndEffect(Card _card)
    {
        if(_card.cardData.cardType != CardType.Soldier) return;

        foreach(var effect in cardData.effects)
        {
            effect.ExecuteOnTurnEnd(_card);
        }
    }

    //卡牌处于特殊地格的效果
    public void CardOnSpecialGridEffect(SlotController _targetSlot)
    {
        switch(_targetSlot.specialGridType)
        {
            case SpecialGridType.Cold:
                if(!isIgnoreSlotEffect)
                {
                    isFreeze = true;
                }
                break;
            case SpecialGridType.Tower:
                //在塔上，伤害+2
                if(!isIgnoreSlotEffect)
                {
                    attackText.text = (int.Parse(attackText.text) + 2).ToString();
                    onTower = true;
                }
                break;
            case SpecialGridType.Posion:
                //中毒时，伤害/2
                if(!isIgnoreSlotEffect)
                {
                    attackText.text = (int.Parse(attackText.text) / 2).ToString();
                    isPoison = true;
                }
                break;
            case SpecialGridType.Trap:
                //陷阱，清除卡牌，然后地格变为None。
                if(!isIgnoreSlotEffect)
                {
                    Debug.Log("陷阱，清除卡牌，然后地格变为None。");
                    //传入TargetSlot
                    StartCoroutine(ClearSpecialGridEffect(_targetSlot));
                    //启动一个携程，让卡牌延迟1秒销毁，确保DOTween动画播放完毕
                    //TODO: 这里需要优化，添加一个小小的动画效果
                    StartCoroutine(DestroyCardWithDelay(this,1f));
                }
                break;
            case SpecialGridType.None:
                break;
        }
    }

    //清除特殊地格的延迟携程
    private IEnumerator ClearSpecialGridEffect(SlotController _targetSlot)
    {
        yield return new WaitForSeconds(1f);
        _targetSlot.specialGridType = SpecialGridType.None;
        _targetSlot.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0f);
    }

    
    //使用带有延迟的卡牌销毁
    private IEnumerator DestroyCardWithDelay(Card _card,float _delay)
    {
        yield return new WaitForSeconds(_delay);
        DestroyCard(_card);
    }

    //卡牌死亡
    public void DestroyCard(Card _card)
    {
        Debug.Log("DestroyCard: " + _card.cardData.cardName);

        //清除内部所有卡牌的携程
        StopAllCoroutines();

        //执行卡牌死亡效果
        foreach(var effect in _card.cardData.effects)
        {
            effect.ExecuteOnDestroy(this);
        }
        //触发其他卡牌死亡事件
        //使用一个新列表来获取其他卡牌
        List<Card> otherCards = new List<Card>();
        foreach(var card in GameManager.Instance.gameBoardController.transform.GetComponentsInChildren<Card>())
        {
            if(card!=null && card!= _card)
            {
                otherCards.Add(card);
            }
        }
        foreach(var card in otherCards)
        {
            foreach(var effect in card.cardData.effects)
            {
                effect.ExecuteOnOtherCardsDie(card);
            }
        }
        otherCards.Clear();
        var cardCurrentSlot = _card.GetComponentInParent<SlotController>();
        cardCurrentSlot.currentCard = null;
        cardCurrentSlot.isEmpty = true;
        Destroy(_card.gameObject);
    }
}
