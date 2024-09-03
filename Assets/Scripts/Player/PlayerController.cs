using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    int id = 0;
    Rigidbody2D rb2d;

    #region Input Variables
    PlayerInput input;
    PlayerAC actions;
    #endregion

    #region Movement Variables
    [Header("Movement Variables")]
    [SerializeField] float currMoveSpeed;
    [SerializeField] float originalMoveSpeed = 0f;
    [SerializeField] float rotationSpeed = 0f;
    [SerializeField] bool canMove = true;
    #endregion

    #region Combat Variables
    [Header("Combat Variables")]
    Combat combat;
    Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 5f;
    #endregion

    #region Audio Variables
    [Header("Audio Variables")]
    AudioManager audioManager;
    #endregion

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        combat = GetComponent<Combat>();
        firePoint = transform.Find("FirePoint");

        actions = new PlayerAC();
        actions.Player.Enable();
        actions.Player.Fire.performed += Fire;

        currMoveSpeed = originalMoveSpeed;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        Movement(actions.Player.Move);
        Look(actions.Player.Look);
    }

    void Movement(InputAction action)
    {
        Vector2 movement = action.ReadValue<Vector2>();

        rb2d.MovePosition(rb2d.position + movement * currMoveSpeed * Time.deltaTime);
    }

    void Look(InputAction action)
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(action.ReadValue<Vector2>());

        Vector2 lookDir = cursorPos - rb2d.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb2d.rotation = angle;
    }

    void Fire(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (bulletPrefab == null)
        { 
            Debug.LogError("Bullet Prefab not found!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
        if (bullet.TryGetComponent<Bullet>(out Bullet b)) 
            b.Init(combat.CurrAtk);
            audioManager.PlaySFX(audioManager.gunshot);
    }

    public void Upgrade(string upgradeType, float multiplier)
    {
        if(upgradeType == "Bullets")
        {
            bulletSpeed *= multiplier;
        }

    }

    public void GameOver()
    {
        Destroy(gameObject);
    }

    void OnDisable()
    {
        actions.Player.Disable();
    }
}
