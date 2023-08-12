using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlideIn : MonoBehaviour
{
    public Vector2 startOffset;
    public float moveDelay = 0f;
    public float moveDuration = 1f;

    public bool localPosition = false;

    // Start is called before the first frame update
    void Start()
    {
        if (localPosition)
        {
            transform.localPosition = transform.localPosition + (Vector3)startOffset;
            transform.DOLocalMove(transform.localPosition - (Vector3)startOffset, moveDuration).SetDelay(moveDelay);
        }
        else
        {
            transform.position = transform.position + (Vector3)startOffset;
            transform.DOMove(transform.position - (Vector3)startOffset, moveDuration).SetDelay(moveDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
