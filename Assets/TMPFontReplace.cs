using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
#if UNITY_EDITOR
using UnityEditor;
#endif
 
/*
https://bonnate.tistory.com/
 
Insert the script into the game object
insert the TMP font in the inspector
and press the button to find and replace all components.
 
It may work abnormally, so make sure to back up your scene before using it!!
*/
 
public class TMPFontReplace : MonoBehaviour
{
    [SerializeField] public TMP_FontAsset FontAsset;
}

#if UNITY_EDITOR
[CustomEditor(typeof(TMPFontReplace))]
public class TMPFontReplaceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Change Font!"))
        {
            TMP_FontAsset fontAsset = ((TMPFontReplace)target).FontAsset;

            foreach (TextMeshPro textMeshPro3D in GameObject.FindObjectsOfType<TextMeshPro>())
            {
                textMeshPro3D.font = fontAsset;
            }
            foreach (TextMeshProUGUI textMeshProUi in GameObject.FindObjectsOfType<TextMeshProUGUI>())
            {
                textMeshProUi.font = fontAsset;
            }
        }
    }
}
#endif