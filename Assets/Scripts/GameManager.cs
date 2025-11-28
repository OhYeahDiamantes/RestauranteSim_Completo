using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias de Escena")]
    public CustomerTable[] customerTables;
    public LayerMask playerInteractionLayerMask;

    [Header("Platos Disponibles")]
    public DishData[] availableDishes;

    [Header("Configuración de Clientes (Spawn)")]
    public GameObject waitingCustomerPrefab;
    public Transform doorSpawnPoint;
    public float baseSpawnInterval = 5f;
    private float currentSpawnInterval;
    private float spawnTimer;

    public int baseMaxCustomers = 5;
    public int currentMaxCustomers;
    private int currentCustomers = 0;

    [Header("Configuración de Charcos")]
    public GameObject puddlePrefab;
    public float baseMinPuddleInterval = 15f;
    public float baseMaxPuddleInterval = 30f;
    private float currentMinPuddleInterval;
    private float currentMaxPuddleInterval;

    public float mapBoundaryX = 10f;
    public float mapBoundaryY = 5f;
    private float puddleSpawnTimer;

    [Header("Estadísticas del Juego")]
    public float gameDuration = 300f;
    private float gameTimer;
    public int totalMoney = 0;

    //Contadores
    public int dishesDeliveredCorrect = 0;
    public int dishesDeliveredIncorrect = 0;
    public int customersSatisfied = 0;
    public int customersAnnoyed = 0;
    public int puddlesCleaned = 0;
    private int puddlesIgnored = 0;

    [Header("UI del Marcador")]
    public TextMeshProUGUI scoreText;

    //Multiplicador de Paciencia
    public float globalPatienceModifier { get; private set; } = 1f;

    public bool isGameActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    void Start()
    {
        //Valores por defecto
        currentSpawnInterval = baseSpawnInterval;
        currentMaxCustomers = baseMaxCustomers;
        currentMinPuddleInterval = baseMinPuddleInterval;
        currentMaxPuddleInterval = baseMaxPuddleInterval;

        spawnTimer = currentSpawnInterval;
        puddleSpawnTimer = Random.Range(currentMinPuddleInterval, currentMaxPuddleInterval);

        playerInteractionLayerMask = LayerMask.GetMask("Interactable");
        gameTimer = gameDuration;

        UpdateScoreDisplay();

        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        isGameActive = false;
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (isGameActive && gameTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                TrySpawnCustomer();
                spawnTimer = currentSpawnInterval;
            }

            puddleSpawnTimer -= Time.deltaTime;
            if (puddleSpawnTimer <= 0)
            {
                TrySpawnPuddle();
                puddleSpawnTimer = Random.Range(currentMinPuddleInterval, currentMaxPuddleInterval);
            }

            gameTimer -= Time.deltaTime;
            if (gameTimer <= 0)
            {
                gameTimer = 0;
                EndGame();
            }
        }
    }

    //Sistema de dificultad

    public void SetDifficulty(int difficultyIndex)
    {
        //0: Fácil, 1: Normal, 2: Difícil

        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        KitchenPickup chef = Object.FindFirstObjectByType<KitchenPickup>();

        switch (difficultyIndex)
        {
            case 0: //Fácil
                Debug.Log("Dificultad: FÁCIL");
                globalPatienceModifier = 1.5f;
                currentSpawnInterval = baseSpawnInterval * 1.5f;
                currentMinPuddleInterval = baseMinPuddleInterval * 1.5f;
                currentMaxPuddleInterval = baseMaxPuddleInterval * 1.5f;
                currentMaxCustomers = baseMaxCustomers;
                if (player != null) player.moveSpeed = 5f;
                break;

            case 1: //Normal
                Debug.Log("Dificultad: NORMAL");
                globalPatienceModifier = 1.0f;
                currentSpawnInterval = baseSpawnInterval;
                currentMinPuddleInterval = baseMinPuddleInterval;
                currentMaxPuddleInterval = baseMaxPuddleInterval;
                currentMaxCustomers = baseMaxCustomers;
                if (player != null) player.moveSpeed = 5f;
                break;

            case 2: //Difícil
                Debug.Log("Dificultad: DIFÍCIL");
                globalPatienceModifier = 0.7f;
                currentSpawnInterval = baseSpawnInterval * 0.7f;
                currentMinPuddleInterval = baseMinPuddleInterval * 0.7f;
                currentMaxPuddleInterval = baseMaxPuddleInterval * 0.7f;
                currentMaxCustomers = baseMaxCustomers + 2;
                if (player != null) player.moveSpeed = 7f;
                if (chef != null) chef.baseCookingTime = 3f;
                break;
        }

        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
        }

        //Iniciar juego
        isGameActive = true;
        Time.timeScale = 1f;
    }

    void TrySpawnCustomer()
    {
        if (currentCustomers >= currentMaxCustomers) return;

        if (waitingCustomerPrefab == null || doorSpawnPoint == null) return;

        GameObject newCustomerObj = Instantiate(waitingCustomerPrefab, doorSpawnPoint.position, Quaternion.identity);
        WaitingCustomer customerScript = newCustomerObj.GetComponent<WaitingCustomer>();

        if (customerScript != null)
        {
            DishData chosenDish = availableDishes[Random.Range(0, availableDishes.Length)];
            customerScript.SetDishData(chosenDish);
            currentCustomers++;
        }
    }

    void TrySpawnPuddle()
    {
        if (puddlePrefab == null) return;
        float randomX = Random.Range(-mapBoundaryX, mapBoundaryX);
        float randomY = Random.Range(-mapBoundaryY, mapBoundaryY);
        Instantiate(puddlePrefab, new Vector3(randomX, randomY, 0f), Quaternion.identity);
    }

    public void RecordDishDelivery(bool isCorrect) { if (isCorrect) { dishesDeliveredCorrect++; customersSatisfied++; } else { dishesDeliveredIncorrect++; } }
    public void RecordCustomerAnnoyed() { customersAnnoyed++; }
    public void RecordPuddleCleaned() { puddlesCleaned++; }
    public void CustomerServed(CustomerTable table) { }
    public void TableFreed(CustomerTable table) { currentCustomers--; }
    public void CustomerLeftQueue() { currentCustomers--; RecordCustomerAnnoyed(); }

    public void AddMoney(int amount) { totalMoney += amount; UpdateScoreDisplay(); }
    private void UpdateScoreDisplay() { if (scoreText != null) scoreText.text = $"Score: {totalMoney}"; }

    void EndGame()
    {
        isGameActive = false;

        if (scoreText != null) scoreText.gameObject.SetActive(false);

        Puddle[] remainingPuddles = Object.FindObjectsByType<Puddle>(FindObjectsSortMode.None);
        puddlesIgnored = remainingPuddles.Length;
        float payScore = CalculateFinalPayScore();
        string outcome = DetermineOutcome(payScore);
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null) uiManager.ShowEndGameScreen(outcome, GetFinalStatistics());
        Time.timeScale = 0f;
    }

    float CalculateFinalPayScore()
    {
        float score = (float)totalMoney;
        score += customersSatisfied * 20; score += puddlesCleaned * 10;
        score -= customersAnnoyed * 30; score -= dishesDeliveredIncorrect * 15; score -= puddlesIgnored * 15;
        return score;
    }

    string DetermineOutcome(float score)
    {
        const float LOW_THRESHOLD = 50; const float MEDIUM_THRESHOLD = 150;
        if (score <= LOW_THRESHOLD) return "DERROTA_BAJA";
        else if (score < MEDIUM_THRESHOLD) return "VICTORIA_NORMAL";
        else return "VICTORIA_COMPLETA";
    }

    public Dictionary<string, int> GetFinalStatistics()
    {
        return new Dictionary<string, int> { { "Dinero Total", totalMoney }, { "Platos Correctos", dishesDeliveredCorrect }, { "Platos Incorrectos", dishesDeliveredIncorrect }, { "Clientes Satisfechos", customersSatisfied }, { "Clientes Molestos", customersAnnoyed }, { "Charcos Limpiados", puddlesCleaned }, { "Charcos Ignorados", puddlesIgnored } };
    }
}