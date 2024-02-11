using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum TeleporterState { Idle, IsReceivingPlayer, IsSendingPlayer, ReceivedPlayerIsOnTeleporter }

public class Teleporter : Mechanic
{
    public TeleporterState state;

    [SerializeField] Teleporter otherTeleporter;
    [SerializeField] BoxCollider2D trigger;
    [SerializeField] GameObject teleportationCarPrefab;

    [SerializeField] float stayDurationRequirredForTeleport;

    [SerializeField] float teleportationSpeed;
    [SerializeField] Ease teleportationEase;

    [SerializeField] float playerScaleDownDuration;
    [SerializeField] Ease playerScaleDownEase;
    [SerializeField] float playerScaleUpDuration;
    [SerializeField] Ease playerScaleUpEase;

    public PlayerController receivedPlayer;

    PlayerController playerOnTeleporter;
    bool playerIsOnTeleporter;
    float stayTime;
    Vector3 cachedPlayerScale;

    public override void Awake()
    {
        base.Awake();

        trigger = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (playerIsOnTeleporter)
        {
            stayTime += Time.deltaTime;

            if (stayTime >= stayDurationRequirredForTeleport)
            {
                playerIsOnTeleporter = false;
                stayTime = 0f;
                Teleport();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state != TeleporterState.Idle) return;

        if (collision.gameObject.GetComponent<PlayerController>() == null) return;  

        playerOnTeleporter = collision.gameObject.GetComponent<PlayerController>();
        if (playerOnTeleporter == null) return;
        if (playerOnTeleporter == receivedPlayer) return;

        playerIsOnTeleporter = true;
        //playerOnTeleporter = collision.gameObject.GetComponent<PlayerController>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() == null) return;

        if (state == TeleporterState.ReceivedPlayerIsOnTeleporter) state = TeleporterState.Idle;

        playerIsOnTeleporter = false;

        if (receivedPlayer == collision.gameObject.GetComponent<PlayerController>())
        {
            //trigger.enabled = true;
            receivedPlayer = null;
        }
    }

    void Teleport()
    {

        playerOnTeleporter.OnBeforTeleport();

        Debug.Log("Player " + playerOnTeleporter.GetPlayerNumber() + " is teleporting");

        GameObject teleportationCarInstance = Instantiate(teleportationCarPrefab, playerOnTeleporter.transform.position, Quaternion.identity);
        playerOnTeleporter.transform.SetParent(teleportationCarInstance.transform);
        playerOnTeleporter.transform.localPosition = Vector3.zero;
        AudioSource teleportationSoundAudioSource = teleportationCarInstance.transform.GetChild(0).GetComponent<AudioSource>();

        Color particlesColor = Color.white;
        particlesColor = playerOnTeleporter.GetPlayerColor();
        teleportationCarInstance.GetComponent<ParticleSystemRenderer>().material.color = particlesColor;

        state = TeleporterState.IsSendingPlayer;
        otherTeleporter.state = TeleporterState.IsReceivingPlayer;

        playerOnTeleporter.enabled = false;

        //trigger.enabled = false;
        //otherTeleporter.trigger.enabled = false;

        otherTeleporter.receivedPlayer = playerOnTeleporter;

        cachedPlayerScale = playerOnTeleporter.transform.localScale;

        playerOnTeleporter.transform.DOScale(0f, playerScaleDownDuration).SetEase(playerScaleDownEase).OnComplete(() =>
        {
            teleportationSoundAudioSource.Play();

            teleportationCarInstance.transform.DOMove(otherTeleporter.transform.position, teleportationSpeed * Vector3.Distance(transform.position, otherTeleporter.transform.position)).SetEase(teleportationEase).OnComplete(() =>
            {
                teleportationSoundAudioSource.Stop();

                playerOnTeleporter.transform.DOScale(cachedPlayerScale, playerScaleUpDuration).SetEase(playerScaleUpEase).OnComplete(() =>
                {
                    otherTeleporter.state = TeleporterState.ReceivedPlayerIsOnTeleporter;
                    state = TeleporterState.Idle;

                    playerOnTeleporter.transform.SetParent(null, true);
                    Destroy(teleportationCarInstance, 1f);

                    playerOnTeleporter.enabled = true;
                });
                
                //trigger.enabled = true;
                //otherTeleporter.trigger.enabled = true;
            });
        });
    }
}
