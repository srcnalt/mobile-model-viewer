using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public Text statsText;
    public Slider morphSlider;
    public MeshRenderer meshRenderer;
    private Material defaultMaterial;

    public void SwitchShader(Material material)
    {
        meshRenderer.material = material;
    }

    public void SetToDefaultShader()
    {
        StartCoroutine(Co_LoadDefaultMaterial());
    }

    private IEnumerator Co_LoadDefaultMaterial()
    {
        while(defaultMaterial == null)
        {
            defaultMaterial = ModelLoader.material;

            yield return null;
        }

        meshRenderer.material = defaultMaterial;
    }

    public void GetModelStats()
    {
        statsText.text = ObjImporter.modelStats;
    }

    private void ChangeMorphAmount()
    {
        meshRenderer.material.SetFloat("_Value", morphSlider.value);
    }
}
