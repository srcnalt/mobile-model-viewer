using UnityEngine;

public class ModelImporter : MonoBehaviour {
    
    void Start()
    {
        Mesh holderMesh = new Mesh();
        ObjImporter newMesh = new ObjImporter();
        holderMesh = newMesh.ImportFile("C:/Users/Sarge/Desktop/test.obj");

        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();

        filter.mesh = holderMesh;
        
        Vector3 v = holderMesh.bounds.extents;
        float max = Mathf.Max(new float[] { v.x, v.y, v.z });

        transform.localScale = Vector3.one / max;

        transform.Translate(holderMesh.bounds.center / max * -1);
    }
}
