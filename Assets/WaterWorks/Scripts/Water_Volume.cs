using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material _material;
        private RTHandle tempRenderTarget;
        private RTHandle source;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
        }

        public void SetSource(RTHandle src)
        {
            source = src;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref tempRenderTarget, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TemporaryColorTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Reflection && _material != null && source != null)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get("Water Volume Pass");

                // Blit from source to temp with material
                Blitter.BlitCameraTexture(commandBuffer, source, tempRenderTarget, _material, 0);
                
                // Blit from temp back to source
                Blitter.BlitCameraTexture(commandBuffer, tempRenderTarget, source);

                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Cleanup is handled automatically by RTHandle system
        }

        public void Dispose()
        {
            tempRenderTarget?.Release();
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();
    private CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }
        
        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.SetSource(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
    }
}