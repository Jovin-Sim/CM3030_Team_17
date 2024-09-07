using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // The rigidbody of the gameobject
    Rigidbody2D rb2d;
    // The input handler
    PlayerInputHandler inputHandler;

    #region Movement Variables
    [Header("Movement Variables")]
    [Tooltip("The speed the player starts at")]
    [SerializeField] float originalMoveSpeed = 0f;
    [Tooltip("The current speed of the player (Read Only)")]
    [SerializeField] float currMoveSpeed;
    [Tooltip("A boolean for if the player can move (Read Only)")]
    [SerializeField] bool canMove = true;
    #endregion

    #region Combat Variables
    [Header("Combat Variables")]
    Combat combat;
    Transform firePoint; // The point the player's bullets come out from
    Coroutine firingCoroutine;

    [Tooltip("The rate of fire of the player")]
    [SerializeField] float fireRate = 0.3f;
    [Tooltip("A boolean for if  the player can fire rapidly")]
    [SerializeField] bool rapidFire = false;

    [Tooltip("The prefab of the bullet")]
    [SerializeField] GameObject bulletPrefab;
    [Tooltip("The speed of the bullet")]
    [SerializeField] float bulletSpeed = 5f;

    [Tooltip("A boolean for if the bullets can pierce through enemies")]
    [SerializeField] bool bulletPierce = false;
    [Tooltip("A boolean for if the bullets explode on impact")]
    [SerializeField] bool bulletExplode = false;
    #endregion

    #region Getters & Setters
    public float CurrMoveSpeed { get { return currMoveSpeed; } set { currMoveSpeed = value; } }
    public Combat Combat { get { return combat; } }
    public float FireRate { get { return fireRate; } set { fireRate = value; } }
    public bool BulletPierce { get { return bulletPierce; } set { bulletPierce = value; } }
    public bool BulletExplode { get { return bulletExplode; } set { bulletExplode = value; } }
    #endregion

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        inputHandler = GameManager.instance.inputHandler;
        combat = GetComponent<Combat>();
        firePoint = transform.Find("FirePoint");

        // Set the player's current speed
        currMoveSpeed = originalMoveSpeed;

        inputHandler.OnFireStart += StartFiring;
        inputHandler.OnFireStop += StopFiring;
    }

    private void FixedUpdate()
    {
        // Handle the player's movement and look
        if (canMove) Movement(inputHandler.MoveInput);
        Look(inputHandler.LookInput);
    }

    /// <summary>
    /// Handle the player's movement input
    /// </summary>
    void Movement(Vector2 movement)
    {
        // Change the player's velocity
        rb2d.velocity = movement * currMoveSpeed;
    }

    /// <summary>
    /// Handle the player's look input
    /// </summary>
    void Look(Vector2 cursorPos)
    {
        // Get the look direction of the player
        Vector2 lookDir = (Vector2)Camera.main.ScreenToWorldPoint(cursorPos) - rb2d.position;
        
        // Get the angle of the look direction
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        // Change the rotation of the gameobject to look towards the mouse
        rb2d.rotation = angle;
    }

    /// <summary>
    /// Lock the player's movement for a specified time
    /// </summary>
    /// <param name="time">The time to lock the player's movement</param>
    public IEnumerator LockMovement(float time = 0.5f)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    /// <summary>
    /// Start the firing coroutine
    /// </summary>
    void StartFiring()
    {
        firingCoroutine = StartCoroutine(Firing());
    }

    /// <summary>
    /// Stop the firing coroutine
    /// </summary>
    void StopFiring()
    {
        if (firingCoroutine == null) return;
        StopCoroutine(firingCoroutine);
        firingCoroutine = null;
    }

    /// <summary>
    /// The firing coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator Firing()
    {
        // Fire a bullet
        FireOnce();

        yield return new WaitForSeconds(fireRate);

        // End the function if rapid fire is disabled
        if (!rapidFire) yield break;

        // Rapid fire
        while (true)
        {
            FireOnce();
            yield return new WaitForSeconds(fireRate);
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
            GameManager.instance.audioManager.PlaySFX(GameManager.instance.audioManager.gunshot);
    }

    private void OnDestroy()
    {
        if (inputHandler == null) return;

        inputHandler.OnFireStart -= StartFiring;
        inputHandler.OnFireStop -= StopFiring;

        StopAllCoroutines();
    }
}
