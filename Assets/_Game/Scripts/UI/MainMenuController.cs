using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Boss;
using GlitchVerse.Narrative;

namespace GlitchVerse.UI
{
    /// <summary>
    /// Master controller for the cinematic containment Start Page / Main Menu.
    /// Manages Main Menu navigation, Subject 075 dossier intro with sequential Architect dialogue,
    /// verified controls panel, and campaign start in Level01.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject dossierPanel;
        [SerializeField] private GameObject controlsPanel;

        [Header("Main Menu Controls")]
        [SerializeField] private Button beginExperimentButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button quitButton;

        [Header("Controls Panel")]
        [SerializeField] private Button controlsBackButton;

        [Header("Dossier & Dialogue Elements")]
        [SerializeField] private Text dossierMetadataText;
        [SerializeField] private Image subjectPortrait;
        [SerializeField] private Text architectDialogueText;
        [SerializeField] private Text speakerLabelText;
        [SerializeField] private Button enterContainmentButton;

        private bool isLoadingLevel = false;
        private Coroutine dialogueCoroutine;

        private readonly string[] openingDialogueLines = new string[]
        {
            "Good morning, Subject 075.",
            "You don't remember how you got here, do you?",
            "How unfortunate.",
            "Complete the tests. Find the exit.",
            "Try not to disappoint me."
        };

        private void Awake()
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Wire button listeners safely
            if (beginExperimentButton != null)
            {
                beginExperimentButton.onClick.RemoveAllListeners();
                beginExperimentButton.onClick.AddListener(OnBeginExperimentClicked);
            }

            if (controlsButton != null)
            {
                controlsButton.onClick.RemoveAllListeners();
                controlsButton.onClick.AddListener(OnControlsClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuitClicked);
            }

            if (controlsBackButton != null)
            {
                controlsBackButton.onClick.RemoveAllListeners();
                controlsBackButton.onClick.AddListener(OnControlsBackClicked);
            }

            if (enterContainmentButton != null)
            {
                enterContainmentButton.onClick.RemoveAllListeners();
                enterContainmentButton.onClick.AddListener(OnEnterContainmentClicked);
            }
        }

        private void Start()
        {
            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
                dialogueCoroutine = null;
            }

            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (dossierPanel != null) dossierPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);
        }

        public void OnBeginExperimentClicked()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);
            if (dossierPanel != null) dossierPanel.SetActive(true);

            if (enterContainmentButton != null) enterContainmentButton.gameObject.SetActive(false);

            if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = StartCoroutine(PlayArchitectOpeningSequence());
        }

        private IEnumerator PlayArchitectOpeningSequence()
        {
            if (speakerLabelText != null) speakerLabelText.text = "// THE ARCHITECT //";
            if (architectDialogueText != null) architectDialogueText.text = "";

            yield return new WaitForSecondsRealtime(0.6f);

            for (int i = 0; i < openingDialogueLines.Length; i++)
            {
                if (architectDialogueText != null)
                {
                    architectDialogueText.text = $"\"{openingDialogueLines[i]}\"";
                }

                // Deterministic pause per line
                yield return new WaitForSecondsRealtime(2.0f);
            }

            // Reveal ENTER CONTAINMENT button at end of dialogue sequence
            if (enterContainmentButton != null)
            {
                enterContainmentButton.gameObject.SetActive(true);
            }
        }

        public void OnEnterContainmentClicked()
        {
            if (isLoadingLevel) return;
            isLoadingLevel = true;

            Time.timeScale = 1.0f;

            // Reset all session and prank state for a clean new campaign
            BossDirector.ResetSessionCheckpoint();
            CollapsingFloor.ResetSessionFlag();
            ImpossibleQuestionPrank.ResetSessionFlag();
            ChapterIntroController.ResetSeenChapters();
            ParanoiaTrapPrank.ResetSessionFlag();

            if (GameSession.Instance != null)
            {
                GameSession.Instance.ResetSession();
            }

            Debug.Log("[MainMenuController] Entering Containment -> Loading Level01");
            SceneManager.LoadScene("Level01");
        }

        public void OnControlsClicked()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (dossierPanel != null) dossierPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(true);
        }

        public void OnControlsBackClicked()
        {
            ShowMainMenu();
        }

        public void OnQuitClicked()
        {
            Debug.Log("[MainMenuController] Quitting application.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
