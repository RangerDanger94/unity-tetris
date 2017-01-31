using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class VolumeControl : MonoBehaviour {
    public AudioMixer mixer;
    public EventSystem events;

	public void SetMasterVolume(float level)
    {
        mixer.SetFloat("MasterVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        mixer.SetFloat("MusicVolume", level);
    }

    public void SetEffectsVolume(float level)
    {
        mixer.SetFloat("EffectsVolume", level);
    }

    
}
