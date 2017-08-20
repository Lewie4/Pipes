using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    [SerializeField] private AudioSource m_menuMusicSource;
    [SerializeField] private AudioSource m_gameMusicSource;
    [SerializeField] private AudioSource m_FXSource;

    [SerializeField] private AudioClip m_menuMusic;
    [SerializeField] private List<AudioClip> m_gameMusic;
    private int m_currentGameAudio = -1;

    private bool m_muted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }  

        if (PlayerPrefs.GetInt("AudioMuted", 0) == 0)
        {
            m_muted = false;
        }
        else
        {
            m_muted = true;
        }
    }

    private void Start()
    {
        m_menuMusicSource.clip = m_menuMusic;

        if (!m_muted)
        {
            m_menuMusicSource.Play();
        }

        SceneManager.activeSceneChanged += PlayCorrectAudio;
    }

    public void MuteAudio(bool active)
    {
        m_muted = !active;

        if (active)
        {
            PlayerPrefs.SetInt("AudioMuted", 0);
            m_menuMusicSource.volume = 1;
            m_gameMusicSource.volume = 1;

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "MainMenu" && !m_menuMusicSource.isPlaying)
            {
                m_menuMusicSource.Play();
            }
            else if (sceneName == "Game" && !m_gameMusicSource.isPlaying)
            {
                m_gameMusicSource.Play();
            }
        }
        else
        {
            PlayerPrefs.SetInt("AudioMuted", 1);
            m_menuMusicSource.volume = 0;
            m_gameMusicSource.volume = 0;
        }
    }

    private void PlayCorrectAudio(Scene prev, Scene current)
    {
        if (prev != current)
        {
            if (current.name == "MainMenu")
            {
                StartCoroutine(FadeOut(m_gameMusicSource, 0.5f));
                StartCoroutine(FadeIn(m_menuMusicSource, m_menuMusic, 0.5f));   
            }
            else if (current.name == "Game")
            {
                StartCoroutine(FadeOut(m_menuMusicSource, 0.5f));
                StartCoroutine(FadeIn(m_gameMusicSource, GetRandomAudio(m_gameMusic), 0.5f));   
            }
        }
    }

    private AudioClip GetRandomAudio(List<AudioClip> audio)
    {
        int random = m_currentGameAudio;
        while (random == m_currentGameAudio)
        {
            random = Random.Range(0, m_gameMusic.Count);
        }

        m_currentGameAudio = random;

        return audio[m_currentGameAudio];
    }

    public bool GetMuted()
    {
        return m_muted;
    }

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        if (!m_muted)
        {
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

                yield return null;
            }
        
            audioSource.volume = startVolume;
        }
        audioSource.Stop();
    }

    public IEnumerator FadeIn(AudioSource audioSource, AudioClip audio, float FadeTime)
    {
        float startVolume = 0.2f;

        audioSource.clip = audio;
        audioSource.volume = 0;
        audioSource.Play();

        if (!m_muted)
        {
            while (audioSource.volume < 1.0f)
            {
                if (m_muted)
                {
                    audioSource.volume = 0f;
                    break;
                }
                audioSource.volume += startVolume * Time.deltaTime / FadeTime;

                yield return null;
            }

            if (!m_muted)
            {
                audioSource.volume = 1f;
            }

        }
    }
}
