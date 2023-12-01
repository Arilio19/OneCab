using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Projectile : MonoBehaviour
{
    public float speed;

    public Vector2 direction;
    public int damage;

    public GameObject collisionParticlesPrefab;

    //Cached
    Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Vector3 shootDirection, Vector3 rotation, Color color)
    {
        direction = shootDirection;
        transform.eulerAngles = rotation;
        spriteRenderer.color = color;
    }

    void FixedUpdate()
    {
        Vector2 vel = new Vector2(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime);
        rigidbody.velocity = vel;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayImpactParticles();        

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null) damageable.Damage(damage);

        Destroy(gameObject);
    }

    void PlayImpactParticles()
    {
        GameObject particles = Instantiate(collisionParticlesPrefab, transform.position, Quaternion.identity);

        Color particlesColor = Color.white;
        particlesColor = spriteRenderer.GetComponent<SpriteRenderer>().color;
        particles.GetComponent<ParticleSystemRenderer>().material.color = particlesColor;
        Destroy(particles, 1f);
    }
}
