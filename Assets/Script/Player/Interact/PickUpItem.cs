using UnityEngine;

public class PickupInteractable : MonoBehaviour, IInteractable
{
    public string itemName = "Objet";

    public string GetInteractionText()
    {
        return "Ramasser " + itemName;
    }

    public void Interact()
    {
        Debug.Log("Ramassé : " + itemName);

        Destroy(gameObject);
    }
}