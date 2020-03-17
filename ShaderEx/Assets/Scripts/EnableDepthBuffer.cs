using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDepthBuffer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
