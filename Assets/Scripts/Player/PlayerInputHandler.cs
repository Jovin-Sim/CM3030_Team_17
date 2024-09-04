using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerInput input;
    PlayerAC actions;

    public delegate void PauseAction();
    public event PauseAction OnPause;

    public delegate void FireAction();
    public event FireAction OnFireStart;
    public event FireAction OnFireStop;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        actions = new PlayerAC();
        actions.UI.Enable();

        //actions.UI.Pause.performed += ctx => OnPause?.Invoke();

        actions.Player.Pause.performed += ctx => OnPause?.Invoke();

        actions.Player.Fire.started += ctx => OnFireStart?.Invoke();
        actions.Player.Fire.canceled += ctx => OnFireStop?.Invoke();

        actions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        actions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;
        actions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        actions.Player.Look.canceled += ctx => LookInput = Vector2.zero;
    }

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

        Time.timeScale = timeScale;
    }

    void OnDisable()
    {
        actions.Player.Disable();
        actions.UI.Disable();
    }
}
