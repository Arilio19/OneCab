using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialDummy : Mechanic, IDamageable, IKillable
{
    [SerializeField] int health;
    [SerializeField] GameObject killParticlesPrefab;
    [SerializeField] AudioClip killSound;

    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0) Kill();
    }

    public void Kill()
    {
        GameObject killParticles = Instantiate(killParticlesPrefab, transform.position, Quaternion.identity);

        Destroy(killParticles, 2f);

        AudioSource.PlayClipAtPoint(killSound, Camera.main.transform.position);

        transform.DOScale(0f, 0.1f);

        GameManager.Instance.OnTutorialDummyKilled(this);
    }
}
