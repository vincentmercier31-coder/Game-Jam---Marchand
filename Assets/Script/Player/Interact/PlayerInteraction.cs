using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Collider interactionCollider;

    private IInteractable currentInteractable;

    [SerializeField] private bool itemInHand;
    [SerializeField] private PickableItem itemDataInHand;

    // Prefab de base utilisé pour tous les objets posés au sol
    public GameObject pickedUpItemPrefab;

    private void Update()
    {
        FindInteractable();

        if (currentInteractable != null)
        {
            if (InputSystem.actions["Interact"].WasPressedThisFrame())
            {
                // Si c'est un pickup
                PickUpItem pickup = currentInteractable as PickUpItem;

                if (pickup != null)
                {
                    // Si le joueur a déjà un objet, on échange les deux
                    if (itemInHand)
                    {
                        SwapItem(pickup);
                    }
                    // Sinon on ramasse simplement l'objet
                    else
                    {
                        pickup.Interact();
                    }

                    return;
                }

                // Interaction normale avec les autres objets
                currentInteractable.Interact();
            }
        }
        else
        {
            // Aucun objet devant : si le joueur a un objet,
            // E permet de le poser au sol
            if (itemInHand && InputSystem.actions["Interact"].WasPressedThisFrame())
            {
                DropItem();
            }
        }
    }

    private void FindInteractable()
    {
        currentInteractable = null;

        Collider[] colliders = Physics.OverlapBox(
            interactionCollider.bounds.center,
            interactionCollider.bounds.extents,
            interactionCollider.transform.rotation
        );

        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            IInteractable interactable = col.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                col.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }
    }

    public void AddItem(PickableItem item)
    {
        if (itemInHand)
            return;

        if (item == null)
            return;

        itemDataInHand = item;
        itemInHand = true;

        Debug.Log("Objet en main : " + itemDataInHand.itemName);
    }

    private void DropItem()
    {
        if (!itemInHand || itemDataInHand == null)
            return;

        Vector3 dropPosition = interactionCollider.bounds.center;
        Quaternion dropRotation = transform.rotation;

        // On utilise toujours le prefab de base
        GameObject droppedObject = Instantiate(
            pickedUpItemPrefab,
            dropPosition,
            dropRotation
        );

        // On récupère le PickupInteractable du prefab
        PickUpItem pickup = droppedObject.GetComponentInChildren<PickUpItem>();

        if (pickup != null)
        {
            // On donne au prefab la data de l'objet que le joueur avait en main
            pickup.itemData = itemDataInHand;
            pickup.itemName = itemDataInHand.itemName;
        }

        Debug.Log("Objet posé : " + itemDataInHand.itemName);

        itemDataInHand = null;
        itemInHand = false;
    }

    private void SwapItem(PickUpItem pickup)
    {
        if (!itemInHand || itemDataInHand == null || pickup == null)
            return;

        PickableItem groundItem = pickup.itemData;
        PickableItem heldItem = itemDataInHand;

        if (groundItem == null)
            return;

        // Position de l'objet qui est au sol
        Vector3 swapPosition = pickup.transform.position;
        Quaternion swapRotation = pickup.transform.rotation;

        // On fait apparaître le prefab de base
        GameObject droppedObject = Instantiate(
            pickedUpItemPrefab,
            swapPosition,
            swapRotation
        );

        // On récupère le PickupInteractable du nouveau prefab
        PickUpItem newPickup = droppedObject.GetComponentInChildren<PickUpItem>();

        if (newPickup != null)
        {
            // On lui donne la data de l'objet que le joueur avait en main
            newPickup.itemData = heldItem;
            newPickup.itemName = heldItem.itemName;
        }

        // On supprime l'ancien objet au sol
        Destroy(pickup.gameObject);

        // Le joueur récupère l'objet qui était au sol
        itemDataInHand = groundItem;
        itemInHand = true;

        Debug.Log("Échange effectué : " + heldItem.itemName + " ↔ " + groundItem.itemName);
    }
}
