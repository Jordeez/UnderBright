//took me 3hr to do (had to go to documentation for this shit)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject creditsPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;
    
    [Header("Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    
    [Header("Level Select")]
    [SerializeField] private Button[] levelButtons;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem menuParticles;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private TextMeshProUGUI versionText;
    
    [Header("Audio")]
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource buttonClickSound;
    
    private void Start()
    {
        // Initialize UI
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        creditsPanel.SetActive(false);
        
        // Set up button listeners
        playButton.onClick.AddListener(ShowLevelSelect);
        settingsButton.onClick.AddListener(ShowSettings);
        creditsButton.onClick.AddListener(ShowCredits);
        quitButton.onClick.AddListener(QuitGame);
        backButton.onClick.AddListener(ReturnToMainMenu);
        
        // Set up settings
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        
        // Initialize settings values
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        fullscreenToggle.isOn = Screen.fullScreen;
        
        // Set up level buttons
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // Assuming level 1 is at build index 1
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
            
            // Enable/disable based on player progress
            bool levelUnlocked = PlayerPrefs.GetInt("LevelUnlocked_" + levelIndex, 0) == 1 || levelIndex == 1;
            levelButtons[i].interactable = levelUnlocked;
        }
        
        // Visual effects
        if (menuParticles != null) menuParticles.Play();
        if (versionText != null) versionText.text = "v" + Application.version;
        
        // Play menu music
        if (menuMusic != null) menuMusic.Play();
    }
    
    private void ShowLevelSelect()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        if (cameraAnimator != null) cameraAnimator.Play("CameraZoomIn");
    }
    
    private void ShowSettings()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    
    private void ShowCredits()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
        if (cameraAnimator != null) cameraAnimator.Play("CameraPanRight");
    }
    
    private void ReturnToMainMenu()
    {
        PlayButtonSound();
        settingsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
        if (cameraAnimator != null) cameraAnimator.Play("CameraReset");
    }
    
    private void LoadLevel(int levelIndex)
    {
        PlayButtonSound();
        SceneManager.LoadScene(levelIndex);
    }
    
    private void QuitGame()
    {
        PlayButtonSound();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void SetMusicVolume(float volume)
    {
        if (menuMusic != null) menuMusic.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    private void SetSFXVolume(float volume)
    {
        buttonClickSound.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    
    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    private void PlayButtonSound()
    {
        if (buttonClickSound != null) buttonClickSound.Play();
    }
}
//vague kasi sinabi ni sam that na this shit is by level so may levels akong ginawa
