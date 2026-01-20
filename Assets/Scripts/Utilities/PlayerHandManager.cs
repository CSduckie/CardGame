using UnityEngine;

public class PlayerHandManager : MonoBehaviour
{
    public static PlayerHandManager instance;
    public bool isDraggingCard;
    public bool hasSlot;
    public SlotController currentHoveredSlot;
    
    private void Awake()
    {
        instance = this;
    }

}
