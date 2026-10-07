using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Collider interactionCollider;

    private IInteractable currentInteractable;

    private void Update()
    {
        FindInteractable();

        if (currentInteractable != null)
        {
            if (InputSystem.actions["Interact"].WasPressedThisFrame())
            {
                currentInteractable.Interact();
            }
        }
    }

    private void FindInteractable()
    {
        currentInteractable = null;

        Collider[] colliders = Physics.OverlapBox(
            interactionCollider.bounds.center,
            interactionCollider.bounds.extents,
            interactionCollider.transform.rotation
        );

        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            IInteractable interactable = col.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                col.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }
    }
}