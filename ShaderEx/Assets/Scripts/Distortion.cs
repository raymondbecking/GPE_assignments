using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//Allow unity to properly serialze parameter settings
[Serializable]
[PostProcess(typeof(DistortionRenderer), PostProcessEvent.BeforeStack, "Custom/Distortion")]
public class Distortion : PostProcessEffectSettings
{
    //The amount of distortion
    [Range(0f, 1.0f), Tooltip("The magnitude in texels of distortion fx.")]
    public FloatParameter Magnitude = new FloatParameter { value = 1.0f };

    //Texture Downscaling for memory saving
    [Range(0, 4), Tooltip("The down-scale factor to apply to the generated texture.")]
    public IntParameter DownScaleFactor = new IntParameter { value = 0 };

    //Allow viewing the distortion effect in debug view
    [Tooltip("Displays the Distortion Effects in debug view.")]
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

        DistortionManager.Instance.PopulateCommandBuffer(context.command);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}

