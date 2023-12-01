using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerNumber { One, Two }

public class PlayerController : MonoBehaviour
{
    public PlayerNumber playerNumber;

    public float speed = 8f;
    public float health;

    public GameObject projectilePrefab;
    public Transform projectileFireLocation;

    public GameObject weaponParent;
    public float weaponRotationDuration;

    public Color projectileColor;
    //public Ease weaponRotationEase;

    public float projectileFireCooldown;

    public Image healthUI;

    Rigidbody2D rb;

    bool isFiring;

    Vector2 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(FireProjectiles());
    }

    float cooldownTimer = 0f;
    void Update()
    {
        if (playerNumber == PlayerNumber.One)
        {
            if (Input.GetButtonDown("Fire1")) isFiring = true;
            if (Input.GetButtonUp("Fire1")) isFiring = false;
        }
        else
        {
            if (Input.GetButtonDown("Fire2")) isFiring = true;
            if (Input.GetButtonUp("Fire2")) isFiring = false;
        }
        

        UpdateWeaponRotation();
    }

    IEnumerator FireProjectiles()
    {
        while (true)
        {
            yield return null;

            if (!isFiring) continue;

            GameObject projectile = Instantiate(projectilePrefab, projectileFireLocation.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize((projectileFireLocation.position - weaponParent.transform.position).normalized, weaponParent.transform.eulerAngles, projectileColor);

            yield return new WaitForSeconds(projectileFireCooldown);
        }
    }

    private void UpdateWeaponRotation()
    {
        if (playerNumber == PlayerNumber.One)
        {
            if (Input.GetAxisRaw("Vertical1") >= 1) weaponParent.transform.localEulerAngles = Vector3.zero;
            if (Input.GetAxisRaw("Vertical1") <= -1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
            if (Input.GetAxisRaw("Horizontal1") >= 1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
            if (Input.GetAxisRaw("Horizontal1") <= -1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
        }
        else
        {
            if (Input.GetAxisRaw("Vertical2") >= 1) weaponParent.transform.localEulerAngles = Vector3.zero;
            if (Input.GetAxisRaw("Vertical2") <= -1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
            if (Input.GetAxisRaw("Horizontal2") >= 1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
            if (Input.GetAxisRaw("Horizontal2") <= -1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
        }
    }

    private void HandleMovementInput()
    {

    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;

        if (playerNumber == PlayerNumber.One) velocity = new Vector2(Input.GetAxis("Horizontal1") * speed * Time.fixedDeltaTime, Input.GetAxis("Vertical1") * speed * Time.fixedDeltaTime);
        else velocity = new Vector2(Input.GetAxis("Horizontal2") * speed * Time.fixedDeltaTime, Input.GetAxis("Vertical2") * speed * Time.fixedDeltaTime);

        rb.velocity = velocity;
    }

    public void AddForce(Vector2 force) => rb.AddForce(force);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var projectileComponent = collision.gameObject.GetComponent<Projectile>();

        if (projectileComponent != null) health -= projectileComponent.damage;

        healthUI.fillAmount = health / 100;

        if (health <= 0) Destroy(gameObject);
    }

    public void AddHealth(int health)
    {
        this.health += health;

        healthUI.fillAmount = this.health / 100;
    }
}
