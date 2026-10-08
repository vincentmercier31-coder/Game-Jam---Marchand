using UnityEngine;

public class ModelSpawner : MonoBehaviour
{
    private PickUpItem pickUpItemScript;



    private void Start()
    {
        pickUpItemScript = gameObject.GetComponent<PickUpItem>();
        Instantiate(pickUpItemScript.itemData.itemPrefab, transform);
    }
}
