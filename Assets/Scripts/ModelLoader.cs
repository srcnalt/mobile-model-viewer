using System;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

public class ModelLoader : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public static Material material;
    
    private void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void LoadModel(string path)
    {
        meshFilter.mesh = ObjImporter.CreateMesh(path);
        material = ObjImporter.CreateMaterial();    //will be reached from options menu
        meshRenderer.material = material;
        RepositionObject();
    }

    private void RepositionObject()
    {
        Vector3 bounds = meshFilter.mesh.bounds.extents;

        if (bounds == Vector3.zero)
        {
            Debug.Log("Object size is zero");
            return;
        }

        float max = Mathf.Max(new float[] { bounds.x, bounds.y, bounds.z });

        transform.localScale = Vector3.one / max;
        transform.position = Vector3.zero;

        transform.Translate(meshFilter.mesh.bounds.center / max * -1);

        Camera.main.transform.position = new Vector3(0, 0, -Screen.width / 200f);
    }

    private T[] GetElements<T>(IntPtr address, int arraySize)
    {
        int offset = 0;
        int size = Marshal.SizeOf(typeof(T));
        T[] elems = new T[arraySize];

        for (int i = 0; i < arraySize; i++)
        {
            elems[i] = (T)Marshal.PtrToStructure(new IntPtr(address.ToInt32() + offset), typeof(T));
            offset += size;
        }

        return elems;
    }

    private Texture Loadtexture(string path)
    {
        Texture2D texture = null;
        byte[] data;

        if (File.Exists(path))
        {
            data = File.ReadAllBytes(path);
            texture = new Texture2D(2, 2);
            texture.LoadImage(data);
        }

        return texture;
    }
}
