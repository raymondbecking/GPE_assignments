using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/*This is the setup of a custom post processing effect
 */

//Allow unity to properly serialze parameter settings
[Serializable]
[PostProcess(typeof(DistortionRenderer), PostProcessEvent.BeforeStack, "Custom/Distortion")]
public class Distortion : PostProcessEffectSettings
{
    //The amount of distortion
    [Range(0f, 1.0f), Tooltip("Amount of distortion")]
    public FloatParameter Magnitude = new FloatParameter { value = 1.0f };

    //Texture Downscaling for memory saving
    [Range(0, 4), Tooltip("Texture downscaling")]
    public IntParameter DownScaleFactor = new IntParameter { value = 0 };

    //Allow viewing the distortion effect in debug view
    [Tooltip("Allow debugging view")]
    public BoolParameter DebugView = new BoolParameter { value = false };

}

public class DistortionRenderer : PostProcessEffectRenderer<Distortion>
{
    private int _globalDistortionTexID;
    private Shader _distortionShader;

    //Allow the use of the camera's depth texture
    public override DepthTextureMode GetCameraFlags()
    {
        return DepthTextureMode.Depth;
    }

    //Initialise some extra values
    public override void Init()
    {
        _globalDistortionTexID = Shader.PropertyToID("_GlobalDistortionTex");
        //Shader that does the actual distortion calculations
        _distortionShader = Shader.Find("Hidden/Custom/Distortion");
        //Also do the default init stuff
        base.Init();
    }

    
    public override void Render(PostProcessRenderContext context)
    {
        //Wrapper for material block
        var sheet = context.propertySheets.Get(_distortionShader);
        sheet.properties.SetFloat("_Magnitude", settings.Magnitude);

        if (!settings.DebugView)
        {
            //Get render target
            context.command.GetTemporaryRT(_globalDistortionTexID,
                context.camera.pixelWidth >> settings.DownScaleFactor,
                context.camera.pixelHeight >> settings.DownScaleFactor,
                0, FilterMode.Bilinear, RenderTextureFormat.RGFloat);
            //Set the render target
            context.command.SetRenderTarget(_globalDistortionTexID);
            //Clear the render target for reuse
            context.command.ClearRenderTarget(false, true, Color.clear);
        }
        //Call distortionmanager to add distortions to render pipeline
        DistortionManager.Instance.PopulateCommandBuffer(context.command);
        //Blit the distorted texture to the screen and voila!
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}

