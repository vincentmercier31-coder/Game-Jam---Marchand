using UnityEngine;

public class PlacePrefabInHand : MonoBehaviour
{
    [SerializeField] private Transform playerHand;
    [SerializeField] private PlayerInteraction playerInteraction;

    private GameObject currentItem;

    private void Update()
    {
        if (playerInteraction == null || playerHand == null)
            return;

        if (!playerInteraction.itemInHand || playerInteraction.itemDataInHand == null)
        {
            if (currentItem != null)
            {
                Destroy(currentItem);
                currentItem = null;
            }

            return;
        }

        if (currentItem != null)
            return;

        GameObject prefab = playerInteraction.itemDataInHand.itemPrefab;

        if (prefab == null)
            return;

        currentItem = Instantiate(prefab, playerHand);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
    }
}