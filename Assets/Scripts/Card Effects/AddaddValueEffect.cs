using UnityEngine;

[CreateAssetMenu(fileName = "AddaddValueEffect", menuName = "Card Effects/AddaddValueEffect")]
public class AddaddValueEffect : Effect
{
    public override void Execute(Card from)
    {
        GameBoardController gameBoard = FindObjectOfType<GameBoardController>();
        gameBoard.UpdateTempDamage(false, value);
        GameManager.Instance.gamePlayPanel.UpdateDamageUI();
        //更新伤害预测
        gameBoard.UpdateEnemyPredictHealth();
        Destroy(from.gameObject);
    }
}
