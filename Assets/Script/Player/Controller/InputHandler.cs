using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 InputVector { get; private set; }
    public Vector3 MousePosition { get; private set; }

    private void Update()
    {
        Vector2 input = Vector2.zero;

        // ZQSD
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                input.x -= 1f;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1f;

            if (Keyboard.current.wKey.isPressed)
                input.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1f;
        }

        // Évite d'aller plus vite en diagonale
        InputVector = input.normalized;

        // Position de la souris
        if (Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            MousePosition = mousePosition;
        }
    }
}