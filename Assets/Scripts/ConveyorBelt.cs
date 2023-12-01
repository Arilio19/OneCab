using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float forceStrength;

    Vector2 force;

    public void Start()
    {
        if (transform.eulerAngles == Vector3.zero) force = new Vector2(0f, forceStrength);
        if (transform.eulerAngles.z == 90 || transform.eulerAngles.z == -270) force = new Vector2(forceStrength, 0f);
        if (transform.eulerAngles.z == 180 || transform.eulerAngles.z == -180) force = new Vector2(0f, -forceStrength);
        if (transform.eulerAngles.z == 270 || transform.eulerAngles.z == -90) force = new Vector2(-forceStrength, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<PlayerController>().AddForce(force);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<PlayerController>().AddForce(force);
    }
}
