using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// The InputHandler of the game.
/// </summary>
public class InputHandler : MonoBehaviour
{
    public static InputSystem_Actions controls { private set; get; } /**< InputHandler Input controls. */

    /// <summary>
    /// Creates PlayerInput.
    /// </summary>
    private void Awake()
    {
        controls = new InputSystem_Actions();
    }

    /// <summary>
    /// Enables controls.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables controls.
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }
}