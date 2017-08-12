using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TileManager))]
public class TileManagerInspector : Editor
{
    private ReorderableList m_reorderableList;

    private TileManager m_tileManager;

    private void OnEnable()
    {
        if (Selection.activeGameObject != null)
        {
            m_tileManager = Selection.activeGameObject.GetComponent<TileManager>();
        }

        if (m_tileManager != null)
        {
            m_reorderableList = new ReorderableList(m_tileManager.m_levels, typeof(TileManager.Level), true, true, false, false);

            if (m_reorderableList != null)
            {
                // Add listeners to draw events
                m_reorderableList.drawHeaderCallback += DrawHeader;
                m_reorderableList.drawElementCallback += DrawElement;
            }
        }
    }

    private void OnDisable()
    {
        if (m_reorderableList != null)
        {
            // Make sure we don't get memory leaks etc.
            m_reorderableList.drawHeaderCallback -= DrawHeader;
            m_reorderableList.drawElementCallback -= DrawElement;
        }
    }

    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, "Levels");
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        TileManager.Level item = m_tileManager.m_levels[index];

        EditorGUI.BeginChangeCheck();
        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Level " + index.ToString());

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (m_tileManager == null)
        {
            m_tileManager = Selection.activeGameObject.GetComponent<TileManager>();
        }

        if (m_tileManager != null)
        {
            // Actually draw the list in the inspector
            m_reorderableList.DoLayoutList();
        }
    }
}