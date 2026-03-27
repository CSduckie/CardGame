using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemLibrarySO", menuName = "Item/ItemLibrarySO")]
public class ItemLibrarySO : ScriptableObject
{
    public List<ItemLibraryEntry> itemLibraryList;
}

[System.Serializable]
public struct ItemLibraryEntry
{
    public ItemDataSO itemData;
}