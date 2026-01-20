using UnityEngine;
using System.Collections.Generic;

public class CardLayoutManager : MonoBehaviour
{
    public bool isHorizontal;
    public float maxWidth = 7f;
    public float cardSpacing = 2f;

    public List<Vector3> cardPos = new();
    private List<Quaternion> cardRotation = new();


    [Header("卡牌弧形布局")]
    public float angleBetweenCards = 7f;
    public float radius = 3f;
    public Vector3 centerPoint;


    private void Awake()
    {
        centerPoint = isHorizontal ? Vector3.up * -4.5f : Vector3.up * -9f;
    }


    public CardTransform GetCardTransform(int index,int totalCards)
    {
        CalculatePosition(totalCards, isHorizontal);
        return new CardTransform(cardPos[index], cardRotation[index]);
    }

    private void CalculatePosition(int numberOfCards,bool isHorizontal)
    {
        cardRotation.Clear();
        cardPos.Clear();
        if(isHorizontal)
        {
            float currentWidth = cardSpacing * (numberOfCards - 1);
            float totalWidth = Mathf.Min(currentWidth, maxWidth);
            //如果多张牌，返回前面的值，如果只有一张牌，返回0
            float currentSpacing = totalWidth > 0 ? totalWidth / (numberOfCards - 1) : 0;

            for (int i = 0; i < numberOfCards;i++)
            {
                float xPos = 0 - (totalWidth/2) + (i * currentSpacing);
                var pos = new Vector3(xPos, centerPoint.y, 0);
                var roation = Quaternion.identity;
                cardPos.Add(pos);
                cardRotation.Add(roation);
            }
        }
        else
        {
            float angle = angleBetweenCards * (numberOfCards - 1) / 2;
            for (int i = 0; i < numberOfCards;i++)
            {
                var pos = FindCardPosition(angle - i * angleBetweenCards);

                var rotation = Quaternion.Euler(0, 0, angle - i * angleBetweenCards);

                cardPos.Add(pos);
                cardRotation.Add(rotation);
            }
        }
    }

    private Vector3 FindCardPosition(float angle)
    {
        return new Vector3(
            centerPoint.x - Mathf.Sin(Mathf.Deg2Rad * angle) * radius,
            centerPoint.y + Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
            0
        );
    }
}
