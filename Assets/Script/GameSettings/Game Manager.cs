using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int playerScoreAmount;
    public TextMeshProUGUI scoreUI;
    public float currentScoreMultiplicator = 1f;

    [Header("Clients")]
    public ClientData[] clientDatasList;
    public int clientQueueSize = 5;

    [Header("Client Spawn")]
    public Transform clientSpawnPoint;
    public GameObject clientPrefab;

    [SerializeField] private int currentClientNumber;
    [SerializeField] private bool isSpawningClient;

    [Header("Drunkness")]
    public float playerDrunknessLevel;
    public CinemachineMixingCamera playerCamera;
    [SerializeField] private int maxDrunknessLevel = 5;

    private void Start()
    {
        UpdateScoreUI();
        currentScoreMultiplicator = 1f;
        UpdateDrunkCamera();
        SpawnNextClient();
    }

    public void AddScore(int amount)
    {
        playerScoreAmount += Mathf.RoundToInt(amount * currentScoreMultiplicator);
        UpdateScoreUI();
        Debug.Log("Score : " + playerScoreAmount);
    }

    private void UpdateScoreUI()
    {
        if (scoreUI != null)
            scoreUI.text = playerScoreAmount.ToString();
    }

    public void IncreaseDrunkness(float amount, float scoreMultiplicatorIncrease)
    {
        playerDrunknessLevel = Mathf.Clamp(playerDrunknessLevel + amount, 0f, maxDrunknessLevel);
        currentScoreMultiplicator += scoreMultiplicatorIncrease;

        UpdateDrunkCamera();

        Debug.Log("Ivresse : " + playerDrunknessLevel + " | Multiplicateur : x" + currentScoreMultiplicator.ToString("F2"));
    }

    private void UpdateDrunkCamera()
    {
        if (playerCamera == null || playerCamera.ChildCameras.Count < 2)
            return;

        playerCamera.SetWeight(1, playerDrunknessLevel);
    }

    public ClientData GetRandomClientData()
    {
        if (clientDatasList == null || clientDatasList.Length == 0)
            return null;

        return clientDatasList[Random.Range(0, clientDatasList.Length)];
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
            return;

        ClientData clientData = GetRandomClientData();

        if (clientData == null)
            return;

        isSpawningClient = true;

        GameObject clientObject = Instantiate(clientPrefab, clientSpawnPoint.position, clientSpawnPoint.rotation);
        Debug.Log("Prefab instancié : " + clientObject.name);

        Clients client = clientObject.GetComponent<Clients>();

        if (client == null)
        {
            Debug.LogError("problème");
            Destroy(clientObject);
            isSpawningClient = false;
            return;
        }

        currentClientNumber++;
        client.Initialize(this, clientData);

        Debug.Log("Client " + currentClientNumber + "/" + clientQueueSize + " apparu.");
        isSpawningClient = false;
    }
}