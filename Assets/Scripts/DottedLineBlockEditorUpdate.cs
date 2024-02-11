using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DottedLineBlockEditorUpdate : MonoBehaviour
{
    public DottedLineBlock block;

    private void Awake()
    {
        block.GetComponent<DottedLineBlock>();
    }

    void Update()
    {
        block.UpdateColor();
    }
}
