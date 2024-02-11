using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum DottedLineBlockColor { Green, Red }

public class DottedLineBlock : Mechanic
{
    public static List<DottedLineBlock> allDottedLineBlocks = new List<DottedLineBlock>();

    public DottedLineBlockColor color;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer block;
    [SerializeField] Color greenColor;
    [SerializeField] Color redColor;

    [Header("Animation")]
    [SerializeField] float scaleUpDuration;
    [SerializeField] Ease scaleUpEase;
    [SerializeField] float scaleDownDuration;
    [SerializeField] Ease scaleDownEase;

    public override void Awake()
    {
        base.Awake();

        if (!allDottedLineBlocks.Contains(this)) allDottedLineBlocks.Add(this);

        UpdateColor();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (!allDottedLineBlocks.Contains(this)) allDottedLineBlocks.Add(this);
    }

    public void UpdateColor()
    {
        block.color = color == DottedLineBlockColor.Green ? greenColor : redColor;
    }

    public void Open()
    {
        block.transform.DOScale(1f, scaleUpDuration).SetEase(scaleUpEase);
    }

    public void Close()
    {
        block.transform.DOScale(0f, scaleDownDuration).SetEase(scaleDownEase);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (allDottedLineBlocks.Contains(this)) allDottedLineBlocks.Remove(this);
    }
}
