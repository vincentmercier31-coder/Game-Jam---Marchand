using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clients : MonoBehaviour
{
    [Header("Reference")]
    public ClientData ClientData;
    [SerializeField] private Collider clientCollider;
    [SerializeField] private Transform prefabSpawnPoint;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI commandTextUI; // Met le nom de la bouteille
    [SerializeField] private GameObject bottleImageUI; // Met le sprite de la bouteille

    [Header("Client State")]
    [SerializeField] private bool orderCompleted;
    [SerializeField] private bool clientFinished;

    [Header("Client State")]
    [SerializeField] AudioSource happy;
    [SerializeField] AudioSource angry;

    private GameManager gameManager;

    // Appel� par le GameManager apr�s le spawn.
    public void Initialize(GameManager manager, ClientData data)
    {
        gameManager = manager;
        ClientData = data;

        SetupClient();
    }

    private void Start()
    {
        // Permet aussi de fonctionner si le client est plac�
        // manuellement dans la sc�ne.
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
            Debug.LogWarning("Aucun ClientData assign� au client.");
            return;
            
        }

        if (ClientData.PickableItem == null)
        {
   
            return;
        }

        // Affiche le nom de la bouteille
        if (commandTextUI != null)
        {
            commandTextUI.text = ClientData.PickableItem.itemName;
        }

        // Affiche l'image de la bouteille
        if (bottleImageUI != null)
        {
            Image image = bottleImageUI.GetComponent<Image>();

            if (image != null)
            {
                image.sprite = ClientData.bottleImage;
            }
        }
        
        
        if (ClientData.clientPrefab != null && prefabSpawnPoint != null)
        {
            Instantiate(ClientData.clientPrefab, prefabSpawnPoint.position, prefabSpawnPoint.rotation, prefabSpawnPoint);
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
            Debug.Log("Bonne bouteille donn�e au client : " + pickup.itemData.itemName);
            GiveScore(ClientData.scoreAmount);
            happy.Play();
        }
        else
        {
            Debug.Log(
                "Mauvaise bouteille donn�e : " + pickup.itemData.itemName
                + " | Le client voulait : " + ClientData.PickableItem.itemName
            );

            GiveScore(5);
            angry.Play();
        }

        // D�truit la bouteille donn�e.
        Destroy(pickup.gameObject);

        CompleteClient();
    }

    private void GiveScore(int amount)
    {
        if (gameManager == null)
        {
            Debug.LogWarning("Le client n'est reli� � aucun GameManager.");
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

        Debug.Log("Commande du client termin�e !");

        // Le GameManager fait appara�tre le suivant.
        if (gameManager != null)
        {
            gameManager.SpawnNextClient();
        }

        // D�truit le client actuel.
        Destroy(gameObject);
    }
}