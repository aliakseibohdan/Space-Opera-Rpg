using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyValidator
{
    private const int MaxAllowedDepth = 3;

    private static readonly string[] RequiredRoots = {
        "_Environment_Static",
        "_Environment_Dynamic",
        "_Gameplay_AI",
        "_Gameplay_Player",
        "_UI",
        "_Lighting",
        "_Audio",
        "_Volumes",
        "_SceneManagement",
        "_Debug"
    };

    static HierarchyValidator()
    {
        EditorApplication.hierarchyChanged += Validate;
    }

    private static void Validate()
    {
        foreach (var root in RequiredRoots)
        {
            if (GameObject.Find(root) == null)
                Debug.LogError($"[HierarchyValidator] Missing root: {root}");
        }

        var activeScene = EditorSceneManager.GetActiveScene();
        var roots = activeScene.GetRootGameObjects();

        foreach (var root in roots)
        {
            Traverse(root.transform, 0);
        }
    }

    private static void Traverse(Transform transform, int depth)
    {
        if (depth > MaxAllowedDepth)
        {
            Debug.LogWarning(
                $"[HierarchyValidator] {GetFullPath(transform)} is nested too deep (Depth = {depth}). Consider flattening.");
        }

        foreach (Transform child in transform)
        {
            Traverse(child, depth + 1);
        }
    }

    private static string GetFullPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            path = t.parent.name + "/" + path;
            t = t.parent;
        }
        return path;
    }
}