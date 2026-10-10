using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildPublicMacOS
{
    private const string MainGameScene = "Assets/Scenes/MainGame.unity";
    private const string PlaceholderMaterialsPath = "Assets/Materials/ReleasePlaceholders";

    public static void Build()
    {
        var outputPath = Environment.GetEnvironmentVariable("LASTLIGHT_BUILD_OUTPUT");
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new InvalidOperationException(
                "Set LASTLIGHT_BUILD_OUTPUT to the absolute .app output path.");
        }

        ReplaceExcludedAssets();

        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new InvalidOperationException("No enabled scenes are configured for the build.");
        }

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.StrictMode
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"macOS build failed with result {report.summary.result}.");
        }

        Debug.Log(
            $"Built {PlayerSettings.productName} to {report.summary.outputPath} " +
            $"({report.summary.totalSize} bytes).");
    }

    private static void ReplaceExcludedAssets()
    {
        var scene = EditorSceneManager.OpenScene(MainGameScene, OpenSceneMode.Single);
        var missingInstances = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
            .Where(transform => PrefabUtility.GetPrefabInstanceStatus(transform.gameObject) ==
                                PrefabInstanceStatus.MissingAsset)
            .Select(transform => transform.gameObject)
            .ToArray();

        if (missingInstances.Length == 0)
        {
            return;
        }

        var bark = CreateMaterial("TreeBark", "#594333");
        var foliage = CreateMaterial("TreeFoliage", "#314B3D");
        var accent = CreateMaterial("RelicAccent", "#D9A441");
        var fabric = CreateMaterial("RelicFabric", "#777B88");
        var bookCover = CreateMaterial("RelicBookCover", "#536C83");
        var spotlightHousing = CreateMaterial("SpotlightHousing", "#3A3D42");

        var replacementCount = 0;
        foreach (var excludedObject in missingInstances)
        {
            if (excludedObject == null)
            {
                continue;
            }

            var oldTransform = excludedObject.transform;
            var parent = oldTransform.parent;
            var localPosition = oldTransform.localPosition;
            var localRotation = oldTransform.localRotation;
            var localScale = oldTransform.localScale;
            var siblingIndex = oldTransform.GetSiblingIndex();
            var originalName = Regex.Replace(
                excludedObject.name,
                @" \(Missing Prefab with guid: [0-9a-f]+\)$",
                string.Empty,
                RegexOptions.IgnoreCase);

            if (originalName.StartsWith(
                    "Placeholder for referenced GameObject in Prefab instance",
                    StringComparison.OrdinalIgnoreCase))
            {
                UnityEngine.Object.DestroyImmediate(excludedObject);
                replacementCount++;
                continue;
            }

            var replacement = new GameObject(originalName);
            var replacementTransform = replacement.transform;
            replacementTransform.SetParent(parent, false);
            replacementTransform.localPosition = localPosition;
            replacementTransform.localRotation = localRotation;
            replacementTransform.localScale = localScale;
            replacementTransform.SetSiblingIndex(siblingIndex);

            foreach (Transform child in oldTransform)
            {
                child.SetParent(replacementTransform, true);
            }

            if (originalName.StartsWith("Poplar_Tree", StringComparison.OrdinalIgnoreCase))
            {
                CreateTree(replacement, false, bark, foliage);
            }
            else if (originalName.StartsWith("Fir_Tree", StringComparison.OrdinalIgnoreCase))
            {
                CreateTree(replacement, true, bark, foliage);
            }
            else if (originalName.StartsWith("Spotlight", StringComparison.OrdinalIgnoreCase))
            {
                CreateSpotlight(replacement, spotlightHousing);
            }
            else if (Enum.TryParse(originalName, true, out RelicId relicId))
            {
                CreateRelic(replacement, relicId, accent, fabric, bookCover, bark);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(replacement);
                throw new InvalidOperationException(
                    $"No public replacement is defined for excluded prefab '{originalName}'.");
            }

            UnityEngine.Object.DestroyImmediate(excludedObject);
            replacementCount++;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            throw new InvalidOperationException($"Could not save {MainGameScene}.");
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Replaced {replacementCount} excluded prefab instances with original placeholders.");
    }

    private static Material CreateMaterial(string name, string colorHex)
    {
        if (!AssetDatabase.IsValidFolder(PlaceholderMaterialsPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "ReleasePlaceholders");
        }

        var assetPath = $"{PlaceholderMaterialsPath}/{name}.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (existing != null)
        {
            return existing;
        }

        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        if (shader == null)
        {
            throw new InvalidOperationException("Could not find a shader for public placeholder materials.");
        }

        if (!ColorUtility.TryParseHtmlString(colorHex, out var color))
        {
            throw new InvalidOperationException($"Invalid color value '{colorHex}'.");
        }

        var material = new Material(shader)
        {
            name = name,
            color = color
        };
        AssetDatabase.CreateAsset(material, assetPath);
        return material;
    }

    private static void CreateTree(GameObject root, bool isFir, Material bark, Material foliage)
    {
        CreatePrimitive(root, PrimitiveType.Cylinder, "Trunk", bark,
            new Vector3(0f, 2.2f, 0f), new Vector3(0.32f, 2.2f, 0.32f));

        if (isFir)
        {
            CreatePrimitive(root, PrimitiveType.Sphere, "Lower foliage", foliage,
                new Vector3(0f, 3.2f, 0f), new Vector3(2.3f, 2.0f, 2.3f));
            CreatePrimitive(root, PrimitiveType.Sphere, "Middle foliage", foliage,
                new Vector3(0f, 4.6f, 0f), new Vector3(1.65f, 1.8f, 1.65f));
            CreatePrimitive(root, PrimitiveType.Sphere, "Upper foliage", foliage,
                new Vector3(0f, 5.8f, 0f), new Vector3(1.0f, 1.5f, 1.0f));
        }
        else
        {
            CreatePrimitive(root, PrimitiveType.Sphere, "Canopy", foliage,
                new Vector3(0f, 5.0f, 0f), new Vector3(2.5f, 3.0f, 2.5f));
        }

        var collider = root.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0f, 2.2f, 0f);
        collider.radius = 0.35f;
        collider.height = 4.6f;
    }

    private static void CreateSpotlight(GameObject root, Material housing)
    {
        CreatePrimitive(root, PrimitiveType.Cylinder, "Housing", housing,
            Vector3.zero, new Vector3(0.24f, 0.38f, 0.24f));

        var lamp = new GameObject("Replacement light");
        lamp.transform.SetParent(root.transform, false);
        lamp.transform.localPosition = new Vector3(0f, -0.35f, 0f);
        var light = lamp.AddComponent<Light>();
        light.type = LightType.Spot;
        light.range = 18f;
        light.spotAngle = 58f;
        light.intensity = 2f;
        light.shadows = LightShadows.None;
    }

    private static void CreateRelic(
        GameObject root,
        RelicId relicId,
        Material accent,
        Material fabric,
        Material bookCover,
        Material bark)
    {
        var pickup = root.AddComponent<RelicPickup>();
        pickup.relicId = relicId;

        var trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1f;

        switch (relicId)
        {
            case RelicId.TeddyBear:
                CreatePrimitive(root, PrimitiveType.Capsule, "Generic bear-shaped relic", bark,
                    new Vector3(0f, 1f, 0f), new Vector3(1.2f, 1.3f, 1.0f));
                break;
            case RelicId.HKUWooden:
                CreatePrimitive(root, PrimitiveType.Cylinder, "Generic wooden relic", bark,
                    new Vector3(0f, 1f, 0f), new Vector3(1.0f, 1.2f, 1.0f));
                break;
            case RelicId.HKUCap:
                CreatePrimitive(root, PrimitiveType.Sphere, "Generic fabric relic", fabric,
                    new Vector3(0f, 0.8f, 0f), new Vector3(1.5f, 0.75f, 1.2f));
                break;
            case RelicId.HKUPillow:
                CreatePrimitive(root, PrimitiveType.Cube, "Generic soft relic", fabric,
                    new Vector3(0f, 0.7f, 0f), new Vector3(1.8f, 1.0f, 1.4f));
                break;
            case RelicId.HKUBook:
                CreatePrimitive(root, PrimitiveType.Cube, "Generic book-shaped relic", bookCover,
                    new Vector3(0f, 0.7f, 0f), new Vector3(1.2f, 1.7f, 0.45f));
                break;
            case RelicId.HKUHoodie:
                CreatePrimitive(root, PrimitiveType.Capsule, "Generic clothing-shaped relic", accent,
                    new Vector3(0f, 1f, 0f), new Vector3(1.0f, 1.6f, 0.8f));
                break;
        }
    }

    private static GameObject CreatePrimitive(
        GameObject parent,
        PrimitiveType type,
        string name,
        Material material,
        Vector3 localPosition,
        Vector3 localScale)
    {
        var child = GameObject.CreatePrimitive(type);
        child.name = name;
        child.transform.SetParent(parent.transform, false);
        child.transform.localPosition = localPosition;
        child.transform.localScale = localScale;

        var collider = child.GetComponent<Collider>();
        if (collider != null)
        {
            UnityEngine.Object.DestroyImmediate(collider);
        }

        var renderer = child.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        return child;
    }
}
