using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb2d;

    #region Input Variables
    PlayerInput input;
    PlayerAC actions;
    #endregion

    #region Movement Variables
    [Header("Movement Variables")]
    [SerializeField] float currMoveSpeed; // The move speed the player currently has
    [SerializeField] float originalMoveSpeed = 0f; // The move speed the player starts with, free of any other effects
    [SerializeField] bool canMove = true;

    Vector2 minBounds, maxBounds;
    #endregion

    #region Combat Variables
    [Header("Combat Variables")]
    Combat combat;
    Transform firePoint; // The point the player's bullets come out from
    Coroutine firingCoroutine;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] bool rapidFire = false;
    [SerializeField] GameObject bulletPrefab; // The prefab of the bullet
    [SerializeField] float bulletSpeed = 5f; // The speed of the bullet
    [SerializeField] bool bulletPierce = false; // A boolean for if the bullets can pierce through enemies
    [SerializeField] bool bulletExplode = false; // A boolean for if the bullets explode on impact
    #endregion

    public float CurrMoveSpeed { get { return currMoveSpeed; } set { currMoveSpeed = value; } }
    public Vector2 MinBounds { get { return minBounds; } set { minBounds = value; } }
    public Vector2 MaxBounds { get {return maxBounds; } set { maxBounds = value; } }
    public Combat Combat { get { return combat; } }
    public float FireRate { get { return fireRate; } set { fireRate = value; } }
    public bool BulletPierce { get { return bulletPierce; } set { bulletPierce = value; } }
    public bool BulletExplode { get { return bulletExplode; } set { bulletExplode = value; } }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        combat = GetComponent<Combat>();
        firePoint = transform.Find("FirePoint");

        // Enable the player controls
        actions = new PlayerAC();
        actions.Player.Enable();
        // Add the logic of firing to the "Fire" input
        actions.Player.Fire.started += _ => StartFiring();
        actions.Player.Fire.canceled += _ => StopFiring();

        currMoveSpeed = originalMoveSpeed;
    }

    private void FixedUpdate()
    {
        // Handle the player's movement and look
        if (canMove) Movement(actions.Player.Move);
        Look(actions.Player.Look);
    }

    public void TogglePlayerControllability(bool canControl)
    {
        if (canControl) actions.Player.Enable();
        else actions.Player.Disable();
    }

    public void ToggleUIControllability(bool canControl)
    {
        if (canControl) actions.UI.Enable();
        else actions.UI.Disable();
    }

    /// <summary>
    /// Handle the player's movement input
    /// </summary>
    /// <param name="action">The player movement input</param>
    void Movement(InputAction action)
    {
        // Retrieve the movement input data
        Vector2 movement = action.ReadValue<Vector2>();

        // Move the player based on the input data
        rb2d.velocity = movement * currMoveSpeed;
    }

    /// <summary>
    /// Handle the player's look input
    /// </summary>
    /// <param name="action">The player look input</param>
    void Look(InputAction action)
    {
        // Retrieve the look input data
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(action.ReadValue<Vector2>());

        // Get the direction the player character should look at
        Vector2 lookDir = cursorPos - rb2d.position;

        // Rotate the player's character to look at the mouse
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb2d.rotation = angle;
    }

    public IEnumerator LockMovement(float time = 0.5f)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void StartFiring()
    {
        firingCoroutine = StartCoroutine(Firing());
    }

    void StopFiring()
    {
        if (firingCoroutine == null) return;
        StopCoroutine(firingCoroutine);
        firingCoroutine = null;
    }

    IEnumerator Firing()
    {
        FireOnce();
        if (!rapidFire) yield return null;
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            FireOnce();
        }
    }

    /// <summary>
    /// Fire a single bullet
    /// </summary>
    void FireOnce()
    {
        // Log an error if the bullet prefab is missing
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab not found!");
            return;
        }

        // Fire a bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
        if (bullet.TryGetComponent<Bullet>(out Bullet b)) 
            b.Init(combat.CurrAtk, bulletPierce, bulletExplode);
            GameplayManager.instance.audioManager.PlaySFX(GameplayManager.instance.audioManager.gunshot);
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
