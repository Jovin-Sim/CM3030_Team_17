using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb2d;

    #region Input Variables
    PlayerInputHandler inputHandler;
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
        inputHandler = GameManager.instance.inputHandler;
        combat = GetComponent<Combat>();
        firePoint = transform.Find("FirePoint");

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
        rb2d.velocity = movement * currMoveSpeed;
    }

    /// <summary>
    /// Handle the player's look input
    /// </summary>
    void Look(Vector2 cursorPos)
    {
        Vector2 lookDir = (Vector2)Camera.main.ScreenToWorldPoint(cursorPos) - rb2d.position;
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
            GameManager.instance.audioManager.PlaySFX(GameManager.instance.audioManager.gunshot);
    }

    public void Upgrade(string upgradeType, float multiplier)
    {
        if(upgradeType == "Bullets")
        {
            bulletSpeed *= multiplier;
        }

    }
}
