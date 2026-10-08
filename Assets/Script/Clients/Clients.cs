using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clients : MonoBehaviour
{
    [Header("Reference")]
    public ClientData ClientData;
    [SerializeField] private Collider clientCollider;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI commandTextUI; // Met le nom de la bouteille
    [SerializeField] private GameObject bottleImageUI; // Met le sprite de la bouteille

    [Header("Client State")]
    [SerializeField] private bool orderCompleted;
    [SerializeField] private bool clientFinished;

    private GameManager gameManager;

    // Appelé par le GameManager après le spawn.
    public void Initialize(GameManager manager, ClientData data)
    {
        gameManager = manager;
        ClientData = data;

        SetupClient();
    }

    private void Start()
    {
        // Permet aussi de fonctionner si le client est placé
        // manuellement dans la scène.
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        if (ClientData != null)
        {
            SetupClient();
        }
    }

    private void Update()
    {
        if (clientFinished)
            return;

        CheckForBottle();
    }

    private void SetupClient()
    {
        if (ClientData == null)
        {
            Debug.LogWarning("Aucun ClientData assigné au client.");
            return;
        }

        if (ClientData.PickableItem == null)
        {
            Debug.LogWarning("Aucun PickableItem demandé dans le ClientData.");
            return;
        }

        // Affiche le nom de la bouteille demandée.
        if (commandTextUI != null)
        {
            commandTextUI.text = ClientData.PickableItem.itemName;
        }

        // Affiche l'image de la bouteille demandée.
        if (bottleImageUI != null)
        {
            Image image = bottleImageUI.GetComponent<Image>();

            if (image != null)
            {
                image.sprite = ClientData.bottleImage;
            }
        }

        Debug.Log("Le client demande : " + ClientData.PickableItem.itemName);
    }

    private void CheckForBottle()
    {
        if (clientCollider == null || ClientData == null || ClientData.PickableItem == null)
            return;

        Collider[] colliders = Physics.OverlapBox(
            clientCollider.bounds.center,
            clientCollider.bounds.extents,
            clientCollider.transform.rotation
        );

        foreach (Collider col in colliders)
        {
            PickUpItem pickup = col.GetComponentInParent<PickUpItem>();

            if (pickup == null || pickup.itemData == null)
                continue;

            // Le client accepte uniquement les bouteilles de vin.
            if (!IsWineBottle(pickup.itemData.itemType))
                continue;

            ReceiveBottle(pickup);
            return;
        }
    }

    private bool IsWineBottle(PickableItem.ItemType itemType)
    {
        return itemType == PickableItem.ItemType.Piquette
            || itemType == PickableItem.ItemType.Clairet
            || itemType == PickableItem.ItemType.Chateau
            || itemType == PickableItem.ItemType.TurnedWine;
    }

    private void ReceiveBottle(PickUpItem pickup)
    {
        if (pickup == null || orderCompleted)
            return;

        orderCompleted = true;

        bool correctBottle = pickup.itemData == ClientData.PickableItem;

        if (correctBottle)
        {
            Debug.Log("Bonne bouteille donnée au client : " + pickup.itemData.itemName);
            GiveScore(ClientData.scoreAmount);
        }
        else
        {
            Debug.Log(
                "Mauvaise bouteille donnée : " + pickup.itemData.itemName
                + " | Le client voulait : " + ClientData.PickableItem.itemName
            );

            GiveScore(-ClientData.scoreAmount);
        }

        // Détruit la bouteille donnée.
        Destroy(pickup.gameObject);

        CompleteClient();
    }

    private void GiveScore(int amount)
    {
        if (gameManager == null)
        {
            Debug.LogWarning("Le client n'est relié à aucun GameManager.");
            return;
        }

        gameManager.AddScore(amount);
    }

    private void CompleteClient()
    {
        if (clientFinished)
            return;

        clientFinished = true;

        // Retire la commande de l'UI.
        if (commandTextUI != null)
        {
            commandTextUI.text = "";
        }

        // Retire l'image de l'UI.
        if (bottleImageUI != null)
        {
            Image image = bottleImageUI.GetComponent<Image>();

            if (image != null)
            {
                image.sprite = null;
            }
        }

        Debug.Log("Commande du client terminée !");

        // Le GameManager fait apparaître le suivant.
        if (gameManager != null)
        {
            gameManager.SpawnNextClient();
        }

        // Détruit le client actuel.
        Destroy(gameObject);
    }
}