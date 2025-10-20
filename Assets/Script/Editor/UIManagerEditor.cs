using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Editor để giúp setup UIManager cross-scene functionality dễ dàng
/// </summary>
[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : Editor
{
    private UIManager uiManager;
    
    private void OnEnable()
    {
        uiManager = (UIManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Cross-Scene UI Management", EditorStyles.boldLabel);
        
        // Refresh UI References button
        if (GUILayout.Button("Refresh UI References", GUILayout.Height(30)))
        {
            RefreshUIReferences();
        }
        
        EditorGUILayout.Space();
        
        // Check for missing references
        CheckMissingReferences();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add UIElementNamer to Selected"))
        {
            AddUIElementNamerToSelected();
        }
        if (GUILayout.Button("Rename All UI Elements"))
        {
            RenameAllUIElements();
        }
        EditorGUILayout.EndHorizontal();
        
        // Debug information
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Information", EditorStyles.boldLabel);
        
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField($"Is Singleton Instance: {UIManager.Instance == uiManager}");
            EditorGUILayout.LabelField($"Game is Paused: {Time.timeScale == 0}");
        }
        else
        {
            EditorGUILayout.LabelField("Enter Play Mode to see runtime information");
        }
    }
    
    private void RefreshUIReferences()
    {
        if (Application.isPlaying)
        {
            // Call the refresh method if playing
            if (UIManager.Instance != null)
            {
                System.Reflection.MethodInfo refreshMethod = typeof(UIManager).GetMethod("RefreshUIReferences", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (refreshMethod != null)
                {
                    refreshMethod.Invoke(UIManager.Instance, null);
                    Debug.Log("UIManager: UI References refreshed via Editor");
                }
            }
        }
        else
        {
            Debug.LogWarning("UIManager: Cannot refresh references in Edit Mode. Enter Play Mode first.");
        }
    }
    
    private void CheckMissingReferences()
    {
        int missingCount = 0;
        
        // Check serialized fields using reflection
        System.Reflection.FieldInfo[] fields = typeof(UIManager).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(GameObject) || 
                field.FieldType == typeof(UnityEngine.UI.Button) || 
                field.FieldType == typeof(UnityEngine.UI.Text) ||
                field.FieldType == typeof(UnityEngine.UI.Image) ||
                field.FieldType == typeof(UnityEngine.UI.Slider))
            {
                var value = field.GetValue(uiManager);
                if (value == null || value.Equals(null))
                {
                    missingCount++;
                }
            }
        }
        
        if (missingCount > 0)
        {
            EditorGUILayout.HelpBox($"Missing {missingCount} UI references. Click 'Refresh UI References' in Play Mode to auto-detect them.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox("All UI references are assigned!", MessageType.Info);
        }
    }
    
    private void AddUIElementNamerToSelected()
    {
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("Please select GameObjects to add UIElementNamer component");
            return;
        }
        
        int addedCount = 0;
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.GetComponent<UIElementNamer>() == null)
            {
                obj.AddComponent<UIElementNamer>();
                addedCount++;
            }
        }
        
        Debug.Log($"Added UIElementNamer to {addedCount} GameObjects");
    }
    
    private void RenameAllUIElements()
    {
        UIElementNamer[] allNamers = FindObjectsOfType<UIElementNamer>();
        if (allNamers.Length == 0)
        {
            Debug.LogWarning("No UIElementNamer components found in scene");
            return;
        }
        
        int renamedCount = 0;
        foreach (UIElementNamer namer in allNamers)
        {
            string oldName = namer.gameObject.name;
            System.Reflection.MethodInfo renameMethod = typeof(UIElementNamer).GetMethod("RenameElement");
            if (renameMethod != null)
            {
                renameMethod.Invoke(namer, null);
                if (namer.gameObject.name != oldName)
                {
                    renamedCount++;
                }
            }
        }
        
        Debug.Log($"Renamed {renamedCount} UI elements in scene");
    }
}