using UnityEngine;
using System;
using System.Collections;

public enum SoundType
{
    HOE,
    WATERING,
    PLANTING,
    HARVESTING,
    PICKUP,
    CHEST,
    UICLICK,
    SHOPBUY,
    SHOPSELL,
    NOTALLOWED,
    TOOLSWITCH,
    FOOTSTEP,
    QUESTCOMPLETE
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 0.25f)
    {
        if (instance == null)
        {
            Debug.LogWarning("SoundManager is missing in the scene!");
            return;
        }

        SoundList list = instance.soundList[(int)sound];

        if (list.sounds == null || list.sounds.Length == 0)
        {
            Debug.LogWarning("No sounds assigned for " + sound);
            return;
        }

       
        AudioClip clip = list.sounds[0];

        Debug.Log("Playing sound type: " + sound + " | clip: " + clip.name + "\n" + Environment.StackTrace);


        instance.audioSource.PlayOneShot(clip, volume);
    }

    public static void PlaySoundForDuration(SoundType sound, float duration, float volume = 1f)
    {
        if (instance == null) return;

        instance.StartCoroutine(instance.PlayClipForTime(sound, duration, volume));
    }

    private IEnumerator PlayClipForTime(SoundType sound, float duration, float volume)
    {
        SoundList list = soundList[(int)sound];

        if (list.sounds == null || list.sounds.Length == 0) yield break;

        AudioClip clip = list.sounds[0];

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(duration);

        audioSource.Stop();
    }
}


[Serializable]
public struct SoundList
{
    
    [HideInInspector] public string name;
    public AudioClip[] sounds;
}

