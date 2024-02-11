using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalType { Entrance, Exit, None }

public class Portal : MonoBehaviour
{
    public PortalType type;

    [SerializeField] Portal otherPortal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Clone") return;

        type = PortalType.Entrance;

        otherPortal.GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        otherPortal.type = PortalType.Exit;

        GameObject clone = Instantiate(collision.gameObject, otherPortal.transform.position, collision.gameObject.transform.rotation);
        clone.transform.SetParent(collision.gameObject.transform);
        clone.tag = "Clone";

        otherPortal.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<BoxCollider2D>().enabled = true;

        if (type == PortalType.Entrance) return;

        foreach (Transform child in collision.gameObject.transform)
        {
            if (!child.CompareTag("Clone")) continue;

            Destroy(child.gameObject);
        }
    }
}
