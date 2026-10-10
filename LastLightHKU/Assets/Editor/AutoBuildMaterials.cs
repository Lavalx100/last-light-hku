using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AutoBuildMaterials
{
    [MenuItem("Tools/Auto Build Materials")]
    public static void Run()
    {
        string texturePath = "Assets"; // scans whole project
        var textures = AssetDatabase.FindAssets("t:Texture2D", new[] { texturePath });

        // Group textures by base name
        Dictionary<string, Dictionary<string, Texture2D>> groups =
            new Dictionary<string, Dictionary<string, Texture2D>>();

        foreach (string guid in textures)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);

            // Normalize name
            string key = Regex.Replace(name, @"(_diffuse.*|_color.*|_colour.*|_normal.*|_ao.*|_occlusion.*)$", "", RegexOptions.IgnoreCase);

            if (!groups.ContainsKey(key))
                groups[key] = new Dictionary<string, Texture2D>();

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            string lname = name.ToLower();
            if (lname.Contains("normal"))
                groups[key]["normal"] = tex;
            else if (lname.Contains("ao") || lname.Contains("occlusion"))
                groups[key]["ao"] = tex;
            else
                groups[key]["base"] = tex;
        }

        // Create materials
        Dictionary<string, Material> materials = new Dictionary<string, Material>();

        foreach (var kv in groups)
        {
            string matName = kv.Key;
            string matPath = $"Assets/Materials/{matName}.mat";

            if (!Directory.Exists("Assets/Materials"))
                Directory.CreateDirectory("Assets/Materials");

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (!mat)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, matPath);
            }

            if (kv.Value.ContainsKey("base"))
                mat.SetTexture("_BaseMap", kv.Value["base"]);

            if (kv.Value.ContainsKey("normal"))
            {
                mat.EnableKeyword("_NORMALMAP");
                mat.SetTexture("_BumpMap", kv.Value["normal"]);
            }

            materials[matName] = mat;
        }

        // Assign materials to meshes
        string[] meshes = AssetDatabase.FindAssets("t:Mesh");

        foreach (string guid in meshes)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);

            if (!mesh) continue;

            foreach (var kv in materials)
            {
                if (mesh.name.Contains(kv.Key))
                {
                    var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                    if (importer != null)
                    {
                        importer.materialImportMode = ModelImporterMaterialImportMode.None;
                        importer.SaveAndReimport();
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ Auto material build complete");
    }
}
