using UnityEngine;

public class TopDownCharacterMover : MonoBehaviour
{
    private InputHandler _input;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private bool rotateTowardsMouse = true;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;


    private void Awake()
    {
        _input = GetComponent<InputHandler>();

        if (_input == null)
        {
            Debug.LogError(
                "TopDownCharacterMover : aucun InputHandler trouvé sur " +
                gameObject.name
            );
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            Debug.LogError(
                "TopDownCharacterMover : aucune caméra trouvée. " +
                "Assigne une caméra dans l'Inspector ou donne-lui le tag MainCamera."
            );
        }
    }


    private void Update()
    {
        if (_input == null || playerCamera == null)
            return;


        // Récupération du déplacement ZQSD
        Vector3 targetVector = new Vector3(
            _input.InputVector.x,
            0f,
            _input.InputVector.y
        );


        // Déplacement
        Vector3 movementVector = MoveTowardTarget(targetVector);


        // Rotation
        if (!rotateTowardsMouse)
        {
            RotateTowardMovementVector(movementVector);
        }
        else
        {
            RotateTowardMouseVector();
        }
    }


    private void RotateTowardMouseVector()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            _input.MousePosition
        );


        if (Physics.Raycast(
            ray,
            out RaycastHit hitInfo,
            300f
        ))
        {
            Vector3 target = hitInfo.point;

            // On garde le personnage droit
            target.y = transform.position.y;


            Vector3 direction = target - transform.position;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime * 5
                );
            }
        }
    }


    private void RotateTowardMovementVector(
        Vector3 movementVector
    )
    {
        if (movementVector.sqrMagnitude < 0.001f)
            return;


        Quaternion rotation =
            Quaternion.LookRotation(movementVector);


        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rotation,
            rotateSpeed * Time.deltaTime * 5
        );
    }


    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        float speed = moveSpeed * Time.deltaTime;


        // On adapte le déplacement à la rotation de la caméra.
        Vector3 movement =
            Quaternion.Euler(
                0f,
                playerCamera.transform.eulerAngles.y,
                0f
            ) * targetVector;


        transform.position += movement * speed;


        return movement;
    }
}
