using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MaterialCycler : EditorWindow
{
    private Material[] materials = Array.Empty<Material>();

    // Track index per object so cycling is consistent when you click around
    private readonly Dictionary<int, int> indexByInstanceId = new Dictionary<int, int>();

    private int materialSlot = 0;
    private Vector2 scroll;

    [MenuItem("Tools/Material Cycler")]
    static void Open()
    {
        var w = GetWindow<MaterialCycler>("Material Cycler");
        w.minSize = new Vector2(320, 260);
        w.ReloadMaterials();
    }

    private void OnEnable()
    {
        ReloadMaterials();
        Selection.selectionChanged += Repaint;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
    }

    private void OnGUI()
    {
        HandleHotkeys();

        EditorGUILayout.LabelField("Material Cycler", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Reload materials", GUILayout.Height(28)))
                ReloadMaterials();

            GUILayout.Space(8);
            materialSlot = EditorGUILayout.IntField(new GUIContent("Slot"), materialSlot, GUILayout.Width(120));
        }

        EditorGUILayout.Space(8);

        var go = Selection.activeGameObject;
        if (go == null)
        {
            EditorGUILayout.HelpBox("Select a GameObject with a MeshRenderer.", MessageType.Info);
            DrawMaterialList();
            return;
        }

        var mr = go.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            EditorGUILayout.HelpBox($"Selected object '{go.name}' has no MeshRenderer.", MessageType.Warning);
            DrawMaterialList();
            return;
        }

        if (materials.Length == 0)
        {
            EditorGUILayout.HelpBox("No materials found in Assets/Materials. Click Reload.", MessageType.Warning);
            return;
        }

        int id = go.GetInstanceID();
        int idx = GetIndex(id);

        var shared = mr.sharedMaterials;
        int slots = shared != null ? shared.Length : 0;

        EditorGUILayout.LabelField("Selected", go.name);
        EditorGUILayout.LabelField("Renderer slots", slots.ToString());
        EditorGUILayout.LabelField("Current index", $"{idx} / {materials.Length - 1}");
        EditorGUILayout.LabelField("Current material", SafeName(materials[idx]));

        EditorGUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Prev (B)", GUILayout.Height(32)))
                Apply(go, mr, -1);

            if (GUILayout.Button("Next (N)", GUILayout.Height(32)))
                Apply(go, mr, +1);
        }

        EditorGUILayout.Space(10);
        DrawMaterialList();
    }

    private void HandleHotkeys()
    {
        var e = Event.current;
        if (e == null) return;

        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.N)
            {
                var go = Selection.activeGameObject;
                if (go) Apply(go, go.GetComponent<MeshRenderer>(), +1);
                e.Use();
            }
            else if (e.keyCode == KeyCode.B)
            {
                var go = Selection.activeGameObject;
                if (go) Apply(go, go.GetComponent<MeshRenderer>(), -1);
                e.Use();
            }
        }
    }

    private void ReloadMaterials()
    {
        indexByInstanceId.Clear();

        string folder = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            Debug.LogWarning($"[MaterialCycler] Folder not found: {folder}");
            materials = Array.Empty<Material>();
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { folder });
        var list = new List<Material>(guids.Length);

        Debug.Log($"[MaterialCycler] Found {guids.Length} material GUIDs in {folder}");

        foreach (string g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Debug.LogWarning($"[MaterialCycler] Failed to load material at path: {path}");
                continue;
            }

            list.Add(mat);
        }

        // stable ordering
        materials = list
            .Where(m => m != null)
            .OrderBy(m => m.name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Debug.Log($"[MaterialCycler] Loaded {materials.Length} materials. First: {SafeName(materials.FirstOrDefault())}, Last: {SafeName(materials.LastOrDefault())}");
    }

    private int GetIndex(int instanceId)
    {
        if (!indexByInstanceId.TryGetValue(instanceId, out int idx))
        {
            idx = 0;
            indexByInstanceId[instanceId] = idx;
        }
        idx = Mathf.Clamp(idx, 0, Mathf.Max(0, materials.Length - 1));
        return idx;
    }

    private void SetIndex(int instanceId, int idx)
    {
        if (materials.Length == 0) idx = 0;
        idx = Mod(idx, Mathf.Max(1, materials.Length));
        indexByInstanceId[instanceId] = idx;
    }

    private void Apply(GameObject go, MeshRenderer mr, int delta)
    {
        if (go == null || mr == null)
        {
            Debug.LogWarning("[MaterialCycler] No selected GameObject or MeshRenderer.");
            return;
        }

        if (materials.Length == 0)
        {
            Debug.LogWarning("[MaterialCycler] No materials loaded.");
            return;
        }

        int id = go.GetInstanceID();
        int idx = GetIndex(id);
        int next = Mod(idx + delta, materials.Length);
        var mat = materials[next];

        var shared = mr.sharedMaterials ?? Array.Empty<Material>();
        if (shared.Length == 0)
        {
            // Ensure at least one slot exists
            shared = new Material[1];
        }

        if (materialSlot < 0 || materialSlot >= shared.Length)
        {
            Debug.LogWarning($"[MaterialCycler] Slot {materialSlot} out of range. Renderer has {shared.Length} slots on '{go.name}'.");
            return;
        }

        Undo.RecordObject(mr, "Cycle Material");
        shared[materialSlot] = mat;
        mr.sharedMaterials = shared;
        EditorUtility.SetDirty(mr);

        SetIndex(id, next);

        Debug.Log($"[MaterialCycler] Applied '{mat.name}' (index {next}/{materials.Length - 1}) to '{go.name}' slot {materialSlot}");
        SceneView.RepaintAll();
        Repaint();
    }

    private void DrawMaterialList()
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField($"Materials loaded: {materials.Length}", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(120));
        for (int i = 0; i < materials.Length; i++)
        {
            var m = materials[i];
            EditorGUILayout.LabelField($"{i:000}: {SafeName(m)}");
        }
        EditorGUILayout.EndScrollView();
    }

    private static int Mod(int a, int m)
    {
        if (m <= 0) return 0;
        int r = a % m;
        return r < 0 ? r + m : r;
    }

    private static string SafeName(UnityEngine.Object o)
    {
        return o ? o.name : "<null>";
    }
}
