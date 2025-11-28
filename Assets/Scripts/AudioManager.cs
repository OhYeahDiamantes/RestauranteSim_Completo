using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips de Audio")]
    public AudioClip backgroundMusic;
    public AudioClip moneyPickupSound;
    public AudioClip buttonClickSound;
    public AudioClip trashSound;
    public AudioClip puddleCleanSound;
    public AudioClip catSound;
    public AudioClip chefFinishSound;

    private bool isMusicMuted = false;
    private bool isSfxMuted = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void Update()
    {
        // INPUT MANAGER: Mutear música
        if (Input.GetButtonDown("MuteMusic"))
        {
            ToggleMusic();
            PlayClickSound();
        }

        // INPUT MANAGER: Mutear efectos
        if (Input.GetButtonDown("MuteSFX"))
        {
            ToggleSFX();
            PlayClickSound();
        }
    }

    //Controles de volumen

    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        if (musicSource != null) musicSource.mute = isMusicMuted;
        Debug.Log($"Música Muteada: {isMusicMuted}");
    }

    public void ToggleSFX()
    {
        isSfxMuted = !isSfxMuted;
        if (sfxSource != null) sfxSource.mute = isSfxMuted;
        Debug.Log($"SFX Muteados: {isSfxMuted}");
    }

    public void SetMusicPauseState(bool isGamePaused)
    {
        if (musicSource == null) return;
        if (isGamePaused) musicSource.mute = true;
        else musicSource.mute = isMusicMuted;
    }

    //Reproducción de efectos

    public void PlayMoneySound()
    {
        if (sfxSource != null && moneyPickupSound != null) sfxSource.PlayOneShot(moneyPickupSound);
    }

    public void PlayClickSound()
    {
        if (sfxSource != null && buttonClickSound != null) sfxSource.PlayOneShot(buttonClickSound);
    }

    public void PlayTrashSound()
    {
        if (sfxSource != null)
        {
            if (trashSound != null) sfxSource.PlayOneShot(trashSound);
            else if (buttonClickSound != null) sfxSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PlayPuddleSound()
    {
        if (sfxSource != null)
        {
            if (puddleCleanSound != null) sfxSource.PlayOneShot(puddleCleanSound);
            else if (buttonClickSound != null) sfxSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PlayCatSound()
    {
        if (sfxSource != null && catSound != null)
        {
            sfxSource.PlayOneShot(catSound);
        }
    }

    public void PlayChefFinishSound()
    {
        if (sfxSource != null)
        {
            if (chefFinishSound != null) sfxSource.PlayOneShot(chefFinishSound);
            else if (buttonClickSound != null) sfxSource.PlayOneShot(buttonClickSound);
        }
    }
}