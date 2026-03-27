using UnityEngine;
using UnityEngine.UI;

public class RewardRoomControllers : MonoBehaviour
{

    //在Reward场景中，当玩家进入奖励房间时，会显示奖励房间面板
    private void Start()
    {
        GameManager.Instance.ActiveRewardRoomUI();
    }
}
