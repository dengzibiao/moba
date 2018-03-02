using UnityEngine;
using System.Collections;

public class FixParticle : MonoBehaviour
{
    public bool setRenderQueue;         //是否设置粒子的RenderQueue  
    public int effectRendererQueue;     //粒子RenderQueue的大小  

    public bool IsChangeAllRenderers = false;

    private void ChangeEffectRender()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        if (particles != null)
        {
            foreach (ParticleSystem myParticles in particles)
            {
                ChangeRendererSetting(myParticles.GetComponent<Renderer>());
            }
        }

        if (IsChangeAllRenderers)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                foreach (var myRenderer in renderers)
                {
                    ChangeRendererSetting(myRenderer);
                }
            }
        }
    }

    private void ChangeRendererSetting(Renderer render)
    {
        if (render == null)
        {
            Debug.LogWarning("------>null.");
            return;
        }
        if (setRenderQueue)
        {
            render.material.renderQueue = effectRendererQueue;
        }
    }

    void OnEnable()
    {
        ChangeEffectRender();
    }
}