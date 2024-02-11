using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MovingBlock : Mechanic
{
    [SerializeField] GameObject block;
    [SerializeField] GameObject gear;
    [SerializeField] Vector3 gearRotationAxis;
    [SerializeField] float gearRotationSpeed;
    [SerializeField] float gearRotationDirection = 1f;
    [SerializeField] Transform waypointsParent;

    [SerializeField] float startDelay;
    [SerializeField] float movementDurationBetweenWaypoints;
    [SerializeField] Ease movementEase;
    [SerializeField] LoopType loopType;

    private void Start()
    {
        List<Vector3> convertedWaypoints = new List<Vector3>();
        foreach (Transform child in waypointsParent) convertedWaypoints.Add(child.transform.localPosition);
        if (loopType == LoopType.Restart) convertedWaypoints.Add(convertedWaypoints.First());

        block.transform.DOLocalPath(convertedWaypoints.ToArray(), movementDurationBetweenWaypoints * convertedWaypoints.Count, PathType.Linear)
                       .SetId("BlockIsMoving")
                       .SetEase(movementEase)
                       .SetDelay(startDelay)
                       .SetLoops(-1, loopType)
                       .OnStepComplete(() =>
                       {
                           if (loopType == LoopType.Yoyo) gearRotationDirection *= -1f;
                       });
    }

    private void Update()
    {
        if (DOTween.IsTweening("BlockIsMoving")) gear.transform.Rotate(gearRotationAxis * gearRotationDirection, gearRotationSpeed * Time.deltaTime);
    }
}
