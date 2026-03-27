using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardTemplet : MonoBehaviour
{
    [Header("UI组件")]
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI cardAttack;
    public TextMeshProUGUI cardTypeText;
    public Button cardButton;
    public CardDataSO currentCardData;
    public void InitCard(CardDataSO _cardData)
    {
        cardButton.interactable = true;
        cardImage.sprite = _cardData.cardImage;
        cardName.text = _cardData.cardName;
        cardTypeText.text = _cardData.cardType.ToString();
        cardAttack.text = _cardData.Attack.ToString();
        cardDescription.text = _cardData.description;
        currentCardData = _cardData;
    }

    public void OnCardClicked()
    {
        // Debug.Log("OnCardClicked: " + currentCardData.cardName);
        cardButton.interactable = false;
        //先查看是否是奖励房间，如果是，则调用RewardRoomPanel的OnCardClicked方法
        if(GetComponentInParent<RewardRoomPanel>() != null)
        {
            GetComponentInParent<RewardRoomPanel>().OnRewardSelected(gameObject);
        }
        else
        {
            GetComponentInParent<PickCardPanel>().OnCardClicked(cardButton, currentCardData);
        }
    }
}
