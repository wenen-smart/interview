using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoSampleClipEditor : MonoBehaviour
{
    CharacterAnim characterAnim;
    public void Awake()
    {
        characterAnim = GetComponent<CharacterAnim>();
    }
    public void Update()
    {
        if (characterAnim)
        {
            characterAnim.EditorUpdate();
        }
    }
}
