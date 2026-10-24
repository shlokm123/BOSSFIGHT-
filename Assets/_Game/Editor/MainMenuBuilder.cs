using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using GlitchVerse.UI;

namespace GlitchVerse.Editor
{
    /// <summary>
    /// Autonomous Builder that constructs the cinematic MainMenu scene and registers all scenes in EditorBuildSettings.
    /// </summary>
    public static class MainMenuBuilder
    {
        private const string ArtPath = "Assets/_Game/Art";
        private const string ScenesPath = "Assets/_Game/Scenes";

        [MenuItem("GlitchVerse/Construct Main Menu Scene")]
        public static void ConstructMainMenuScene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // 1. Camera
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.04f, 0.03f, 0.05f, 1f); // Dark industrial charcoal
            cam.transform.position = new Vector3(0f, 0f, -10f);
            camObj.AddComponent<UniversalAdditionalCameraData>();

            // 2. Atmospheric 2D Lighting
            GameObject globalLightObj = new GameObject("GlobalLight_2D");
            Light2D globalLight = globalLightObj.AddComponent<Light2D>();
            globalLight.lightType = Light2D.LightType.Global;
            globalLight.color = new Color(0.12f, 0.08f, 0.15f, 1f);
            globalLight.intensity = 0.85f;

            GameObject redLampObj = new GameObject("Menu_EmergencyRedLight");
            redLampObj.transform.position = new Vector3(-6f, 3f, 0f);
            Light2D redLight = redLampObj.AddComponent<Light2D>();
            redLight.lightType = Light2D.LightType.Point;
            redLight.pointLightOuterRadius = 7.5f;
            redLight.color = new Color(1.0f, 0.12f, 0.22f, 1f);
            redLight.intensity = 0.85f;

            GameObject cyanLampObj = new GameObject("Menu_TerminalCyanLight");
            cyanLampObj.transform.position = new Vector3(6f, -3f, 0f);
            Light2D cyanLight = cyanLampObj.AddComponent<Light2D>();
            cyanLight.lightType = Light2D.LightType.Point;
            cyanLight.pointLightOuterRadius = 7.5f;
            cyanLight.color = new Color(0f, 0.90f, 1.0f, 1f);
            cyanLight.intensity = 0.65f;

            // 3. UI Canvas
            GameObject canvasObj = new GameObject("MainMenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 5f;
            canvas.sortingOrder = 50;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // Fullscreen subtle background image
            GameObject bgObj = new GameObject("Menu_BackgroundShade");
            bgObj.transform.SetParent(canvasObj.transform, false);
            RectTransform bgRt = bgObj.AddComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.02f, 0.02f, 0.03f, 0.96f);

            // 4. Panels
            // A. Main Menu Panel
            GameObject mainPanel = CreateUIPanel(canvasObj.transform, "MainMenuPanel", new Vector2(0f, 0f), new Vector2(1f, 1f));

            // Main Title
            CreateUIText(mainPanel.transform, "Title_Glitchverse",
                "GLITCHVERSE", defaultFont, 56, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.76f), new Vector2(0.5f, 0.76f), Vector2.zero, new Vector2(800f, 70f),
                new Color(0f, 0.94f, 1f, 1f));

            CreateUIText(mainPanel.transform, "Title_EchoProtocol",
                "ECHO PROTOCOL", defaultFont, 36, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.69f), new Vector2(0.5f, 0.69f), Vector2.zero, new Vector2(800f, 50f),
                Color.white);

            CreateUIText(mainPanel.transform, "Title_Secondary",
                "CONTAINMENT SYSTEM // ONLINE", defaultFont, 16, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.63f), new Vector2(0.5f, 0.63f), Vector2.zero, new Vector2(600f, 30f),
                new Color(1f, 0.15f, 0.25f, 1f));

            // Main Menu Buttons
            Button btnBegin = CreateUIButton(mainPanel.transform, "Btn_BeginExperiment",
                "BEGIN EXPERIMENT", defaultFont, 20,
                new Vector2(0.5f, 0.48f), new Vector2(360f, 52f),
                new Color(0f, 0.94f, 1f, 1f));

            Button btnControls = CreateUIButton(mainPanel.transform, "Btn_Controls",
                "CONTROLS", defaultFont, 18,
                new Vector2(0.5f, 0.38f), new Vector2(360f, 50f),
                new Color(0.85f, 0.88f, 0.92f, 1f));

            Button btnQuit = CreateUIButton(mainPanel.transform, "Btn_Quit",
                "QUIT", defaultFont, 18,
                new Vector2(0.5f, 0.28f), new Vector2(360f, 50f),
                new Color(0.7f, 0.7f, 0.75f, 1f));

            // B. Dossier Panel
            GameObject dossierPanel = CreateUIPanel(canvasObj.transform, "DossierPanel", new Vector2(0f, 0f), new Vector2(1f, 1f));

            CreateUIText(dossierPanel.transform, "Dossier_Header",
                "// CLASSIFIED FACILITY ARCHIVE // SUBJECT PROFILE //", defaultFont, 20, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.84f), new Vector2(0.5f, 0.84f), Vector2.zero, new Vector2(800f, 40f),
                new Color(1.0f, 0.7f, 0.2f, 1f));

            // Dossier Container Box
            GameObject dossierCard = CreateUIPanel(dossierPanel.transform, "DossierCard", new Vector2(0.18f, 0.32f), new Vector2(0.82f, 0.78f));
            Image cardBg = dossierCard.GetComponent<Image>();
            cardBg.color = new Color(0.05f, 0.06f, 0.08f, 0.95f);

            // Subject 075 Portrait Box
            GameObject portraitBox = new GameObject("SubjectPortraitBox");
            portraitBox.transform.SetParent(dossierCard.transform, false);
            RectTransform portRt = portraitBox.AddComponent<RectTransform>();
            portRt.anchorMin = new Vector2(0.06f, 0.25f);
            portRt.anchorMax = new Vector2(0.28f, 0.85f);
            portRt.offsetMin = Vector2.zero;
            portRt.offsetMax = Vector2.zero;
            Image portImg = portraitBox.AddComponent<Image>();
            Sprite playerSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/player_sprite.png");
            if (playerSp != null) portImg.sprite = playerSp;
            portImg.color = new Color(0.75f, 0.95f, 1.0f, 1f);

            // Dossier Metadata Text
            string dossierContent =
                "SUBJECT FILE: 075\n\n" +
                "IDENTITY: CLASSIFIED\n\n" +
                "STATUS: CONSCIOUS\n\n" +
                "MEMORY INTEGRITY: COMPROMISED\n\n" +
                "ECHO RECORDER: ONLINE\n\n" +
                "CONTAINMENT STATUS: ACTIVE";

            Text metaTxt = CreateUIText(dossierCard.transform, "DossierMetadata",
                dossierContent, defaultFont, 18, TextAnchor.MiddleLeft,
                new Vector2(0.34f, 0.1f), new Vector2(0.95f, 0.9f), Vector2.zero, Vector2.zero,
                new Color(0f, 0.92f, 1.0f, 1f));

            // Architect Transmission Dialogue Box
            GameObject diagBox = CreateUIPanel(dossierPanel.transform, "ArchitectTransmissionBox", new Vector2(0.18f, 0.14f), new Vector2(0.82f, 0.28f));
            Image diagBg = diagBox.GetComponent<Image>();
            diagBg.color = new Color(0.02f, 0.03f, 0.05f, 0.95f);

            Text speakerTxt = CreateUIText(diagBox.transform, "SpeakerName",
                "// THE ARCHITECT //", defaultFont, 14, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, -8f), new Vector2(300f, 20f),
                new Color(0f, 0.94f, 1f, 1f));

            Text diagTxt = CreateUIText(diagBox.transform, "DialogueText",
                "", defaultFont, 20, TextAnchor.MiddleLeft,
                new Vector2(0f, 0f), new Vector2(1f, 0.8f), new Vector2(20f, 0f), new Vector2(-40f, 0f),
                Color.white);

            Button btnEnter = CreateUIButton(dossierPanel.transform, "Btn_EnterContainment",
                "ENTER CONTAINMENT", defaultFont, 20,
                new Vector2(0.5f, 0.07f), new Vector2(380f, 50f),
                new Color(0f, 1.0f, 0.5f, 1f));
            btnEnter.gameObject.SetActive(false);

            // C. Controls Panel
            GameObject controlsPanel = CreateUIPanel(canvasObj.transform, "ControlsPanel", new Vector2(0f, 0f), new Vector2(1f, 1f));

            CreateUIText(controlsPanel.transform, "Controls_Header",
                "// SYSTEM CONTROL GUIDE // VERIFIED BINDINGS //", defaultFont, 22, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.84f), new Vector2(0.5f, 0.84f), Vector2.zero, new Vector2(800f, 40f),
                new Color(0f, 0.94f, 1f, 1f));

            GameObject ctrlCard = CreateUIPanel(controlsPanel.transform, "ControlsCard", new Vector2(0.22f, 0.20f), new Vector2(0.78f, 0.78f));
            Image ctrlBg = ctrlCard.GetComponent<Image>();
            ctrlBg.color = new Color(0.04f, 0.05f, 0.07f, 0.95f);

            string bindingsText =
                "<color=#00F0FF>A / D</color>  —  Move\n\n" +
                "<color=#00F0FF>Space</color>  —  Jump\n\n" +
                "<color=#00F0FF>G</color>  —  Gravity Inversion\n\n" +
                "<color=#00F0FF>F</color>  —  Time Freeze\n\n" +
                "<color=#00F0FF>Q</color>  —  Dimension Switching\n\n" +
                "<color=#00F0FF>H</color>  —  Echo Replay\n\n" +
                "<color=#00F0FF>C</color>  —  Clear Echo\n\n" +
                "<color=#FFAA00>R</color>  —  Restart Chapter\n\n" +
                "<color=#FF1E40>Y / N</color>  —  Chapter III Interrogation Responses";

            CreateUIText(ctrlCard.transform, "BindingsList",
                bindingsText, defaultFont, 18, TextAnchor.MiddleLeft,
                new Vector2(0.1f, 0.05f), new Vector2(0.9f, 0.95f), Vector2.zero, Vector2.zero,
                Color.white);

            Button btnBack = CreateUIButton(controlsPanel.transform, "Btn_ControlsBack",
                "BACK", defaultFont, 18,
                new Vector2(0.5f, 0.11f), new Vector2(260f, 46f),
                new Color(0.85f, 0.88f, 0.92f, 1f));

            dossierPanel.SetActive(false);
            controlsPanel.SetActive(false);
            mainPanel.SetActive(true);

            // 5. Attach and wire MainMenuController
            MainMenuController controller = canvasObj.AddComponent<MainMenuController>();
            SetPrivateField(controller, "mainMenuPanel", mainPanel);
            SetPrivateField(controller, "dossierPanel", dossierPanel);
            SetPrivateField(controller, "controlsPanel", controlsPanel);
            SetPrivateField(controller, "beginExperimentButton", btnBegin);
            SetPrivateField(controller, "controlsButton", btnControls);
            SetPrivateField(controller, "quitButton", btnQuit);
            SetPrivateField(controller, "controlsBackButton", btnBack);
            SetPrivateField(controller, "dossierMetadataText", metaTxt);
            SetPrivateField(controller, "subjectPortrait", portImg);
            SetPrivateField(controller, "architectDialogueText", diagTxt);
            SetPrivateField(controller, "speakerLabelText", speakerTxt);
            SetPrivateField(controller, "enterContainmentButton", btnEnter);

            // 6. EventSystem
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<InputSystemUIInputModule>();

            // Save MainMenu.unity
            string fullScenePath = Path.Combine(ScenesPath, "MainMenu.unity");
            EditorSceneManager.SaveScene(newScene, fullScenePath);
            Debug.Log($"[MainMenuBuilder] MainMenu.unity successfully saved to {fullScenePath}");

            // Register all scenes in Build Settings in required order
            RegisterAllScenesInBuildSettings();
        }

        public static void RegisterAllScenesInBuildSettings()
        {
            string[] scenePaths = new string[]
            {
                "Assets/_Game/Scenes/MainMenu.unity",
                "Assets/_Game/Scenes/Levels/Level01.unity",
                "Assets/_Game/Scenes/Levels/Level02.unity",
                "Assets/_Game/Scenes/Levels/Level03.unity",
                "Assets/_Game/Scenes/Levels/Level04.unity",
                "Assets/_Game/Scenes/Levels/Level05.unity",
                "Assets/_Game/Scenes/Levels/Level06.unity",
                "Assets/_Game/Scenes/Levels/Level07.unity"
            };

            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }

            EditorBuildSettings.scenes = buildScenes;
            AssetDatabase.SaveAssets();
            Debug.Log($"[MainMenuBuilder] Registered {buildScenes.Length} scenes in EditorBuildSettings (MainMenu at Index 0).");
        }

        private static GameObject CreateUIPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent, false);

            RectTransform rt = panelObj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = panelObj.AddComponent<Image>();
            img.color = new Color(0.03f, 0.03f, 0.04f, 0.94f);

            return panelObj;
        }

        private static Button CreateUIButton(Transform parent, string name, string label, Font font, int fontSize, Vector2 anchorCenter, Vector2 size, Color textColor)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            RectTransform rt = btnObj.AddComponent<RectTransform>();
            rt.anchorMin = anchorCenter;
            rt.anchorMax = anchorCenter;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = size;

            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.10f, 0.12f, 0.16f, 0.95f);

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = new Color(0.10f, 0.12f, 0.16f, 0.95f);
            cb.highlightedColor = new Color(0.18f, 0.22f, 0.30f, 1.0f);
            cb.pressedColor = new Color(0.06f, 0.08f, 0.10f, 1.0f);
            cb.selectedColor = cb.highlightedColor;
            btn.colors = cb;

            CreateUIText(btnObj.transform, "Label",
                label, font, fontSize, TextAnchor.MiddleCenter,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                textColor);

            return btn;
        }

        private static Text CreateUIText(Transform parent, string name, string content, Font font, int size,
            TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
        {
            GameObject txtObj = new GameObject(name);
            txtObj.transform.SetParent(parent, false);

            RectTransform rt = txtObj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Text text = txtObj.AddComponent<Text>();
            text.text = content;
            text.font = font;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.supportRichText = true;

            return text;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var type = target.GetType();
            System.Reflection.FieldInfo field = null;
            while (type != null && field == null)
            {
                field = type.GetField(fieldName,
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);
                type = type.BaseType;
            }

            if (field != null)
            {
                field.SetValue(target, value);
            }
        }
    }
}
