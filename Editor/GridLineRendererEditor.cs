using UnityEngine;
using UnityEditor;

namespace GridLineRenderer.Editor   
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GridLineRenderer))]
    public class GridLineRendererEditor : UnityEditor.Editor
    {
        GridLineRenderer gridLineRenderer;
        SerializedProperty gridSize;
        SerializedProperty gridSpacing;
        SerializedProperty gridPivot;
        SerializedProperty lineMaterial;
        SerializedProperty lineWidth;
        SerializedProperty disableXParallelLines;
        SerializedProperty disableYParallelLines;
        SerializedProperty disableZParallelLines;
        SerializedProperty autoUpdate;

        void OnEnable()
        {
            gridLineRenderer = (GridLineRenderer)target;
            gridSize = serializedObject.FindProperty("gridSize");
            gridSpacing = serializedObject.FindProperty("gridSpacing");
            gridPivot = serializedObject.FindProperty("gridPivot");
            lineMaterial = serializedObject.FindProperty("lineMaterial");
            lineWidth = serializedObject.FindProperty("lineWidth");
            disableXParallelLines = serializedObject.FindProperty("disableXParallelLines");
            disableYParallelLines = serializedObject.FindProperty("disableYParallelLines");
            disableZParallelLines = serializedObject.FindProperty("disableZParallelLines");
            autoUpdate = serializedObject.FindProperty("autoUpdate");
        }

        public override void OnInspectorGUI()
        {
            var prevGridSize = gridSize.vector3IntValue;
            var prevGridSpacing = gridSpacing.vector3Value;
            var prevGridPivot = gridPivot.vector3Value;
            var prevLineMaterial = lineMaterial.objectReferenceValue;
            var prevLineWidth = lineWidth.floatValue;
            var prevDisableXParallelLines = disableXParallelLines.boolValue;
            var prevDisableYParallelLines = disableYParallelLines.boolValue;
            var prevDisableZParallelLines = disableZParallelLines.boolValue;
            var valid = true;
            var manualUpdate = false;
            serializedObject.Update();
            gridSize.vector3IntValue = EditorGUILayout.Vector3IntField("Grid Size", gridSize.vector3IntValue);
            if (gridSize.vector3IntValue.x < 0 || gridSize.vector3IntValue.y < 0 || gridSize.vector3IntValue.z < 0)
            {
                EditorGUILayout.HelpBox("Grid size must be bigger than or equal to 0.", MessageType.Warning);
                valid = false;
            }
            gridSpacing.vector3Value = EditorGUILayout.Vector3Field("Grid Spacing", gridSpacing.vector3Value);
            var pivotX = EditorGUILayout.Slider("Pivot X", gridPivot.vector3Value.x, 0.0f, 1.0f);
            var pivotY = EditorGUILayout.Slider("Pivot Y", gridPivot.vector3Value.y, 0.0f, 1.0f);
            var pivotZ = EditorGUILayout.Slider("Pivot Z", gridPivot.vector3Value.z, 0.0f, 1.0f);
            gridPivot.vector3Value = new Vector3(pivotX, pivotY, pivotZ);
            lineMaterial.objectReferenceValue = EditorGUILayout.ObjectField("Line Material", lineMaterial.objectReferenceValue, typeof(Material), false);
            lineWidth.floatValue = EditorGUILayout.FloatField("Line Width", lineWidth.floatValue);
            if (lineWidth.floatValue <= 0.0f)
            {
                EditorGUILayout.HelpBox("Line Width must be bigger than 0.", MessageType.Warning);
                valid = false;
            }
            disableXParallelLines.boolValue = EditorGUILayout.Toggle("Disable X Parallel Lines", disableXParallelLines.boolValue);
            disableYParallelLines.boolValue = EditorGUILayout.Toggle("Disable Y Parallel Lines", disableYParallelLines.boolValue);
            disableZParallelLines.boolValue = EditorGUILayout.Toggle("Disable Z Parallel Lines", disableZParallelLines.boolValue);
            autoUpdate.boolValue = EditorGUILayout.Toggle("Auto Update", autoUpdate.boolValue);
            using (new EditorGUI.DisabledScope(autoUpdate.boolValue))
            {
                manualUpdate = GUILayout.Button("Update");
            }
            serializedObject.ApplyModifiedProperties();

            if (!valid) return;

            var valueChanged = 
                prevGridSize != gridSize.vector3IntValue ||
                prevGridSpacing != gridSpacing.vector3Value ||
                prevLineMaterial != lineMaterial.objectReferenceValue ||
                prevLineWidth != lineWidth.floatValue ||
                prevDisableXParallelLines != disableXParallelLines.boolValue ||
                prevDisableYParallelLines != disableYParallelLines.boolValue ||
                prevDisableZParallelLines != disableZParallelLines.boolValue ||
                prevGridPivot != gridPivot.vector3Value;

            if ((autoUpdate.boolValue && valueChanged) || manualUpdate)
            {
                gridLineRenderer.CreateGrid();
            }

        }
    }
}