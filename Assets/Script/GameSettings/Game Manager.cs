using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int playerScoreAmount;
    public TextMeshProUGUI scoreUI;

    [Header("Clients")]
    public ClientData[] clientDatasList;
    public int clientQueueSize = 5;

    [Header("Client Spawn")]
    public Transform clientSpawnPoint;
    public GameObject clientPrefab;

    [SerializeField] private int currentClientNumber;
    [SerializeField] private bool isSpawningClient;

    private void Start()
    {
        UpdateScoreUI();

        Debug.Log("GameManager démarré.");

        SpawnNextClient();
    }

    public void AddScore(int amount)
    {
        playerScoreAmount += amount;

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
            Debug.LogError(
                "GameManager : l'entrée " + randomIndex
                + " de clientDatasList est vide !"
            );

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
            Debug.Log("La file de clients est terminée.");
            return;
        }

        if (clientSpawnPoint == null)
        {
            Debug.LogError(
                "GameManager : clientSpawnPoint n'est pas assigné dans l'Inspector !"
            );

            return;
        }

        if (clientPrefab == null)
        {
            Debug.LogError(
                "GameManager : clientPrefab n'est pas assigné dans l'Inspector !"
            );

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

        Debug.Log("Prefab instancié : " + clientObject.name);

        Clients client = clientObject.GetComponent<Clients>();

        if (client == null)
        {
            Debug.LogError(
                "Le prefab clientPrefab ne contient pas le script Clients sur sa racine !"
            );

            Destroy(clientObject);
            isSpawningClient = false;
            return;
        }

        currentClientNumber++;

        // Transmet le GameManager et la commande au client.
        client.Initialize(this, clientData);

        Debug.Log(
            "Client " + currentClientNumber
            + "/" + clientQueueSize + " apparu."
        );

        isSpawningClient = false;
    }
}