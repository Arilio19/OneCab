using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int healthAddition;
    public float rotationSpeed;

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

            Destroy(gameObject);
        }
    }
}
