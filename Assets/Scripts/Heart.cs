using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Mechanic
{
    [SerializeField] int healthAddition;
    [SerializeField] float rotationSpeed;
    [SerializeField] AudioClip collectSound;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerComponent = collision.gameObject.GetComponent<PlayerController>();

        if (playerComponent != null)
        {
            playerComponent.Heal(healthAddition);

            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position);

            Destroy(gameObject);
        }
    }
}
