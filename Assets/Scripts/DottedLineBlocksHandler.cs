using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DottedLineBlocksHandler : MonoBehaviour
{
    public static DottedLineBlocksHandler Instance;

    [SerializeField] float switchDelay;
    [SerializeField] DottedLineBlockColor currentActiveColor = DottedLineBlockColor.Green;
    [SerializeField] AudioClip switchSound1;
    [SerializeField] AudioClip switchSound2;

    private void Awake() => Instance = this;

    private void Start()
    {
        StartCoroutine(SwitchDottedLineBlocks());
    }

    IEnumerator SwitchDottedLineBlocks()
    {
        while (true)
        {
            currentActiveColor = currentActiveColor == DottedLineBlockColor.Green ? DottedLineBlockColor.Red : DottedLineBlockColor.Green;

            foreach (var block in DottedLineBlock.allDottedLineBlocks)
            {
                if (block.color == currentActiveColor) block.Open();
                else block.Close();
            }

            if (DottedLineBlock.allDottedLineBlocks.Count > 0) AudioSource.PlayClipAtPoint(currentActiveColor == DottedLineBlockColor.Red ? switchSound1 : switchSound2, Camera.main.transform.position, 0.9f);

            yield return new WaitForSeconds(switchDelay);
        }
    }
}
