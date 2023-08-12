using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysTurn : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate on Z axis
        transform.Rotate(0, 0, -60 * Time.deltaTime);
    }
}
