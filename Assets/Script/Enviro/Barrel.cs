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

    [Header("Wine Items")]
    [SerializeField] private PickableItem piquetteItem;
    [SerializeField] private PickableItem clairetItem;
    [SerializeField] private PickableItem chateauItem;
    [SerializeField] private PickableItem turnedWineItem;

    [Header("Info")]
    [SerializeField] private bool isMakingWine;
    [SerializeField] private float wineMakingTimer;
    [SerializeField] private WineData.WineType currentWineType = WineData.WineType.Empty;

    [Header("UI")]

    [SerializeField] AudioSource grapes;
    [SerializeField] AudioSource bottlesfx;

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
        if (barrelCollider == null)
            return;

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

            // Si c'est une bouteille vide, on essaye de la remplir.
            if (pickup.itemData.itemType == PickableItem.ItemType.EmptyWineBottle)
            {
                if (TryFillBottle(pickup))
                    return;

                continue;
            }

            // Si le tonneau est déjà en train de faire du vin,
            // on ne récupère plus de raisins.
            if (isMakingWine)
                continue;

            // On vérifie si l'objet est bien des Grapes.
            if (pickup.itemData.itemType != PickableItem.ItemType.Grapes)
                continue;

            // On vérifie si le tonneau peut encore recevoir des raisins.
            if (currentGrapesCounter >= maxGrapes)
                continue;

            // On ajoute le raisin.
            AddGrape();

            // On retire le raisin du monde.
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
        grapes.Play();

        Debug.Log(
            "Raisin ajouté au tonneau : "
            + currentGrapesCounter
            + "/"
            + maxGrapes
        );

        // Dès que le tonneau est rempli, on commence la fabrication.
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

        // Le vin est terminé uniquement au dernier threshold.
        if (wineMakingTimer >= turnedTreshold)
        {
            FinishWineMaking();
        }
    }

    private void UpdateWineType()
    {
        // Avant le premier threshold : Empty.
        if (wineMakingTimer < piquetteTreshold)
        {
            currentWineType = WineData.WineType.Empty;
            barUI.color = Color.red;
        }
        // À partir de piquetteTreshold : Piquette.
        else if (wineMakingTimer < clairetTreshold)
        {
            currentWineType = WineData.WineType.Piquette;
            barUI.color = Color.orange;
        }
        // À partir de clairetTreshold : Clairet.
        else if (wineMakingTimer < chateauTreshold)
        {
            currentWineType = WineData.WineType.Clairet;
            barUI.color = Color.yellow;
        }
        // À partir de chateauTreshold : Chateau.
        else if (wineMakingTimer < turnedTreshold)
        {
            currentWineType = WineData.WineType.Chateau;
            barUI.color = Color.green;
        }
        // À partir de turnedTreshold : TurnedWine.
        else
        {
            currentWineType = WineData.WineType.TurnedWine;
            barUI.color = Color.orange;
        }
    }

    private void UpdateBarUI()
    {
        if (barUI == null)
            return;

        if (turnedTreshold <= 0f)
        {
            barUI.fillAmount = 0f;
            return;
        }

        // Progression de 0 à 1 basée sur le temps maximum.
        barUI.fillAmount = Mathf.Clamp01(
            wineMakingTimer / turnedTreshold
        );
    }

    private void FinishWineMaking()
    {
        // À la fin, le vin devient TurnedWine.
        currentWineType = WineData.WineType.TurnedWine;

        Debug.Log(
            "Le vin est terminé ! Type final : "
            + currentWineType
        );

        // La fabrication est terminée.
        isMakingWine = false;

        // On garde le timer à sa valeur maximale tant que
        // le TurnedWine n'a pas été récupéré.
        wineMakingTimer = turnedTreshold;

        // Les raisins ont été consommés.
        currentGrapesCounter = 0;

        UpdateBarUI();
    }

    private bool TryFillBottle(PickUpItem emptyBottle)
    {
        if (emptyBottle == null)
            return false;

        // Le tonneau doit contenir un type de vin.
        if (currentWineType == WineData.WineType.Empty)
            return false;

        if (wineDropPoint == null)
            return false;

        if (pickUpItemPrefab == null)
            return false;

        PickableItem wineItem = GetWineItem();

        if (wineItem == null)
        {
            Debug.LogWarning(
                "Aucun PickableItem configuré pour le type de vin : "
                + currentWineType
            );

            return false;
        }

        // On détruit la bouteille vide.
        Destroy(emptyBottle.gameObject);

        // On crée la bouteille remplie au point de drop du tonneau.
        GameObject bottle = Instantiate(
            pickUpItemPrefab,
            wineDropPoint.transform.position,
            wineDropPoint.transform.rotation
        );
        bottlesfx.Play();

        // Récupère le PickUpItem du prefab.
        PickUpItem pickup = bottle.GetComponentInChildren<PickUpItem>();

        if (pickup == null)
        {
            Debug.LogWarning(
                "Le prefab pickUpItemPrefab ne contient pas de PickUpItem."
            );

            Destroy(bottle);
            return false;
        }

        // On donne à la nouvelle bouteille le bon PickableItem.
        pickup.itemData = wineItem;
        pickup.itemName = wineItem.itemName;

        // Ajoute le modèle de bouteille pleine.
        if (filledWineBottleModel != null)
        {
            GameObject model = Instantiate(
                filledWineBottleModel,
                pickup.transform
            );

            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }

        Debug.Log(
            "Bouteille remplie : "
            + wineItem.itemName
        );

        // reset tout
        currentWineType = WineData.WineType.Empty;
        isMakingWine = false;
        wineMakingTimer = 0f;
        currentGrapesCounter = 0;
        if (barUI != null)
        {
            barUI.fillAmount = 0f;
        }

        Debug.Log("Le tonneau a été vidé et la fabrication est arrêtée.");

        return true;
    }

    private PickableItem GetWineItem()
    {
        switch (currentWineType)
        {
            case WineData.WineType.Piquette:
                return piquetteItem;

            case WineData.WineType.Clairet:
                return clairetItem;

            case WineData.WineType.Chateau:
                return chateauItem;

            case WineData.WineType.TurnedWine:
                return turnedWineItem;
        }

        return null;
    }
}