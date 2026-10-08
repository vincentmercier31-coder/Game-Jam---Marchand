using UnityEngine;
using UnityEngine.InputSystem;

public class ItemGiver : MonoBehaviour
{
    [SerializeField] private GameObject pickUpItemPrefab;
    [SerializeField] private PickableItem itemData;
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private Collider interactionCollider;

    private void Update()
    {
        if (!InputSystem.actions["Interact"].WasPressedThisFrame())
            return;

        Collider[] colliders = Physics.OverlapBox(interactionCollider.bounds.center, interactionCollider.bounds.extents, interactionCollider.transform.rotation);

        foreach (Collider col in colliders)
        {
            if (col.GetComponentInParent<PlayerInteraction>() == null)
                continue;

            SpawnItem();
            return;
        }
    }

    private void SpawnItem()
    {
        if (pickUpItemPrefab == null || itemData == null || itemSpawnPoint == null)
            return;

        GameObject spawnedItem = Instantiate(pickUpItemPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

        PickUpItem pickUpItem = spawnedItem.GetComponent<PickUpItem>();

        if (pickUpItem != null)
            pickUpItem.itemData = itemData;
    }
}