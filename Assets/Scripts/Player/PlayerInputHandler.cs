using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerInput input;
    PlayerAC actions;

    // Delegate and event for handling pause action
    public delegate void PauseAction();
    public event PauseAction OnPause;

    // Delegate and events for handling fire action
    public delegate void FireAction();
    public event FireAction OnFireStart;
    public event FireAction OnFireStop;

    // Properties for storing movement and look input vectors
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        actions = new PlayerAC();
        // Enable the UI action map
        actions.UI.Enable();

        // Subscribe to the appropriate input events
        actions.Player.Pause.performed += ctx => OnPause?.Invoke();

        actions.Player.Fire.started += ctx => OnFireStart?.Invoke();
        actions.Player.Fire.canceled += ctx => OnFireStop?.Invoke();

        actions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        actions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;
        actions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        actions.Player.Look.canceled += ctx => LookInput = Vector2.zero;
    }

    /// <summary>
    /// Changes the active action map between gameplay and UI
    /// Also adjusts the game's time scale
    /// </summary>
    /// <param name="mapName">The name of the action map to enable</param>
    /// <param name="timeScale">The time scale to apply</param>
    public void ChangeActionMap(string mapName, float timeScale = 1f)
    {
        switch (mapName)
        {
            case "Player":
                actions.Player.Enable();
                actions.UI.Disable();
                break;
            case "UI":
                actions.Player.Disable();
                actions.UI.Enable();
                break;
            default:
                break;
        }
        // Set the time scale for the game
        Time.timeScale = timeScale;
    }

    void OnDisable()
    {
        // Disable both action maps on destroy
        actions.Player.Disable();
        actions.UI.Disable();
    }

    private void OnDestroy()
    {
        // Unsubscribe from input events
        actions.Player.Pause.performed -= ctx => OnPause?.Invoke();
        actions.Player.Fire.started -= ctx => OnFireStart?.Invoke();
        actions.Player.Fire.canceled -= ctx => OnFireStop?.Invoke();
        actions.Player.Move.performed -= ctx => MoveInput = ctx.ReadValue<Vector2>();
        actions.Player.Move.canceled -= ctx => MoveInput = Vector2.zero;
        actions.Player.Look.performed -= ctx => LookInput = ctx.ReadValue<Vector2>();
        actions.Player.Look.canceled -= ctx => LookInput = Vector2.zero;

        // Disable the action maps
        actions.Player.Disable();
        actions.UI.Disable();
    }
}
