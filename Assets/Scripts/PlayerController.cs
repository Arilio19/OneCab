using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;
using DG.Tweening;
using UnityEngine.Rendering;

public enum PlayerNumber { One, Two }

public class PlayerController : Mechanic, IDamageable, IKillable
{
    [SerializeField] PlayerNumber playerNumber;

    [Header("Physics")]
    [SerializeField] float speed = 8f;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float projectileFireCooldown;
    [SerializeField] float accelaration;
    [SerializeField] float accelarationDuration;
    [SerializeField] Ease accelarationEase;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileFireLocation;
    [SerializeField] GameObject weaponParent;
    [SerializeField] Image healthUI;
    [SerializeField] GameObject playerKillParticlesPrefab;

    [Header("Visuals")]
    [SerializeField] Color projectileColor;

    [Header("Audio")]
    [SerializeField] AudioSource weaponShootAudioSource;
    [SerializeField] AudioClip killSound;
    [SerializeField] AudioSource hitAudioSource;
    
    //Cached Variables
    Rigidbody2D rigidbody;
    bool isFiring;
    float cachedHealth;
    KeyCode leftInput;
    KeyCode rightInput;
    KeyCode upInput;
    KeyCode downInput;
    KeyCode fireInput;
    Vector2 velocity;

    public override void Awake() { base.Awake(); rigidbody = GetComponent<Rigidbody2D>(); }

    void Start()
    {
        StartCoroutine(FireProjectiles());

        InitializeInputKeys();

        cachedHealth = maxHealth;
    }

    void InitializeInputKeys()
    {
        int playerNumber = (int)this.playerNumber + 1;

        if (playerNumber == 1)
        {
            leftInput = KeyCode.A;
            rightInput = KeyCode.D;
            upInput = KeyCode.W;
            downInput = KeyCode.S;

            fireInput = KeyCode.Space;
        }
        else
        {
            leftInput = KeyCode.LeftArrow;
            rightInput = KeyCode.RightArrow;
            upInput = KeyCode.UpArrow;
            downInput = KeyCode.DownArrow;

            fireInput = KeyCode.Return;
        }
    }

    void Update()
    {
        if (GameManager.Instance.state == GameState.PlayerKilled) { isFiring = false; return; }

        UpdateInput();  

        UpdateWeaponRotation();
    }

    void UpdateInput()
    {
        

        if (Input.GetKeyDown(leftInput) || Input.GetKey(leftInput)) velocity.x = Mathf.Clamp(velocity.x -= accelaration * Time.deltaTime, -1f, 0f);
        if (Input.GetKeyDown(rightInput) || Input.GetKey(rightInput)) velocity.x = Mathf.Clamp(velocity.x += accelaration * Time.deltaTime, 0f, 1f);
        if (!Input.GetKey(rightInput) && !Input.GetKey(leftInput) && velocity.x != 0f) velocity.x = Mathf.Clamp(velocity.x += velocity.x < 0f ? accelaration * Time.deltaTime : -accelaration * Time.deltaTime, -1f, 0f);

        if (Input.GetKeyDown(downInput) || Input.GetKey(downInput)) velocity.y = Mathf.Clamp(velocity.y -= accelaration * Time.deltaTime, -1f, 0f);
        if (Input.GetKeyDown(upInput) || Input.GetKey(upInput)) velocity.y = Mathf.Clamp(velocity.y += accelaration * Time.deltaTime, -0f, 1f);
        if (!Input.GetKey(upInput) && !Input.GetKey(downInput) && velocity.y != 0f) velocity.y = Mathf.Clamp(velocity.y += velocity.y < 0f ? accelaration * Time.deltaTime : -accelaration * Time.deltaTime, -1f, 0f);

        if (Input.GetKeyDown(fireInput) || Input.GetKey(fireInput)) isFiring = true;
        else isFiring = false;
    }

    IEnumerator FireProjectiles()
    {
        while (true)
        {
            yield return null;

            if (!isFiring) { weaponShootAudioSource.Pause(); continue; }

            GameObject projectile = Instantiate(projectilePrefab, projectileFireLocation.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize((projectileFireLocation.position - weaponParent.transform.position).normalized, weaponParent.transform.eulerAngles, projectileColor, playerNumber);

            weaponShootAudioSource.UnPause();

            yield return new WaitForSeconds(projectileFireCooldown);
        }
    }

    void UpdateWeaponRotation()
    {
        if (velocity.y >= 1) weaponParent.transform.localEulerAngles = Vector3.zero;
        if (velocity.y <= -1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        if (velocity.x >= 1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
        if (velocity.x <= -1) weaponParent.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.state == GameState.PlayerKilled) return;

        Vector2 velocity = rigidbody.velocity;
        velocity = this.velocity * speed * Time.deltaTime;
        rigidbody.velocity = velocity;
    }

    public void AddForce(Vector2 force) => rigidbody.AddForce(force);

    public void Heal(int health)
    {
        this.cachedHealth += health;

        if (this.cachedHealth > maxHealth) cachedHealth = maxHealth;

        OnHealthChanged();
    }

    public void Damage(int damage)
    {
        if (GameManager.Instance.state == GameState.ButtonsTutorial) return;

        if (hitAudioSource.isPlaying) hitAudioSource.Stop();
        hitAudioSource.Play();

        cachedHealth -= damage;

        OnHealthChanged();
    }

    void OnHealthChanged()
    {
        healthUI.fillAmount = cachedHealth / 100;

        if (cachedHealth <= 0) Kill();
    }

    public void Kill()
    {
        GameManager.Instance.OnPlayerKilled(playerNumber);

        GameObject killParticles = Instantiate(playerKillParticlesPrefab, transform.position, Quaternion.identity);

        Color particlesColor = Color.white;
        particlesColor = projectileColor;
        killParticles.GetComponent<ParticleSystemRenderer>().material.color = particlesColor;
        Destroy(killParticles, 1f);

        AudioSource.PlayClipAtPoint(killSound, Camera.main.transform.position);

        transform.DOScale(0f, 0.1f);
    }

    public Color GetPlayerColor() => projectileColor;

    public PlayerNumber GetPlayerNumber() => playerNumber;

    public override void OnMatchStart()
    {
        base.OnMatchStart();

        cachedHealth = maxHealth;
        OnHealthChanged();
    }

    public void OnBeforTeleport()
    {
        rigidbody.velocity = Vector2.zero;
        isFiring = false;
    }
}
