using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LevelingUI : MonoBehaviour
{
    [Header("Core UI Elements")]
    [SerializeField] private GameObject levelingPanel;
    [SerializeField] private Button closeButton;
    
    [Header("Player Info")]
    [SerializeField] private Image playerAvatar;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image xpProgressBar;
    [SerializeField] private TMP_Text xpText;
    
    [Header("Level Progression")]
    [SerializeField] private Transform levelNodesParent;
    [SerializeField] private GameObject levelNodePrefab;
    [SerializeField] private Sprite lockedLevelSprite;
    [SerializeField] private Sprite unlockedLevelSprite;
    [SerializeField] private Sprite completedLevelSprite;
    
    [Header("Stats Display")]
    [SerializeField] private TMP_Text totalJumpsText;
    [SerializeField] private TMP_Text enemiesDefeatedText;
    [SerializeField] private TMP_Text coinsCollectedText;
    [SerializeField] private TMP_Text timePlayedText;
    
    [Header("Abilities Unlocked")]
    [SerializeField] private Transform abilitiesParent;
    [SerializeField] private GameObject abilityIconPrefab;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem levelUpParticles;
    [SerializeField] private AudioClip levelUpSound;
    
    private List<LevelNode> levelNodes = new List<LevelNode>();
    private PlayerProgress playerProgress;

    private void Awake()
    {
        closeButton.onClick.AddListener(ClosePanel);
        levelingPanel.SetActive(false);
    }

    public void Initialize(PlayerProgress progress)
    {
        playerProgress = progress;
        CreateLevelNodes();
        UpdateUI();
    }

    public void ShowLevelingUI()
    {
        levelingPanel.SetActive(true);
        UpdateUI();
    }

    private void ClosePanel()
    {
        levelingPanel.SetActive(false);
    }

    private void CreateLevelNodes()
    {
        // Clear existing nodes
        foreach (Transform child in levelNodesParent)
        {
            Destroy(child.gameObject);
        }
        levelNodes.Clear();

        // Create new nodes based on game levels
        for (int i = 0; i < playerProgress.totalLevels; i++)
        {
            GameObject nodeObj = Instantiate(levelNodePrefab, levelNodesParent);
            LevelNode node = nodeObj.GetComponent<LevelNode>();
            
            int levelIndex = i + 1;
            bool isUnlocked = playerProgress.IsLevelUnlocked(levelIndex);
            bool isCompleted = playerProgress.IsLevelCompleted(levelIndex);
            
            node.Initialize(
                levelIndex,
                isUnlocked ? (isCompleted ? completedLevelSprite : unlockedLevelSprite) : lockedLevelSprite,
                isUnlocked,
                playerProgress.GetLevelScore(levelIndex)
            );
            
            levelNodes.Add(node);
        }
    }

    private void UpdateUI()
    {
        // Player info
        playerNameText.text = playerProgress.playerName;
        levelText.text = $"Level {playerProgress.currentLevel}";
        xpProgressBar.fillAmount = (float)playerProgress.currentXP / playerProgress.xpToNextLevel;
        xpText.text = $"{playerProgress.currentXP}/{playerProgress.xpToNextLevel} XP";

        // Stats
        totalJumpsText.text = playerProgress.totalJumps.ToString();
        enemiesDefeatedText.text = playerProgress.enemiesDefeated.ToString();
        coinsCollectedText.text = playerProgress.coinsCollected.ToString();
        timePlayedText.text = FormatTime(playerProgress.timePlayed);

        // Update level nodes
        foreach (LevelNode node in levelNodes)
        {
            int levelIndex = node.LevelIndex;
            bool isUnlocked = playerProgress.IsLevelUnlocked(levelIndex);
            bool isCompleted = playerProgress.IsLevelCompleted(levelIndex);
            
            node.UpdateNode(
                isUnlocked ? (isCompleted ? completedLevelSprite : unlockedLevelSprite) : lockedLevelSprite,
                isUnlocked,
                playerProgress.GetLevelScore(levelIndex)
            );
        }

        // Update abilities
        UpdateAbilitiesDisplay();
    }

    private void UpdateAbilitiesDisplay()
    {
        // Clear existing ability icons
        foreach (Transform child in abilitiesParent)
        {
            Destroy(child.gameObject);
        }

        // Create icons for unlocked abilities
        foreach (var ability in playerProgress.unlockedAbilities)
        {
            GameObject abilityIcon = Instantiate(abilityIconPrefab, abilitiesParent);
            abilityIcon.GetComponent<AbilityIcon>().Initialize(ability.icon, ability.name);
        }
    }

    private string FormatTime(float seconds)
    {
        System.TimeSpan time = System.TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}h {1:D2}m {2:D2}s", 
            time.Hours, 
            time.Minutes, 
            time.Seconds);
    }

    public void PlayLevelUpEffects()
    {
        if (levelUpParticles != null) levelUpParticles.Play();
        AudioSource.PlayClipAtPoint(levelUpSound, Camera.main.transform.position);
    }
}

// Level Node UI Component
public class LevelNode : MonoBehaviour
{
    [SerializeField] private Image levelIcon;
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject scoreContainer;
    
    public int LevelIndex { get; private set; }

    public void Initialize(int levelIndex, Sprite icon, bool isUnlocked, int score)
    {
        LevelIndex = levelIndex;
        levelNumberText.text = levelIndex.ToString();
        levelIcon.sprite = icon;
        lockedOverlay.SetActive(!isUnlocked);
        scoreContainer.SetActive(isUnlocked);
        scoreText.text = score.ToString();
    }

    public void UpdateNode(Sprite icon, bool isUnlocked, int score)
    {
        levelIcon.sprite = icon;
        lockedOverlay.SetActive(!isUnlocked);
        scoreContainer.SetActive(isUnlocked);
        scoreText.text = score.ToString();
    }
}

// Ability Icon UI Component
public class AbilityIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text abilityNameText;

    public void Initialize(Sprite icon, string abilityName)
    {
        iconImage.sprite = icon;
        abilityNameText.text = abilityName;
    }
}

// Player Progress Data Structure
[System.Serializable]
public class PlayerProgress
{
    public string playerName;
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int totalLevels = 10;
    public int totalJumps;
    public int enemiesDefeated;
    public int coinsCollected;
    public float timePlayed;
    
    public List<UnlockedAbility> unlockedAbilities = new List<UnlockedAbility>();
    public Dictionary<int, bool> levelUnlockStatus = new Dictionary<int, bool>();
    public Dictionary<int, bool> levelCompletionStatus = new Dictionary<int, bool>();
    public Dictionary<int, int> levelScores = new Dictionary<int, int>();

    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelUnlockStatus.ContainsKey(levelIndex) && levelUnlockStatus[levelIndex];
    }

    public bool IsLevelCompleted(int levelIndex)
    {
        return levelCompletionStatus.ContainsKey(levelIndex) && levelCompletionStatus[levelIndex];
    }

    public int GetLevelScore(int levelIndex)
    {
        return levelScores.ContainsKey(levelIndex) ? levelScores[levelIndex] : 0;
    }
}

[System.Serializable]
public class UnlockedAbility
{
    public string name;
    public Sprite icon;
    public string description;
}

