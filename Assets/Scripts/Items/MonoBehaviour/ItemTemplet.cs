using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemTemplet : MonoBehaviour
{
    public Button itemButton;
    public ItemDataSO currentItemData;
    public TextMeshProUGUI itemName;


    public void InitItem(ItemDataSO _itemData)
    {
        currentItemData = _itemData;
        itemName.text = _itemData.itemName;
    }

    public void OnItemClicked()
    {
        // Debug.Log("OnCardClicked: " + currentCardData.cardName);
        itemButton.interactable = false;
        //先查看是否是奖励房间，如果是，则调用RewardRoomPanel的OnCardClicked方法
        if(GetComponentInParent<RewardRoomPanel>() != null)
        {
            GetComponentInParent<RewardRoomPanel>().OnRewardSelected(gameObject);
        }
    }
}
