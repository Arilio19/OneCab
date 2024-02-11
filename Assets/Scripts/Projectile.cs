using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Projectile : Mechanic
{
    public float speed;

    public Vector2 direction;
    public int damage;

    public GameObject collisionParticlesPrefab;

    public PlayerNumber fromPlayer;

    //Cached
    Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Vector3 shootDirection, Vector3 rotation, Color color, PlayerNumber fromPlayer)
    {
        direction = shootDirection;
        transform.eulerAngles = rotation;
        spriteRenderer.color = color;
        this.fromPlayer = fromPlayer;

        if (GameManager.Instance.state == GameState.ButtonsTutorial)
        {
            transform.localScale *= 3f;
            speed = 1200f;
        }
    }

    void FixedUpdate()
    {
        Vector2 vel = new Vector2(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime);
        rigidbody.velocity = vel;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayImpactParticles();

        PlayerController playerComp = collision.gameObject.GetComponent<PlayerController>();

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null && playerComp != null && playerComp.GetPlayerNumber() != fromPlayer) damageable.Damage(damage);
        else if (damageable != null && playerComp != null && playerComp.GetPlayerNumber() == fromPlayer) goto skip;
        else if (damageable != null) damageable.Damage(damage);

        skip:
        Destroy(gameObject);
    }

    void PlayImpactParticles()
    {
        GameObject particles = Instantiate(collisionParticlesPrefab, transform.position, Quaternion.identity);

        if (GameManager.Instance.state == GameState.ButtonsTutorial) particles.transform.localScale *= 2f;

        Color particlesColor = Color.white;
        particlesColor = spriteRenderer.GetComponent<SpriteRenderer>().color;
        particles.GetComponent<ParticleSystemRenderer>().material.color = particlesColor;
        Destroy(particles, 1f);
    }
}
