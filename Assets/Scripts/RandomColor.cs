using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomColor : MonoBehaviour, IPool
{
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Mesh originalMesh;
    public Material[] materials;

    static Dictionary<Mesh, Dictionary<Color, Mesh>> meshLibrary = new Dictionary<Mesh, Dictionary<Color, Mesh>>();
    void Awake()
    { 
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        originalMesh = meshFilter.sharedMesh;

        if (meshLibrary.ContainsKey(originalMesh))
        {
            OnTake();
            return;
        }

        var dictionary = new Dictionary<Color, Mesh>();
        for (int i = 0; i < materials.Length; i++)
            dictionary.Add(materials[i].color, GenerateColoredMesh(originalMesh, materials[i].color));

        meshLibrary.Add(originalMesh, dictionary);

        OnTake();
    }
    public static Mesh GenerateColoredMesh(Mesh sharedMesh, Color color)
    {
        var count = 0;
        for (var s = 0; s < sharedMesh.subMeshCount; s++)
            count += sharedMesh.GetSubMesh(s).vertexCount;

        var colors = new Color[count];
        for (int i = 0; i < count; i++)
            colors[i] = color;

        var mesh = new Mesh
        {
            vertices = sharedMesh.vertices,
            triangles = sharedMesh.triangles,
            uv = null,
            normals = sharedMesh.normals,
            colors = colors
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    public void OnTake()
    {
        meshRenderer.material = materials[Random.Range(0, materials.Length)];
        //var newColor = materials[Random.Range(0, materials.Length)].color;
        //if (meshLibrary.TryGetValue(originalMesh, out var dictionary))
        //{
        //    if (dictionary.TryGetValue(newColor, out var newMesh))
        //        meshFilter.mesh = newMesh;
        //    else
        //        Debug.Log(newColor + " for " + originalMesh + " not generated");
        //}
    }

    public void OnPush()
    {
    }
#if UNITY_EDITOR
    public void PickMaterial()
    {
        var mat = GetComponent<MeshRenderer>().sharedMaterial;
        materials = new Material[1];
        materials[0] = mat;
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }
#endif
}
