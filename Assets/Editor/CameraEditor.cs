using System;
using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(CameraCapture))]
public class CameraSnapshotEditor: Editor
{
    private int m_captureWidth = 1920;
    private int m_captureHeight = 1080;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        m_captureWidth = EditorGUILayout.IntField("Capture Width", m_captureWidth);
        m_captureHeight = EditorGUILayout.IntField("Capture Height", m_captureHeight);

        if (GUILayout.Button("Save Capture To File"))
            Capture();
    }

    public void Capture()
    {
        var savePath = EditorUtility.SaveFilePanel("Save Capture...", Application.dataPath, "capture.png", "png");
        if (string.IsNullOrEmpty(savePath))
            return;

        var capture = (CameraCapture)target;
        var camera = capture.GetComponent<Camera>();
        var texture = new RenderTexture(m_captureWidth, m_captureHeight, 24);
        var oldTexture = camera.targetTexture;
        camera.targetTexture = texture;
        camera.Render();
        camera.targetTexture = oldTexture;

        var t2d = new Texture2D(m_captureWidth, m_captureHeight);
        var oldRT = RenderTexture.active;
        RenderTexture.active = texture;
        t2d.ReadPixels(new Rect(0, 0, m_captureWidth, m_captureHeight), 0, 0);
        t2d.Apply();
        RenderTexture.active = oldRT;

        var pngData = t2d.EncodeToPNG();
        DestroyImmediate(texture);
        DestroyImmediate(t2d);

        File.WriteAllBytes(savePath, pngData);
    }
}