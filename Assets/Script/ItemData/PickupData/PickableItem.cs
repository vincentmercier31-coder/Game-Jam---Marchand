using UnityEngine;

[CreateAssetMenu(fileName = "PickableItem", menuName = "Scriptable Objects/PickableItem")]
public class PickableItem : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;


    public ItemType itemType;
    
    
    public enum ItemType
    {
        Grapes,
        EmptyWineBottle,
        Piquette,
        Clairet,
        Chateau,
        TurnedWine
    }
}
