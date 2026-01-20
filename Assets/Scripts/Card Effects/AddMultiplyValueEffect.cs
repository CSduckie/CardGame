using UnityEngine;

[CreateAssetMenu(fileName = "AddMultiplyValueEffect", menuName = "Card Effects/AddMultiplyValueEffect")]
public class AddMultiplyValueEffect : Effect
{
    public override void Execute(Card from)
    {
        GameBoardController gameBoard = FindFirstObjectByType<GameBoardController>();
        gameBoard.UpdateTempDamage(true, value);
        GameManager.Instance.gamePlayPanel.UpdateDamageUI();
        //更新伤害预测
        gameBoard.UpdateEnemyPredictHealth();
        Destroy(from.gameObject);
    }
}
