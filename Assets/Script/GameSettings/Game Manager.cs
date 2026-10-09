using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int playerScoreAmount;
    public TextMeshProUGUI scoreUI;
    public int currentScoreMultiplicator; //--

    [Header("Clients")]
    public ClientData[] clientDatasList;
    public int clientQueueSize = 5;

    [Header("Client Spawn")]
    public Transform clientSpawnPoint;
    public GameObject clientPrefab;

    [SerializeField] private int currentClientNumber;
    [SerializeField] private bool isSpawningClient;
    
    [Header("Drunkness")]
    public int playerDrunknessLevel;
    public Camera playerCamera; //pour les effets

    private void Start()
    {
        UpdateScoreUI();
        currentScoreMultiplicator = 1;

        SpawnNextClient();
    }

    public void AddScore(int amount)
    {
        playerScoreAmount += amount * currentScoreMultiplicator;

        UpdateScoreUI();

        Debug.Log("Score : " + playerScoreAmount);
    }

    private void UpdateScoreUI()
    {
        if (scoreUI != null)
        {
            scoreUI.text = playerScoreAmount.ToString();
        }
    }

    public ClientData GetRandomClientData()
    {
        if (clientDatasList == null || clientDatasList.Length == 0)
        {
            Debug.LogError("GameManager : clientDatasList est vide !");
            return null;
        }

        int randomIndex = Random.Range(0, clientDatasList.Length);
        ClientData data = clientDatasList[randomIndex];

        if (data == null)
        {
            return null;
        }

        return data;
    }

    public void SpawnNextClient()
    {
        if (isSpawningClient)
            return;

        if (currentClientNumber >= clientQueueSize)
        {
            Debug.Log("La file de clients terminée.");
            return;
        }

        if (clientSpawnPoint == null)
        {
            Debug.LogError("GameManager : clientSpawnPoint n'est pas assigné dans l'Inspector !");

            return;
        }

        if (clientPrefab == null)
        {
            return;
        }

        ClientData clientData = GetRandomClientData();

        if (clientData == null)
            return;

        isSpawningClient = true;

        GameObject clientObject = Instantiate(
            clientPrefab,
            clientSpawnPoint.position,
            clientSpawnPoint.rotation
        );

        Debug.Log("Prefab instanci� : " + clientObject.name);

        Clients client = clientObject.GetComponent<Clients>();

        if (client == null)
        {
            Debug.LogError("problème");

            Destroy(clientObject);
            isSpawningClient = false;
            return;
        }

        currentClientNumber++;

        // Transmet le GameManager et la commande au client.
        client.Initialize(this, clientData);

        Debug.Log("Client " + currentClientNumber+ "/" + clientQueueSize + " apparu.");

        isSpawningClient = false;
    }
}