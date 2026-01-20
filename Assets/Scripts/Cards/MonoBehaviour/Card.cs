using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using DG.Tweening;
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardDataSO cardData;
    public CardType cardType;
    public bool isMultiply;
    public SpriteRenderer cardSprite;
    public TextMeshPro attackText, healthText, costText, cardName, cardDescription,typeText;

    private bool isDragging = false;

    [Header("卡牌原始坐标数据")]
    public Vector3 originalPosition{get; private set;}
    private Quaternion originalRotation;
    private int originalLayerOrder;
    public Sequence currentSequence;
    public bool isAnimating = false;
    public bool isPlaced = false;
    [Header("卡牌是否被眩晕")]
    public bool isStun = false;


    [Header("事件")]
    public ObjectEventSO discardCardEvent;

    private void Start()
    {
        Init(cardData);
        GetComponent<SortingGroup>().sortingOrder = 1;
    }

    public void Init(CardDataSO _cardData)
    {
        cardData = _cardData;
        cardName.text = _cardData.cardName;
        cardSprite.sprite = _cardData.cardImage;
        attackText.text = _cardData.Attack.ToString();
        healthText.text = _cardData.health.ToString();
        costText.text = _cardData.cost.ToString();
        cardDescription.text = _cardData.description;
        cardType = _cardData.cardType;
        isMultiply = _cardData.isMultiply;
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
        Debug.Log("OnPointerEnter");
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
        Debug.Log("OnPointerExit");
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
            Debug.Log("OnMouseDown Complete");
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
        Debug.Log("ResetCardTransform");
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
        GameBoardController gameBoard = GetComponentInParent<GameBoardController>();
        int myRow = transform.parent.GetComponent<SlotController>().Raw;
        int myColumn = transform.parent.GetComponent<SlotController>().Column;
        if(gameBoard.isRightHaveObject(myRow, myColumn))
        {
            var currentSlot = transform.parent.GetComponent<SlotController>();
            currentSlot.currentCard = null;
            currentSlot.isEmpty = true;
            Destroy(gameObject);
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
}
