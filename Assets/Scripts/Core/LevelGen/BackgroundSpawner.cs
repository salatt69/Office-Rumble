using System;
using System.Collections;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    [SerializeField] Material[] backgroundMaterials;
    [SerializeField] int selectedMaterialIndex;
    [SerializeField] ModularLevelGeneration levelGeneration;

    private GameObject backgroundQuad;
    private int lastValidIndex;

    void Start()
    {
        StartCoroutine(InitBackground());
    }

    IEnumerator InitBackground()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnBackground();
    }

    public void SpawnBackground()
    {
        if (backgroundMaterials == null || backgroundMaterials.Length == 0)
            return;

        lastValidIndex = Mathf.Clamp(selectedMaterialIndex, 0, backgroundMaterials.Length - 1);

        if (backgroundQuad != null)
            Destroy(backgroundQuad);

        backgroundQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        backgroundQuad.name = "BackgroundQuad";
        backgroundQuad.transform.SetParent(transform, false);

        ApplyMaterial(lastValidIndex);
        CenterAndScaleWithLevel();
    }

    void ApplyMaterial(int index)
    {
        if (backgroundQuad == null || backgroundMaterials == null)
            return;

        if (index >= 0 && index < backgroundMaterials.Length)
            backgroundQuad.GetComponent<Renderer>().material = backgroundMaterials[index];
    }

    public void CenterAndScaleWithLevel()
    {
        if (levelGeneration == null || backgroundQuad == null)
        {
            Debug.LogWarning("BackgroundSpawner: levelGeneration or backgroundQuad is null");
            return;
        }

        if (levelGeneration.TryGetSymmetricWorldBoundsFromStart(out Vector2 center, out Vector2 size))
        {
            Debug.Log($"BackgroundSpawner: center={center}, size={size}");
            backgroundQuad.transform.position = new Vector3(center.x, center.y, 10f);
            backgroundQuad.transform.localScale = new Vector3(size.x, size.y, 1f);
        }
        else
        {
            Debug.LogWarning("BackgroundSpawner: TryGetSymmetricWorldBoundsFromStart returned false");
        }
    }

    public void SetSelectedMaterial(int index)
    {
        if (backgroundMaterials == null || backgroundMaterials.Length == 0)
            return;

        if (index < 0 || index >= backgroundMaterials.Length)
        {
            Debug.LogWarning($"BackgroundSpawner: Material index {index} is out of bounds. Valid range: 0-{backgroundMaterials.Length - 1}. Keeping last valid index {lastValidIndex}.");
            return;
        }

        selectedMaterialIndex = index;
        lastValidIndex = index;
        ApplyMaterial(index);
    }

    public Material[] GetBackgroundMaterials() => backgroundMaterials;

    public string[] GetMaterialNames()
    {
        if (backgroundMaterials == null) return new string[0];
        return Array.ConvertAll(backgroundMaterials, m => m != null ? m.name : "Material");
    }
}