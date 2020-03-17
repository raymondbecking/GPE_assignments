using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Component for regestering a renderer 
[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class DistortionEffect : MonoBehaviour
{
    //Attached Renderer
    public Renderer Renderer { get; private set; }

    //Material on the renderer
    public Material Material { get; private set; }

    //When enabled, caches and registers values with the manager
    private void OnEnable()
    {
        Renderer = GetComponent<Renderer>();
        //Renderer not needed, because only the Post process pass should use this effect
        Renderer.enabled = false;
        Material = Renderer.sharedMaterial;
        //Register the material where this script is attached to, as an Distortion Effect in the _distortionEffects list
        DistortionManager.Instance.Register(this);
    }

    //Deregister effect
    private void OnDisable()
    {
        DistortionManager.Instance.Deregister(this);
    }
}
