using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[InitializeOnLoad]
public static class SceneStatistics
{
    //Constructor
    static SceneStatistics()
    {
        Initialize();
    }

    public static bool ShowStatistics = false;
    private static int vertCount = 0;
    private static int triCount = 0;
    private static int matCount = 0;
    private static int meshCount = 0;
    private static int submeshCount = 0;

    private static bool validSelection = false;
    private static bool CheckValidSelection
    {
        get
        {
            return Selected != null && mFiltersInSelection.Count > 0;
        }
    }

    private static GameObject[] Selected { get { return Selection.gameObjects; } }
    private static List<MeshFilter> mFiltersInSelection = new List<MeshFilter>();
    private static List<MeshRenderer> mRenderersInSelection = new List<MeshRenderer>();


    private static GUIStyle _windowStyle = null;
    public static GUIStyle WindowStyle
    {
        get
        {
            if (_windowStyle == null)
                _windowStyle = new GUIStyle(GUI.skin.window)
                {
                    fontStyle = FontStyle.Bold
                };
            return _windowStyle;
        }
    }


    [MenuItem("Tools/Toggle Selection Statistics %7")]
    static void ToggleSceneStatistics()
    {
        ShowStatistics = !ShowStatistics;
        EditorPrefs.SetBool("SHOW_SCENE_STATS", ShowStatistics);
        if (ShowStatistics)
        {

        }
    }

    public static void Initialize()
    {
        Selection.selectionChanged -= OnSelectionChanged;
        Selection.selectionChanged += OnSelectionChanged;
        SceneView.onSceneGUIDelegate -= DrawGUI;
        SceneView.onSceneGUIDelegate += DrawGUI;
        if (EditorPrefs.HasKey("SHOW_SCENE_STATS"))
        {
            ShowStatistics = EditorPrefs.GetBool("SHOW_SCENE_STATS");
        }
        OnSelectionChanged();
    }

    public static void DrawGUI(SceneView view)
    {
        if (ShowStatistics)
        {
            if (!validSelection)
                return;
            Handles.BeginGUI();
            Rect rect = new Rect(20, 20, 180, 110);
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            GUILayout.BeginArea(rect, new GUIContent(string.Format("{0} {1} Selected", meshCount, meshCount == 1 ? "Mesh" : " Meshes")), WindowStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Vertices", BoldLabelLeftMiddle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(vertCount.ToString("N0"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Triangles", BoldLabelLeftMiddle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(triCount.ToString("N0"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Materials", BoldLabelLeftMiddle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(matCount.ToString("N0"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sub Meshes", BoldLabelLeftMiddle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(submeshCount.ToString("N0"));
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }


    private static void OnSelectionChanged()
    {
        if (ShowStatistics)
        {
            GetSelectionObjects();
            validSelection = CheckValidSelection;
            vertCount = GetSelectedVertCount();
            triCount = GetSelectedTriCount();
            matCount = GetSelectedMaterialCount();
            meshCount = GetSelectedMeshCount();
            submeshCount = GetSelectedSubMeshCount();
        }
    }

    private static void GetSelectionObjects()
    {
        mFiltersInSelection.Clear();
        mRenderersInSelection.Clear();
        foreach (var go in Selection.gameObjects)
        {
            if (go != null)
            {
                MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
                foreach (var mrenderer in renderers)
                {
                    mRenderersInSelection.Add(mrenderer);
                    MeshFilter mfilter = mrenderer.GetComponent<MeshFilter>();
                    if (mfilter != null)
                        mFiltersInSelection.Add(mfilter);
                }
            }
        }
    }


    /// <summary>
    /// Returns the total face count of all Mesh's currently selected
    /// </summary>
    /// <returns></returns>
    public static int GetSelectedVertCount()
    {
        int verts = 0;
        foreach (MeshFilter m in mFiltersInSelection)
        {
            verts += m.sharedMesh.vertexCount;
        }
        return verts;
    }

    public static int GetSelectedTriCount()
    {
        int tris = 0;
        foreach (MeshFilter m in mFiltersInSelection)
        {
            tris += m.sharedMesh.triangles.Length / 3;
        }
        return tris;
    }

    /// <summary>
    /// Returns the total number of unique materials in the selection
    /// </summary>
    /// <returns></returns>
    public static int GetSelectedMaterialCount()
    {
        List<Material> uniqueMats = new List<Material>();
        foreach (MeshRenderer m in mRenderersInSelection)
        {
            foreach (Material mat in m.sharedMaterials)
            {
                if (!uniqueMats.Contains(mat))
                    uniqueMats.Add(mat);
            }
        }
        return uniqueMats.Count;
    }

    /// <summary>
    /// Returns the total number of meshes selected
    /// </summary>
    /// <returns></returns>
    public static int GetSelectedMeshCount()
    {
        return mFiltersInSelection.Count;
    }

    /// <summary>
    /// Returns the total number of submeshes in the selection
    /// </summary>
    /// <returns></returns>
    public static int GetSelectedSubMeshCount()
    {
        int c = 0;
        foreach (MeshFilter m in mFiltersInSelection)
        {
            c += m.sharedMesh.subMeshCount;
        }
        return c;
    }

    /// <summary>
    /// Bold label left middle.
    /// </summary>
    private static GUIStyle boldLabelLeftMiddle;
    public static GUIStyle BoldLabelLeftMiddle
    {
        get
        {
            if (boldLabelLeftMiddle == null)
            {
                boldLabelLeftMiddle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    contentOffset = new Vector2(0, 0),
                    richText = true,
                    padding = new RectOffset(3, 3, 3, 3),
                    alignment = TextAnchor.MiddleLeft
                };
            }
            return boldLabelLeftMiddle;
        }
    }
}