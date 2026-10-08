using UnityEngine;
using UnityEngine.UI;

public class Barrel : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Collider barrelCollider;
    [SerializeField] private GameObject wineDropPoint;
    [SerializeField] private GameObject filledWineBottleModel;
    [SerializeField] private GameObject pickUpItemPrefab;

    [Header("UI")]
    [SerializeField] private Image barUI;

    [Header("Grapes")]
    [SerializeField] private int currentGrapesCounter;
    [SerializeField] private int maxGrapes;

    [Header("WineMakingTime")]
    [SerializeField] private float piquetteTreshold;
    [SerializeField] private float clairetTreshold;
    [SerializeField] private float chateauTreshold;
    [SerializeField] private float turnedTreshold;

    [Header("Info")]
    [SerializeField] private bool isMakingWine;
    [SerializeField] private float wineMakingTimer;
    [SerializeField] private WineData.WineType
    currentWineType = WineData.WineType.Empty;

    private void Update()
    {
        CheckForItems();

        if (isMakingWine)
        {
            UpdateWineMaking();
        }
    }

    private void CheckForItems()
    {
        Collider[] colliders = Physics.OverlapBox(
            barrelCollider.bounds.center,
            barrelCollider.bounds.extents,
            barrelCollider.transform.rotation
        );

        foreach (Collider col in colliders)
        {
            PickUpItem pickup = col.GetComponentInParent<PickUpItem>();

            if (pickup == null)
                continue;

            if (pickup.itemData == null)
                continue;

            // Si c'est une bouteille vide
            if (pickup.itemData.itemType == PickableItem.ItemType.EmptyWineBottle)
            {
                // On essaye de remplir la bouteille avec le vin actuel
                TryFillBottle(pickup);

                continue;
            }

            // Si le tonneau est en train de faire du vin,
            // on ne récupère pas les raisins.
            if (isMakingWine)
                continue;

            // On vérifie si l'objet est bien des Grapes
            if (pickup.itemData.itemType != PickableItem.ItemType.Grapes)
                continue;

            // On vérifie si le tonneau peut encore recevoir des raisins
            if (currentGrapesCounter >= maxGrapes)
                continue;

            // On ajoute le raisin
            AddGrape();

            // On retire le raisin du monde
            Destroy(pickup.gameObject);
        }
    }

    private void AddGrape()
    {
        if (isMakingWine)
            return;

        if (currentGrapesCounter >= maxGrapes)
            return;

        currentGrapesCounter++;

        Debug.Log(
            "Raisin ajouté au tonneau : "
            + currentGrapesCounter
            + "/"
            + maxGrapes
        );

        // Dès que le tonneau est rempli, on commence la fabrication
        if (currentGrapesCounter >= maxGrapes)
        {
            StartWineMaking();
        }
    }

    private void StartWineMaking()
    {
        isMakingWine = true;
        wineMakingTimer = 0f;

        // Le tonneau commence vide.
        currentWineType = WineData.WineType.Empty;

        Debug.Log("Le tonneau commence à faire du vin !");

        UpdateBarUI();
    }

    private void UpdateWineMaking()
    {
        wineMakingTimer += Time.deltaTime;

        UpdateWineType();
        UpdateBarUI();

        // Le vin est terminé uniquement au dernier threshold
        if (wineMakingTimer >= turnedTreshold)
        {
            FinishWineMaking();
        }
    }

    private void UpdateWineType()
    {
        // Avant le premier threshold : Empty
        if (wineMakingTimer < piquetteTreshold)
        {
            currentWineType = WineData.WineType.Empty;
        }
        // À partir de piquetteTreshold : Piquette
        else if (wineMakingTimer < clairetTreshold)
        {
            currentWineType = WineData.WineType.Piquette;
        }
        // À partir de clairetTreshold : Clairet
        else if (wineMakingTimer < chateauTreshold)
        {
            currentWineType = WineData.WineType.Clairet;
        }
        // À partir de chateauTreshold : Chateau
        else if (wineMakingTimer < turnedTreshold)
        {
            currentWineType = WineData.WineType.Chateau;
        }
        // À partir de turnedTreshold : TurnedWine
        else
        {
            currentWineType = WineData.WineType.TurnedWine;
        }
    }

    private void UpdateBarUI()
    {
        if (barUI == null)
            return;

        // Progression de 0 à 1 basée sur le temps maximum
        barUI.fillAmount = Mathf.Clamp01(
            wineMakingTimer / turnedTreshold
        );
    }

    private void FinishWineMaking()
    {
        // À la fin, le vin devient TurnedWine
        currentWineType = WineData.WineType.TurnedWine;

        Debug.Log(
            "Le vin est terminé ! Type final : "
            + currentWineType
        );

        currentGrapesCounter = 0;
        wineMakingTimer = 0f;
        isMakingWine = false;

        if (barUI != null)
        {
            barUI.fillAmount = 0f;
        }
    }

    private bool TryFillBottle(PickUpItem emptyBottle)
    {
        // Si le tonneau est vide, on ne fait rien
        if (currentWineType == WineData.WineType.Empty)
            return false;

        if (emptyBottle == null)
            return false;

        if (wineDropPoint == null)
            return false;

        if (pickUpItemPrefab == null)
            return false;

        // Cherche automatiquement le PickableItem correspondant
        PickableItem wineItem = FindWineItem();

        if (wineItem == null)
        {
            Debug.LogWarning(
                "Impossible de trouver le PickableItem correspondant à : "
                + currentWineType
            );

            return false;
        }

        // Position de la bouteille vide
        Vector3 bottlePosition = emptyBottle.transform.position;
        Quaternion bottleRotation = emptyBottle.transform.rotation;

        // On supprime la bouteille vide
        Destroy(emptyBottle.gameObject);

        // On crée la bouteille pleine au même endroit
        GameObject bottle = Instantiate(
            pickUpItemPrefab,
            bottlePosition,
            bottleRotation
        );

        PickUpItem pickup = bottle.GetComponentInChildren<PickUpItem>();

        if (pickup != null)
        {
            // Donne à la bouteille le bon PickableItem
            pickup.itemData = wineItem;
            pickup.itemName = wineItem.itemName;
        }

        // Ajoute le modèle de bouteille pleine
        if (filledWineBottleModel != null)
        {
            GameObject model = Instantiate(
                filledWineBottleModel,
                bottle.transform
            );

            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }

        Debug.Log(
            "Bouteille remplie : "
            + wineItem.itemName
        );

        // Le vin actuel est récupéré
        currentWineType = WineData.WineType.Empty;

        return true;
    }

    private PickableItem FindWineItem()
    {
        PickableItem[] items = Resources.FindObjectsOfTypeAll<PickableItem>();

        foreach (PickableItem item in items)
        {
            if (item == null)
                continue;

            if (item.itemType == (PickableItem.ItemType)currentWineType)
            {
                return item;
            }
        }

        return null;
    }
}