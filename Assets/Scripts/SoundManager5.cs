using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager5 : MonoBehaviour
{
    public static SoundManager5 instance { get; private set; }
    public bool soundEnabled { get; private set; }

    [SerializeField] private AudioSource[] audioSource;
    [SerializeField] private GameObject musicButton, muteButton;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        UpdateSound();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
        UpdateSound();
    }

    public void UpdateSound()
    {
        foreach (var item in audioSource)
            item.volume = soundEnabled ? 1f : 0f;

        if (musicButton && muteButton)
        {
            musicButton.SetActive(soundEnabled);
            muteButton.SetActive(!soundEnabled);
        }
    }

    public void PlaySound(int index)
    {
        if (soundEnabled && index >= 0 && index < audioSource.Length)
            audioSource[index].Play();
    }
}
