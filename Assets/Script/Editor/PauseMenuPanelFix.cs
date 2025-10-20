using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace CityDriver.Editor
{
    public class PauseMenuPanelFix : EditorWindow
    {
        [MenuItem("CityDriver/Fix Pause Menu Panel")]
        public static void ShowWindow()
        {
            GetWindow<PauseMenuPanelFix>("Pause Menu Panel Fix");
        }

        private void OnGUI()
        {
            GUILayout.Label("Pause Menu Panel Fix Tool", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Check Current Scene for PauseMenuPanel", GUILayout.Height(40)))
            {
                CheckForPauseMenuPanel();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Create Missing PauseMenuPanel", GUILayout.Height(40)))
            {
                CreatePauseMenuPanel();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("List All UI Objects", GUILayout.Height(30)))
            {
                ListAllUIObjects();
            }
        }

        private void CheckForPauseMenuPanel()
        {
            Debug.Log("=== Checking for PauseMenuPanel ===");
            
            // Method 1: Direct search
            GameObject directFind = GameObject.Find("PauseMenuPanel");
            if (directFind != null)
            {
                Debug.Log($"✅ Found PauseMenuPanel by direct search: {directFind.name} (Active: {directFind.activeInHierarchy})");
                return;
            }

            // Method 2: Search all objects
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
            Debug.Log($"Searching through {allObjects.Length} GameObjects...");

            foreach (GameObject obj in allObjects)
            {
                string objName = obj.name.ToLower();
                if (objName == "pausemenupanel" || objName == "pause menu panel")
                {
                    Debug.Log($"✅ Found PauseMenuPanel: {obj.name} (Active: {obj.activeInHierarchy})");
                    return;
                }
            }

            Debug.LogWarning("❌ PauseMenuPanel not found in current scene!");
            
            // List potential matches
            Debug.Log("Potential pause-related objects:");
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.ToLower().Contains("pause"))
                {
                    Debug.Log($"  - {obj.name} (Active: {obj.activeInHierarchy})");
                }
            }
        }

        private void CreatePauseMenuPanel()
        {
            Debug.Log("=== Creating PauseMenuPanel ===");

            // Find Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found! Please create a Canvas first.");
                return;
            }

            // Check if already exists
            GameObject existing = GameObject.Find("PauseMenuPanel");
            if (existing != null)
            {
                Debug.Log($"PauseMenuPanel already exists: {existing.name}");
                return;
            }

            // Create main panel
            GameObject pausePanel = new GameObject("PauseMenuPanel");
            pausePanel.transform.SetParent(canvas.transform, false);
            
            // Add Image component for background
            Image panelImage = pausePanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black

            // Set RectTransform to fill parent
            RectTransform panelRect = pausePanel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;

            // Create button container
            GameObject buttonContainer = new GameObject("ButtonContainer");
            buttonContainer.transform.SetParent(pausePanel.transform, false);
            
            // Add VerticalLayoutGroup to container
            VerticalLayoutGroup layoutGroup = buttonContainer.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 20;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;

            // Set container RectTransform
            RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(200, 300);
            containerRect.anchoredPosition = Vector2.zero;

            // Create buttons
            CreatePauseMenuButton(buttonContainer, "ResumeButton", "Resume");
            CreatePauseMenuButton(buttonContainer, "RestartButton", "Restart");
            CreatePauseMenuButton(buttonContainer, "MainMenuFromPauseButton", "Main Menu");

            // Initially deactivate the panel
            pausePanel.SetActive(false);

            Debug.Log($"✅ Created PauseMenuPanel with buttons in {canvas.name}");
            
            // Try to find UIManager and refresh references
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                Debug.Log("Found UIManager - you may need to refresh UI references");
            }

            Selection.activeGameObject = pausePanel;
        }

        private void CreatePauseMenuButton(GameObject parent, string buttonName, string buttonText)
        {
            GameObject buttonObj = new GameObject(buttonName);
            buttonObj.transform.SetParent(parent.transform, false);

            // Add Button component
            Button button = buttonObj.AddComponent<Button>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.3f, 0.8f, 0.8f); // Blue background

            // Set button size
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(180, 50);

            // Create text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            Text buttonTextComp = textObj.AddComponent<Text>();
            buttonTextComp.text = buttonText;
            buttonTextComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonTextComp.fontSize = 16;
            buttonTextComp.color = Color.white;
            buttonTextComp.alignment = TextAnchor.MiddleCenter;

            // Set text RectTransform to fill button
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            button.targetGraphic = buttonImage;

            Debug.Log($"Created button: {buttonName}");
        }

        private void ListAllUIObjects()
        {
            Debug.Log("=== All UI Objects in Scene ===");
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
            
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.ToLower().Contains("panel") || 
                    obj.name.ToLower().Contains("menu") || 
                    obj.name.ToLower().Contains("button") ||
                    obj.name.ToLower().Contains("hud"))
                {
                    string parent = obj.transform.parent ? obj.transform.parent.name : "None";
                    Debug.Log($"UI Object: {obj.name} | Parent: {parent} | Active: {obj.activeInHierarchy}");
                }
            }
        }
    }
}