using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable
{
    public string itemName = "Objet";
    public PickableItem itemData;

    public void Interact()
    {
        PlayerInteraction player = FindFirstObjectByType<PlayerInteraction>();

        if (player == null)
            return;

        player.AddItem(itemData);

        Debug.Log("Ramassé : " + itemName);

        Destroy(gameObject);
    }

    public void Start()
    {
        //Debug.Log("");
        Instantiate(itemData.itemPrefab, transform);
    }
   
}