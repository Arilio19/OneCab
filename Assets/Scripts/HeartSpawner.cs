using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSpawner : MonoBehaviour
{
    public GameObject heartPrefab;

    public GameObject cachedHeart;

    public float spawnDelay = 5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnHearts());
    }

    IEnumerator SpawnHearts()
    {
        while (true)
        {
            yield return null;

            if (cachedHeart != null) continue;

            yield return new WaitForSeconds(spawnDelay);

            GameObject newHeart = Instantiate(heartPrefab, transform.position, Quaternion.identity);
            newHeart.transform.SetParent(transform);
            newHeart.transform.localPosition = Vector3.zero;

            cachedHeart = newHeart;
        }   
    }
}
