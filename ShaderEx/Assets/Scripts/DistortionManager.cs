using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class DistortionManager
{
    #region Singleton
    //What is a singleton?

    private static DistortionManager _instance;
    
    //Allows instancing
    public static DistortionManager Instance
    {
        get
        {
            return _instance = _instance ?? new DistortionManager();
        }
    }

    #endregion

    //List of distortion effects
    private readonly List<DistortionEffect> _distortionEffects = new List<DistortionEffect>();

    //Register distortion effect
    public void Register(DistortionEffect distortionEffect)
    {
        _distortionEffects.Add(distortionEffect);
    }

    //Deregister distortion effect
    public void Deregister(DistortionEffect distortionEffect)
    {
        _distortionEffects.Remove(distortionEffect);
    }

    //Commands to draw the distortion effect list to the commandbuffer(?)
    public void PopulateCommandBuffer(CommandBuffer commandBuffer)
    {
        for(int i = 0, len = _distortionEffects.Count; i < len; i++)
        {
            //Get each effect in the list
            var effect = _distortionEffects[i];
            commandBuffer.DrawRenderer(effect.Renderer, effect.Material);
        }
    }

}
