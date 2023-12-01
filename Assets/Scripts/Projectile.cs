using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    public Vector2 direction;
    public float damage;

    public GameObject collisionParticlesPrefab;

    Rigidbody2D rb;
    SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Vector3 shootDirection, Vector3 rotation, Color color)
    {
        direction = shootDirection;
        transform.eulerAngles = rotation;
        sr.color = color;
    }

    private void FixedUpdate()
    {
        Vector2 vel = new Vector2(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime);
        rb.velocity = vel;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject particles = Instantiate(collisionParticlesPrefab, transform.position, Quaternion.identity);

        Color particlesColor = Color.white;
        particlesColor = sr.GetComponent<SpriteRenderer>().color;
        particles.GetComponent<ParticleSystemRenderer>().material.color = particlesColor;
        Destroy(particles, 1f);

        Destroy(gameObject);
    }
}
