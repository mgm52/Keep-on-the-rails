using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class NumberText : MonoBehaviour
{

    public TMPro.TMP_Text changeIndicator;
    public TMPro.TMP_Text numberText;

    public Vector3 indicatorMoveDisplacement = new Vector3(0, 0.25f, 0);
    public float indicatorMoveDuration = 3f;
    public float indicatorFadeDuration = 3f;

    public bool fade = true;

    private Vector3 originalPos;

    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        //originalPos = changeIndicator.transform.localPosition;
        originalPos = new Vector3(0.36f, 0f, 0f);
        changeIndicator.enabled = false;
        Debug.Log("Original local position is " + originalPos);
        Debug.Log("Original position is " + changeIndicator.transform.position);
    }

    public void SetText(string value)
    {
        numberText.text = value;
    }

    public void IndicateChange(int change)
    {
        changeIndicator.DOComplete();
        changeIndicator.transform.DOComplete();

        changeIndicator.transform.localPosition = originalPos;
        changeIndicator.enabled = false;

        if (change != 0)
        {
            changeIndicator.enabled = true;
            if (change > 0)
            {
                changeIndicator.text = "+" + change.ToString();
                changeIndicator.color = positiveColor;
            }
            else if (change < 0)
            {
                changeIndicator.text = change.ToString();
                changeIndicator.color = negativeColor;
            }
            if (fade)
            {
                // Set opacity to 1
                changeIndicator.DOFade(1f, 0f);
                //Vector3 originalPos = changeIndicator.transform.localPosition;

                // Fade out then disappear
                changeIndicator.DOFade(0, indicatorFadeDuration).SetEase(Ease.InOutSine).OnComplete(() => changeIndicator.enabled = false);
                // Move
                changeIndicator.transform.DOLocalMove(originalPos + indicatorMoveDisplacement, indicatorMoveDuration).SetEase(Ease.InOutSine);
            }
            else
            {
                // Move then disappear
                changeIndicator.transform.DOLocalMove(originalPos + indicatorMoveDisplacement, indicatorMoveDuration).SetEase(Ease.InOutSine).OnComplete(() => changeIndicator.enabled = false);
            }
            changeIndicator.transform.DOLocalMove(originalPos, 0f).SetDelay(indicatorMoveDuration);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
