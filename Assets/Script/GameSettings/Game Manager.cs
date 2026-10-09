using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int playerScoreAmount;
    public TextMeshProUGUI scoreUI;
    public float currentScoreMultiplicator = 1f;

    [Header("Clients")]
    public ClientData[] clientDatasList;
    public int clientQueueSize = 5;
    public float clientPatience;
    public int maxPatience = 100;

    [Header("UI")]
    [SerializeField] private Image patienceBar;

    [Header("Client Spawn")]
    public Transform clientSpawnPoint;
    public GameObject clientPrefab;

    [SerializeField] private int currentClientNumber;
    [SerializeField] private bool isSpawningClient;

    private Clients currentClient;

    [Header("Drunkness")]
    public float playerDrunknessLevel;
    public CinemachineMixingCamera playerCamera;
    [SerializeField] private int maxDrunknessLevel = 5;

    [Header("SFX")]
    [SerializeField] AudioSource happy;
    [SerializeField] AudioSource angry;
    [SerializeField] AudioSource patience;

    private void Start()
    {
        UpdateScoreUI();
        currentScoreMultiplicator = 1f;
        UpdateDrunkCamera();
        clientPatience = 0f;
        UpdatePatienceBar();
        SpawnNextClient();
    }

    private void Update()
    {
        if (currentClient == null)
            return;

        if (clientPatience < maxPatience)
        {
            clientPatience += Time.deltaTime;
            clientPatience = Mathf.Min(clientPatience, maxPatience);
            UpdatePatienceBar();
        }

        if (clientPatience >= maxPatience)
            ClientPatienceEnded();
    }

    public void AddScore(int amount)
    {
        playerScoreAmount += Mathf.RoundToInt(amount * currentScoreMultiplicator);
        UpdateScoreUI();
        if (amount >= 6)
        {
            happy.Play();
        }
        else { angry.Play(); }
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
            currentClient = null;

            if (patienceBar != null)
                patienceBar.fillAmount = 0f;

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

        Clients client = clientObject.GetComponent<Clients>();

        if (client == null)
        {
            Debug.LogError("Le prefab ne possède pas de composant Clients.");
            Destroy(clientObject);
            isSpawningClient = false;
            return;
        }

        currentClientNumber++;
        currentClient = client;
        clientPatience = 0f;

        client.Initialize(this, clientData);

        UpdatePatienceBar();

        Debug.Log("Client " + currentClientNumber + "/" + clientQueueSize + " apparu.");

        isSpawningClient = false;
    }

    private void UpdatePatienceBar()
    {
        if (patienceBar == null)
            return;

        if (maxPatience <= 0)
        {
            patienceBar.fillAmount = 1f;
            return;
        }

        patienceBar.fillAmount = Mathf.Clamp01(clientPatience / maxPatience);
    }

    private void ClientPatienceEnded()
    {
        if (currentClient == null)
            return;

        Debug.Log("Le client a atteint sa patience maximale et s'en va.");

        Clients clientLeaving = currentClient;
        currentClient = null;
        clientPatience = 0f;

        UpdatePatienceBar();

        Destroy(clientLeaving.gameObject);
        SpawnNextClient();
        patience.Play();
    }
}