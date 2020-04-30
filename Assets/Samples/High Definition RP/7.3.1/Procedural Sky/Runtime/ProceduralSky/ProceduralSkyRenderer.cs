using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Samples.High_Definition_RP._7._3._1.Procedural_Sky.Runtime.ProceduralSky
{
    internal class ProceduralSkyRenderer : SkyRenderer
    {
        private Material m_ProceduralSkyMaterial;
        private MaterialPropertyBlock m_PropertyBlock = new MaterialPropertyBlock();

        private readonly int _SkyIntensity = Shader.PropertyToID("_SkyIntensity");
        private readonly int _PixelCoordToViewDirWS = Shader.PropertyToID("_PixelCoordToViewDirWS");
        private readonly int _SunSizeParam = Shader.PropertyToID("_SunSize");
        private readonly int _SunSizeConvergenceParam = Shader.PropertyToID("_SunSizeConvergence");
        private readonly int _AtmoshpereThicknessParam = Shader.PropertyToID("_AtmosphereThickness");
        private readonly int _SkyTintParam = Shader.PropertyToID("_SkyTint");
        private readonly int _GroundColorParam = Shader.PropertyToID("_GroundColor");
        private readonly int _SunColorParam = Shader.PropertyToID("_SunColor");
        private readonly int _SunDirectionParam = Shader.PropertyToID("_SunDirection");

        public ProceduralSkyRenderer()
        {
        }

        public override void Build()
        {
            var hdrp = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;
            m_ProceduralSkyMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/HDRP/Sky/ProceduralSky"));
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_ProceduralSkyMaterial);
        }

        public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
        {
            ProceduralSky skySettings = builtinParams.skySettings as ProceduralSky;
            CoreUtils.SetKeyword(m_ProceduralSkyMaterial, "_ENABLE_SUN_DISK", skySettings.enableSunDisk.value);

            // Default values when no sun is provided
            Color sunColor = Color.white;
            Vector3 sunDirection = Vector3.zero;
            float sunSize = 0.0f;

            if (builtinParams.sunLight != null)
            {
                sunColor = builtinParams.sunLight.color * builtinParams.sunLight.intensity;
                sunDirection = -builtinParams.sunLight.transform.forward;
                sunSize = skySettings.sunSize.value;
            }

            if (!renderSunDisk)
                sunSize = 0.0f;

            m_PropertyBlock.SetFloat(_SkyIntensity, GetSkyIntensity(skySettings, builtinParams.debugSettings));
            m_PropertyBlock.SetFloat(_SunSizeParam, sunSize);
            m_PropertyBlock.SetFloat(_SunSizeConvergenceParam, skySettings.sunSizeConvergence.value);
            m_PropertyBlock.SetFloat(_AtmoshpereThicknessParam, skySettings.atmosphereThickness.value);
            m_PropertyBlock.SetColor(_SkyTintParam, skySettings.skyTint.value);
            m_PropertyBlock.SetColor(_GroundColorParam, skySettings.groundColor.value);
            m_PropertyBlock.SetColor(_SunColorParam, sunColor);
            m_PropertyBlock.SetVector(_SunDirectionParam, sunDirection);
            m_PropertyBlock.SetMatrix(_PixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);

            CoreUtils.DrawFullScreen(builtinParams.commandBuffer, m_ProceduralSkyMaterial, m_PropertyBlock, renderForCubemap ? 0 : 1);
        }
    }
}
