using UnityEngine;
using UnityEngine.InputSystem;

public class DrinkingBottle : MonoBehaviour
{
    [Header("Reference")]
    public Collider colliderZone;
    public PlayerInteraction playerInteraction;
    public GameManager gameManager;

    [Header("Drinking")]
    [SerializeField] private float drunknessIncrease = 0.2f;
    [SerializeField] private float scoreMultiplicatorIncrease = 0.1f;
    [SerializeField] AudioSource drinking;

    private bool playerInRange;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (!playerInRange || playerInteraction == null || gameManager == null)
            return;

        if (InputSystem.actions["Jump"].WasPressedThisFrame())
            Drink();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInteraction interaction = other.GetComponentInParent<PlayerInteraction>();

        if (interaction == null)
            return;

        playerInteraction = interaction;
        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInteraction interaction = other.GetComponentInParent<PlayerInteraction>();

        if (interaction == null || interaction != playerInteraction)
            return;

        playerInRange = false;
        playerInteraction = null;
    }

    private void Drink()
    {
        if (!playerInteraction.itemInHand || playerInteraction.itemDataInHand == null)
            return;

        PickableItem bottle = playerInteraction.itemDataInHand;

        if (bottle.itemType != PickableItem.ItemType.Piquette &&
            bottle.itemType != PickableItem.ItemType.Clairet &&
            bottle.itemType != PickableItem.ItemType.Chateau &&
            bottle.itemType != PickableItem.ItemType.TurnedWine)
            return;
        drinking.Play();

        gameManager.IncreaseDrunkness(drunknessIncrease, scoreMultiplicatorIncrease);

        playerInteraction.itemDataInHand = null;
        playerInteraction.itemInHand = false;

        Debug.Log("Bouteille bue : " + bottle.itemName);
    }
}