using UnityEngine;
using UnityEngine.Rendering;
public class SlotController : MonoBehaviour
{
    [Header("slot信息")]
    public int value;
    public bool isMultiply = false;

    
    public bool isEmpty = true;
    public Card currentCard;
    
    [Header("slot自己的坐标位置")]
    public int Raw, Column;

    public void Init(int _raw, int _column)
    {
        Raw = _raw;
        Column = _column;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Card") && PlayerHandManager.instance.isDraggingCard)
        {
            if (isEmpty)
            {
                // 设置当前悬停的 slot
                PlayerHandManager.instance.currentHoveredSlot = this;
                PlayerHandManager.instance.hasSlot = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Card") && PlayerHandManager.instance.isDraggingCard)
        {
            if (isEmpty && PlayerHandManager.instance.currentHoveredSlot != this)
            {
                // 如果当前 slot 是空的，更新悬停的 slot
                PlayerHandManager.instance.currentHoveredSlot = this;
                PlayerHandManager.instance.hasSlot = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Card"))
        {
            // 如果离开的卡牌是当前悬停的 slot，清除引用
            if (PlayerHandManager.instance.currentHoveredSlot == this)
            {
                PlayerHandManager.instance.currentHoveredSlot = null;
                PlayerHandManager.instance.hasSlot = false;
            }
        }
    }

    public void PlaceCard(Card card)
    {
        if (isEmpty)
        {
            currentCard = card;
            //设置卡牌的位置，大小，父物体
            currentCard.transform.SetParent(transform);
            currentCard.transform.localPosition = Vector3.zero;
            currentCard.transform.localScale = Vector3.one;
            currentCard.GetComponent<SortingGroup>().sortingOrder = 1;
            isEmpty = false;
            Debug.Log($"Place Card at Slot ({Raw}, {Column})");
        }
    }
}
