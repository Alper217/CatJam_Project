using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NPCHighlight : MonoBehaviour
{
    private Renderer[] renderers;
    private Material[] originalMaterials;
    private Material[] highlightMaterials;
    private bool isHighlighted = false;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        List<Material> origMats = new List<Material>();
        List<Material> highlightMats = new List<Material>();

        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                origMats.Add(mat);

                // Sarý highlight materyali oluþtur
                Material highlightMat = new Material(mat);
                highlightMat.color = new Color(1f, 1f, 0f, 0.1f);
                highlightMat.SetFloat("_Metallic", 0f);
                highlightMat.SetFloat("_Glossiness", 0.8f);
                highlightMats.Add(highlightMat);
            }

        }

        originalMaterials = origMats.ToArray();
        highlightMaterials = highlightMats.ToArray();
    }

    public void SetHighlight(bool highlight)
    {
        if (isHighlighted == highlight) return;

        isHighlighted = highlight;
        int matIndex = 0;

        foreach (var renderer in renderers)
        {
            Material[] matsToApply = new Material[renderer.materials.Length];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                matsToApply[i] = highlight ? highlightMaterials[matIndex] : originalMaterials[matIndex];
                matIndex++;
            }

            renderer.materials = matsToApply;
        }
    }
}