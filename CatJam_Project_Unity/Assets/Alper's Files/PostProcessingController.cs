using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    private Volume volume;
    private DepthOfField dof;
    private Bloom bloom;
    private ChromaticAberration chromatic;
    private LensDistortion lens;

    StressManager stressManager;

    void Start()
    {
        stressManager = StressManager.instance;

        volume = GetComponent<Volume>();
        if (volume.profile.TryGet(out dof) &&
            volume.profile.TryGet(out bloom) &&
            volume.profile.TryGet(out chromatic) &&
            volume.profile.TryGet(out lens))
        {
            // Baþlangýçta her þeyi kapat
            dof.active = false;
            bloom.active = false;
            chromatic.active = false;
            lens.active = false;
        }
        else
        {
            Debug.LogWarning("Post process efektlerinden bazýlarý eksik.");
        }
    }

    void Update()
    {
        float stress = stressManager.stressLevel;

        if (stress <= 4f)
        {
            // Low stress - No effects
            dof.active = false;
            bloom.active = false;
            chromatic.active = false;
            lens.active = false;
        }
        else if (stress > 4f && stress <= 8f)
        {
            // Medium stress - Light blur
            dof.active = true;
            bloom.active = true;
            chromatic.active = true;
            lens.active = true;

            dof.mode.value = DepthOfFieldMode.Gaussian;
            dof.gaussianStart.value = 0.1f;
            dof.gaussianEnd.value = 1f;
            dof.highQualitySampling.value = true;

            bloom.intensity.value = 1.2f;
            chromatic.intensity.value = 0.2f;
            lens.intensity.value = -0.2f;
        }
        else if (stress > 8f)
        {
            // High stress - Strong blur and distortion
            dof.active = true;
            bloom.active = true;
            chromatic.active = true;
            lens.active = true;

            dof.mode.value = DepthOfFieldMode.Gaussian;
            dof.gaussianStart.value = 0f;
            dof.gaussianEnd.value = 0.5f;
            dof.highQualitySampling.value = true;

            bloom.intensity.value = 2.5f;
            chromatic.intensity.value = 0.5f;
            lens.intensity.value = -0.4f;
        }
    }
}
