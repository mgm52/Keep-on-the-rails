using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMove : MonoBehaviour
{
    public float speed = 1f;
    public float amplitude = 1f;
    public float startPhase = Mathf.PI / 2.0f;

    public bool vertical = false;

    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float displacement = Mathf.Sin((startPhase + Time.time - startTime) * speed) * amplitude;
        if (vertical)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + displacement, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + displacement, transform.position.y, transform.position.z);
        }
    }
}
