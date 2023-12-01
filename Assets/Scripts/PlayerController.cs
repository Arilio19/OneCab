using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerNumber { One, Two }

public class PlayerController : MonoBehaviour, IDamageable, IKillable
{
    [SerializeField] PlayerNumber playerNumber;

    [Header("Physics")]
    [SerializeField] float speed = 8f;
    [SerializeField] float health = 100f;
    [SerializeField] float projectileFireCooldown;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileFireLocation;
    [SerializeField] GameObject weaponParent;
    [SerializeField] Image healthUI;

    [Header("Visuals")]
    [SerializeField] Color projectileColor;   
    
    //Cached Variables
    Rigidbody2D rigidbody;
    bool isFiring;
    string horizontalInputString;
    string verticalInputString;
    string actionInputString;

    private void Awake() => rigidbody = GetComponent<Rigidbody2D>();

    void Start()
    {
        StartCoroutine(FireProjectiles());

        InitialiseInputStrings();
    }

    void InitialiseInputStrings()
    {
        int playerNumber = (int)this.playerNumber + 1;
        horizontalInputString = "Horizontal" + playerNumber;
        verticalInputString = "Vertical" + playerNumber;
        actionInputString = "Fire" + playerNumber;
    }

    void Update()
    {
        if (Input.GetButtonDown(actionInputString)) isFiring = true;
        if (Input.GetButtonUp(actionInputString)) isFiring = false;        

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

    void UpdateWeaponRotation()
    {
        if (Input.GetAxisRaw(verticalInputString) >= 1) weaponParent.transform.localEulerAngles = Vector3.zero;
        if (Input.GetAxisRaw(verticalInputString) <= -1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        if (Input.GetAxisRaw(horizontalInputString) >= 1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
        if (Input.GetAxisRaw(horizontalInputString) <= -1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
    }

    void FixedUpdate()
    {
        Vector2 velocity = rigidbody.velocity;
        velocity = new Vector2(Input.GetAxis(horizontalInputString) * speed * Time.fixedDeltaTime, Input.GetAxis(verticalInputString) * speed * Time.fixedDeltaTime);
        rigidbody.velocity = velocity;
    }

    public void AddForce(Vector2 force) => rigidbody.AddForce(force);

    public void Heal(int health)
    {
        this.health += health;

        OnHealthChanged();
    }

    public void Damage(int damage)
    {
        health -= damage;

        OnHealthChanged();
    }

    void OnHealthChanged()
    {
        healthUI.fillAmount = health / 100;

        if (health <= 0) Kill();
    }

    public void Kill() => Destroy(gameObject);
}
