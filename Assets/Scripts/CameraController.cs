using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float dragSpeed = 5f;

    [SerializeField] private SharedBoolScriptable allowInput;

    private void Start()
    {
        EventManager.OnProjectLoaded += OnProjectLoaded;
    }

    private void Update()
    {
        if (!allowInput.value) return;

        Vector2 keyboardInput = InputHandler.controls.Player.MoveWorld.ReadValue<Vector2>();

        transform.position += (Vector3)keyboardInput * moveSpeed * Time.deltaTime;

        if (Mouse.current.middleButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            transform.position -= (Vector3)delta * dragSpeed * Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        EventManager.OnProjectLoaded -= OnProjectLoaded;
    }

    private void OnProjectLoaded() => transform.position = new Vector3(0, 0, -10);
}
