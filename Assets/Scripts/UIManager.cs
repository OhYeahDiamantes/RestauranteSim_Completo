using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Paneles de Inicio")]
    public GameObject startScreenPanel;
    public GameObject controlsPanel;

    [Header("Selector de Dificultad")]
    public GameObject difficultyPanel;

    [Header("Menú de Pausa")]
    public GameObject pausePanel;
    public Button pauseButtonHUD;

    [Header("Pantalla Final")]
    public GameObject endGamePanel;
    public Image characterSpriteDisplay;
    public TextMeshProUGUI outcomeText;
    public TextMeshProUGUI statsTableText;
    public Button restartButton;

    [Header("Navegación Mando (Primeros Botones)")]
    public GameObject startFirstButton;
    public GameObject difficultyFirstButton;
    public GameObject pauseFirstButton;
    public GameObject controlsCloseButton;
    public GameObject endGameFirstButton;

    [Header("Sprites de Resultado")]
    public Sprite winSprite;
    public Sprite normalSprite;
    public Sprite loseSprite;

    [Header("Elementos de UI del Juego")]
    public Canvas mainGameCanvas;

    //Chef
    public TextMeshProUGUI chefStatusTextPrefab;
    private TextMeshProUGUI activeChefStatusText;
    private Vector3 chefWorldPosition;

    //Mesas y Clientes
    public TextMeshProUGUI customerPatienceTextPrefab;
    public Image customerOrderDialogPrefab;
    private List<CustomerUIElements> customerUIPool = new List<CustomerUIElements>();
    public int initialCustomerUIPoolSize = 5;
    private List<WaitingUIElements> waitingUIPool = new List<WaitingUIElements>();

    private bool isPaused = false;

    [System.Serializable]
    public class CustomerUIElements
    {
        public TextMeshProUGUI patienceText;
        public Image orderDialogImage;
        public CustomerTable assignedTable;
    }

    [System.Serializable]
    public class WaitingUIElements
    {
        public TextMeshProUGUI patienceText;
        public WaitingCustomer assignedCustomer;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (mainGameCanvas == null)
        {
            mainGameCanvas = Object.FindFirstObjectByType<Canvas>();
            if (mainGameCanvas == null) { enabled = false; return; }
        }
        mainGameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if (chefStatusTextPrefab != null)
        {
            activeChefStatusText = Instantiate(chefStatusTextPrefab, mainGameCanvas.transform);
            activeChefStatusText.gameObject.SetActive(false);
        }

        for (int i = 0; i < initialCustomerUIPoolSize; i++)
        {
            CustomerUIElements newUI = new CustomerUIElements();
            if (customerPatienceTextPrefab != null)
            {
                newUI.patienceText = Instantiate(customerPatienceTextPrefab, mainGameCanvas.transform);
                newUI.patienceText.gameObject.SetActive(false);
            }
            if (customerOrderDialogPrefab != null)
            {
                newUI.orderDialogImage = Instantiate(customerOrderDialogPrefab, mainGameCanvas.transform);
                newUI.orderDialogImage.gameObject.SetActive(false);
            }
            customerUIPool.Add(newUI);
        }
    }

    void Start()
    {
        ShowStartScreen();
        if (pausePanel != null) pausePanel.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (controlsPanel.activeSelf)
            {
                OnCloseControlsClicked();
            }
            else if (!startScreenPanel.activeSelf &&
                     !endGamePanel.activeSelf &&
                     (difficultyPanel == null || !difficultyPanel.activeSelf))
            {
                TogglePause();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!startScreenPanel.activeSelf && !endGamePanel.activeSelf && !controlsPanel.activeSelf && (difficultyPanel == null || !difficultyPanel.activeSelf))
            {
                TogglePause();
            }
        }

        if (activeChefStatusText != null && activeChefStatusText.gameObject.activeSelf)
        {
            PositionUIElement(activeChefStatusText.rectTransform, chefWorldPosition, new Vector2(0, 50));
        }
    }

    private void SelectButton(GameObject btn)
    {
        if (btn != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(btn);
        }
    }

    //Pausa y control del juego

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicPauseState(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                SelectButton(pauseFirstButton);
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnPauseHUDClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        TogglePause();
    }

    public void OnResumeClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        if (isPaused) TogglePause();
    }

    public void ShowStartScreen()
    {
        Time.timeScale = 0f;
        startScreenPanel.SetActive(true);
        endGamePanel.SetActive(false);
        controlsPanel.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (pauseButtonHUD != null) pauseButtonHUD.gameObject.SetActive(false);

        SelectButton(startFirstButton);
    }

    public void OnStartButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();

        startScreenPanel.SetActive(false);

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
            SelectButton(difficultyFirstButton);
        }
        else
        {
            SelectDifficulty(1);
        }
    }

    public void SelectDifficulty(int difficultyIndex)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();

        if (difficultyPanel != null) difficultyPanel.SetActive(false);
        if (pauseButtonHUD != null) pauseButtonHUD.gameObject.SetActive(true);
        GameManager.Instance.SetDifficulty(difficultyIndex);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnControlsButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        controlsPanel.SetActive(true);
        startScreenPanel.SetActive(false);
        SelectButton(controlsCloseButton);
    }

    public void OnCloseControlsClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        controlsPanel.SetActive(false);

        if (isPaused)
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                SelectButton(pauseFirstButton);
            }
        }
        else
        {
            if (Time.timeScale == 0f && !endGamePanel.activeSelf)
            {
                startScreenPanel.SetActive(true);
                SelectButton(startFirstButton);
            }
        }
    }

    public void OnExitButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnRestartButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowEndGameScreen(string outcome, Dictionary<string, int> stats)
    {
        HideAllFloatingUI();
        startScreenPanel.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);

        if (pauseButtonHUD != null) pauseButtonHUD.gameObject.SetActive(false);

        endGamePanel.SetActive(true);
        Time.timeScale = 0f;

        string message = "";
        switch (outcome) { case "DERROTA_BAJA": message = "GAME OVER. Paga Baja."; characterSpriteDisplay.sprite = loseSprite; break; case "VICTORIA_NORMAL": message = "VICTORIA. Paga Media."; characterSpriteDisplay.sprite = normalSprite; break; case "VICTORIA_COMPLETA": message = "¡VICTORIA COMPLETA!"; characterSpriteDisplay.sprite = winSprite; break; }
        outcomeText.text = message;
        string statsOutput = "-----------------\n"; foreach (var stat in stats) statsOutput += $"{stat.Key}: {stat.Value}\n"; statsTableText.text = statsOutput;

        SelectButton(endGameFirstButton);
    }

    public void ShowChefStatus(string status, Vector3 worldPosition)
    {
        if (activeChefStatusText != null)
        {
            activeChefStatusText.text = status; activeChefStatusText.gameObject.SetActive(true);
            chefWorldPosition = worldPosition;
            PositionUIElement(activeChefStatusText.rectTransform, worldPosition, new Vector2(0, 50));
        }
    }
    public void HideChefStatus() { if (activeChefStatusText != null) activeChefStatusText.gameObject.SetActive(false); }

    //Mesas y clientes
    public CustomerUIElements GetOrCreateCustomerUI(CustomerTable table)
    {
        if (endGamePanel != null && endGamePanel.activeSelf) return null;
        foreach (CustomerUIElements ui in customerUIPool)
        {
            if (ui.assignedTable == null || (!ui.patienceText.gameObject.activeSelf && !ui.orderDialogImage.gameObject.activeSelf))
            {
                ui.assignedTable = table;
                if (ui.patienceText != null) ui.patienceText.gameObject.SetActive(true);
                return ui;
            }
        }
        CustomerUIElements newUI = new CustomerUIElements();
        if (customerPatienceTextPrefab != null) { newUI.patienceText = Instantiate(customerPatienceTextPrefab, mainGameCanvas.transform); newUI.patienceText.gameObject.SetActive(true); }
        if (customerOrderDialogPrefab != null) { newUI.orderDialogImage = Instantiate(customerOrderDialogPrefab, mainGameCanvas.transform); newUI.orderDialogImage.gameObject.SetActive(false); }
        newUI.assignedTable = table; customerUIPool.Add(newUI); return newUI;
    }

    public void UpdateCustomerPatience(CustomerUIElements ui, float currentPatience, float maxPatience, Vector3 worldPosition)
    {
        if (endGamePanel != null && endGamePanel.activeSelf) return;
        if (ui != null)
        {
            if (ui.patienceText != null)
            {
                if (!ui.patienceText.gameObject.activeSelf) ui.patienceText.gameObject.SetActive(true);
                int percentage = Mathf.RoundToInt((currentPatience / maxPatience) * 100f);
                ui.patienceText.text = $"Paciencia: {percentage}%";
                if (percentage > 75) ui.patienceText.color = Color.green; else if (percentage > 40) ui.patienceText.color = Color.yellow; else if (percentage > 15) ui.patienceText.color = new Color(1f, 0.6f, 0f); else ui.patienceText.color = Color.red;
                PositionUIElement(ui.patienceText.rectTransform, worldPosition, new Vector2(0, 5));
            }
            if (ui.orderDialogImage != null && ui.orderDialogImage.gameObject.activeSelf) PositionUIElement(ui.orderDialogImage.rectTransform, worldPosition, new Vector2(0, 150));
        }
    }

    public void UpdateCustomerEating(CustomerUIElements ui, Vector3 worldPosition)
    {
        if (endGamePanel != null && endGamePanel.activeSelf) return;
        if (ui != null && ui.patienceText != null)
        {
            if (!ui.patienceText.gameObject.activeSelf) ui.patienceText.gameObject.SetActive(true);
            ui.patienceText.text = "Comiendo..."; ui.patienceText.color = Color.cyan;
            PositionUIElement(ui.patienceText.rectTransform, worldPosition, new Vector2(0, 40));
        }
    }

    public void SetCustomerOrderImage(CustomerUIElements ui, Sprite dishSprite, Vector3 worldPosition)
    {
        if (endGamePanel != null && endGamePanel.activeSelf) return;
        if (ui != null && ui.orderDialogImage != null)
        {
            ui.orderDialogImage.sprite = dishSprite; ui.orderDialogImage.preserveAspect = true; ui.orderDialogImage.rectTransform.sizeDelta = new Vector2(60f, 60f); ui.orderDialogImage.gameObject.SetActive(true);
            PositionUIElement(ui.orderDialogImage.rectTransform, worldPosition, new Vector2(0, 150));
        }
    }

    public void HideCustomerUI(CustomerUIElements ui)
    {
        if (ui != null) { if (ui.patienceText != null) ui.patienceText.gameObject.SetActive(false); if (ui.orderDialogImage != null) ui.orderDialogImage.gameObject.SetActive(false); ui.assignedTable = null; }
    }

    public WaitingUIElements GetOrCreateWaitingUI(WaitingCustomer customer)
    {
        if (endGamePanel != null && endGamePanel.activeSelf) return null;
        foreach (WaitingUIElements ui in waitingUIPool)
        {
            if (ui.assignedCustomer == null || !ui.patienceText.gameObject.activeSelf) { ui.assignedCustomer = customer; ui.patienceText.gameObject.SetActive(true); return ui; }
        }
        WaitingUIElements newUI = new WaitingUIElements();
        if (customerPatienceTextPrefab != null) { newUI.patienceText = Instantiate(customerPatienceTextPrefab, mainGameCanvas.transform); newUI.patienceText.gameObject.SetActive(true); }
        newUI.assignedCustomer = customer; waitingUIPool.Add(newUI); return newUI;
    }

    public void UpdateWaitingPatience(WaitingUIElements ui, float current, float max, Vector3 worldPos)
    {
        if (endGamePanel != null && endGamePanel.activeSelf) return;
        if (ui != null && ui.patienceText != null)
        {
            int percentage = Mathf.RoundToInt((current / max) * 100f); ui.patienceText.text = $"Paciencia: {percentage}%";
            if (percentage > 75) ui.patienceText.color = Color.green; else if (percentage > 40) ui.patienceText.color = Color.yellow; else ui.patienceText.color = Color.red;
            PositionUIElement(ui.patienceText.rectTransform, worldPos, new Vector2(0, 15));
        }
    }

    public void HideWaitingUI(WaitingUIElements ui) { if (ui != null) { if (ui.patienceText != null) ui.patienceText.gameObject.SetActive(false); ui.assignedCustomer = null; } }

    private void PositionUIElement(RectTransform uiRectTransform, Vector3 worldPosition, Vector2 offset)
    {
        if (Camera.main == null) return;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
        if (screenPoint.z < 0) return;
        screenPoint.x += offset.x;
        screenPoint.y += offset.y;
        screenPoint.z = 0;
        uiRectTransform.position = screenPoint;
    }

    private void HideAllFloatingUI()
    {
        foreach (CustomerUIElements ui in customerUIPool) HideCustomerUI(ui);
        foreach (WaitingUIElements ui in waitingUIPool) HideWaitingUI(ui);
        HideChefStatus();
    }
}