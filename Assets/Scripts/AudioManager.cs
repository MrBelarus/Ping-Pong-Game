using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public SoundAudioClip[] soundAudioClipArray;

    public enum Sounds
    {
        Save,
        WallHit,
        Win,
        Defeat,
        Typing,
        TypingDelete,
        Goal,
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(Sounds sound, float pan = 1f, float pitch = 1f)
    {
        GameObject soundObj = new GameObject("Sound");
        AudioSource newSound = soundObj.AddComponent<AudioSource>();
        SoundAudioClip soundAudio = GetAudioClip(sound);

        newSound.clip = soundAudio.audioClip;
        newSound.volume = soundAudio.volume;
        newSound.panStereo = soundAudio.panEffect * pan;
        newSound.pitch = pitch;

        newSound.Play();

        Destroy(soundObj, newSound.clip.length / newSound.pitch);
    }

    SoundAudioClip GetAudioClip(Sounds sound)
    {
        foreach(SoundAudioClip audio in soundAudioClipArray)
        {
            if (audio.sound == sound)
            {
                return audio;
            }
        }

        Debug.LogError("Didn't find an audio clip!");
        return null;
    }

    [System.Serializable]
    public class SoundAudioClip
    {
        public Sounds sound;
        public AudioClip audioClip;

        [Range(0f, 1f)]
        public float volume;

        [Range(0f, 1f)]
        public float panEffect;
    }
}
