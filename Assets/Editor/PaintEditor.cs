using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainPainter))]
public class PaintEditor : Editor
{
    void OnSceneGUI()
    {
        TerrainPainter Painter = (TerrainPainter)target;
        if (Event.current.type == EventType.MouseDown)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var SpawnPosition = new Vector2(Mathf.Round(worldRay.origin.x), Mathf.Round(worldRay.origin.y));
            var Instance = GameObject.Instantiate(Painter.Prefab, SpawnPosition, Quaternion.identity, Painter.transform);
            Instance.name = Painter.name + $" ({SpawnPosition.x}, {SpawnPosition.y})";
            Event.current.Use();
        }
        Selection.activeGameObject = Painter.gameObject;
    }
}
