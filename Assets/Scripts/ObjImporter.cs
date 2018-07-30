using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ObjImporter
{
    private static List<Vector3> vertices;
    private static List<Vector3> normals;
    private static List<Vector2> uvs;
    private static List<int> tris;

    private static string materialName;
    private static string mainPath;

    public static string modelStats;

    private static void init()
    {
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();

        materialName = "";
        mainPath = "";
    }

    private static bool validateObjLine(string str)
    {
        return  str.IndexOf("f ") == 0 ||
                str.IndexOf("v ") == 0 ||
                str.IndexOf("vt ") == 0 ||
                str.IndexOf("vn ") == 0 ||
                str.IndexOf("mtllib ") == 0;
    }

    private static bool validateMatLine(string str)
    {
        return str.IndexOf("Kd ") == 0 ||
                str.IndexOf("Ks ") == 0 ||
                str.IndexOf("newmtl ") == 0 ||
                str.IndexOf("map_Kd ") == 0;
    }

    private static float toFloat(string str)
    {
        return System.Convert.ToSingle(str);
    }

    private static int toInt(string str)
    {
        return System.Convert.ToInt32(str);
    }

    public static Mesh CreateMesh(string path)
    {
        init();

        mainPath = path.Substring(0, path.LastIndexOfAny(new char[] {'\\', '/' }));

        int f = 0;

        List<Vector3> vertexList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        List<Vector3> normalList = new List<Vector3>();
        List<Vector3> faceList = new List<Vector3>();
        List<int> faceIndex = new List<int>();

        StreamReader stream = File.OpenText(path);
        string entireText = stream.ReadToEnd();
        stream.Close();

        StringReader reader = new StringReader(entireText);

        string line = reader.ReadLine();

        while (line != null)
        {
            if (validateObjLine(line))
            {
                string[] pieces = line.Split(' ');

                switch (pieces[0])
                {
                    case "mtllib":
                        materialName = pieces[1];
                        break;
                    case "v":
                        vertexList.Add(new Vector3(toFloat(pieces[1]), toFloat(pieces[2]), toFloat(pieces[3])));
                        break;
                    case "vt":
                        uvList.Add(new Vector3(toFloat(pieces[1]), toFloat(pieces[2])));
                        break;
                    case "vn":
                        normalList.Add(new Vector3(toFloat(pieces[1]), toFloat(pieces[2]), toFloat(pieces[3])));
                        break;
                    case "f":
                        int j = 1;
                        faceIndex.Clear();

                        while (j < pieces.Length && 0 < pieces[j].Length)
                        {
                            Vector3 temp = new Vector3();
                            string[] facePieces = pieces[j].Split('/');
                            temp.x = toInt(facePieces[0]);

                            if(facePieces.Length > 1)
                            {
                                temp.y = facePieces[1] != "" ? toInt(facePieces[1]) : 0;
                                temp.z = toInt(facePieces[2]);
                            }

                            faceList.Add(temp);
                            faceIndex.Add(f);

                            j++;
                            f++;
                        }

                        j = 1;

                        while (j + 2 < pieces.Length)
                        {
                            tris.Add(faceIndex[0]);
                            tris.Add(faceIndex[j]);
                            tris.Add(faceIndex[j + 1]);

                            j++;
                        }
                        break;
                }
            }

            line = reader.ReadLine();
        }

        for (int i = 0; i < faceList.Count; i++)
        {
            vertices.Add(vertexList[(int)faceList[i].x - 1]);

            if (faceList[i].y >= 1)
            {
                uvs.Add(uvList[(int)faceList[i].y - 1]);
            }

            if (faceList[i].z >= 1)
            {
                normals.Add(normalList[(int)faceList[i].z - 1]);
            }
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = tris.ToArray();

        mesh.RecalculateBounds();

        //build stats string
        modelStats = "";
        modelStats += "Vertex Count:\t\t" + vertices.Count + "\n";
        modelStats += "Triangle Count:\t\t" + tris.Count + "\n";
        modelStats += "Model Bounds:\t\t" + mesh.bounds.size.ToString();

        return mesh;
    }

    public static Material CreateMaterial()
    {
        Color albedo = new Color();
        Texture texture = null;
        float smoothness = 0;

        StreamReader stream;

        try
        {
            stream = File.OpenText(mainPath + "/" + materialName);
        }
        catch (System.Exception)
        {
            return new Material(Shader.Find("Standard"));
        }
        
        string entireText = stream.ReadToEnd();
        stream.Close();

        StringReader reader = new StringReader(entireText);

        string line = reader.ReadLine();

        while (line != null)
        {
            if (validateMatLine(line))
            {
                string[] pieces = line.Split(' ');

                switch (pieces[0])
                {
                    case "Kd":
                        albedo = new Color(toFloat(pieces[1]), toFloat(pieces[2]), toFloat(pieces[3]));
                        break;
                    case "Ks":
                        smoothness = (toFloat(pieces[1]) + toFloat(pieces[2]) + toFloat(pieces[3])) / 3;
                        break;
                    case "map_Kd":
                        texture = LoadTexture(pieces[1]);
                        break;
                }
            }
        }

        Material material = new Material(Shader.Find("Standard"));
        material.SetColor("_Color", albedo);
        material.SetFloat("_Glossiness", smoothness);
        material.mainTexture = texture;

        return material;
    }

    private static Texture LoadTexture(string path)
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
