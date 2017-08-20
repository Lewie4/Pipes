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
    }

    private void Start()
    {
        m_menuMusicSource.clip = m_menuMusic;

        m_menuMusicSource.Play();

        SceneManager.activeSceneChanged += PlayCorrectAudio;
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

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public static IEnumerator FadeIn(AudioSource audioSource, AudioClip audio, float FadeTime)
    {
        float startVolume = 0.2f;

        audioSource.clip = audio;
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.volume = 1f;
    }
}
