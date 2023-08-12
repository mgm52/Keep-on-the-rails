using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public float delay = 1;
    public float duration = 1;
    private float timeElapsed = 0;
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        timeElapsed += Time.deltaTime;

        float percentFromStart = (timeElapsed - delay) / duration;
        var alpha = Mathf.Min(1, percentFromStart);

        var textMesh = this.GetComponent<TMP_Text>();
        if(textMesh != null)
        {
            textMesh.color = new Color(textMesh.color.r,textMesh.color.g,textMesh.color.b,alpha);
        }

        var renderer = this.GetComponent<Renderer>();
        if(renderer != null)
        {
            var oldColor = renderer.material.color;
            renderer.material.color = new Color(oldColor.r,oldColor.g,oldColor.b, alpha);
        }
    }
}
