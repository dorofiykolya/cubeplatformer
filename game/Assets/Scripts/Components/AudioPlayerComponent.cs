using System.Collections.Generic;
using UnityEngine;

namespace Game.Components
{
  public class AudioPlayerComponent : MonoBehaviour
  {
    [SerializeField]
    private AudioClip[] _clips;

    private Dictionary<string, AudioClip> _clipMap = new Dictionary<string, AudioClip>();
    private AudioSource _sounds;
    private AudioSource _themes;


    public void Play(string sound)
    {
      AudioClip clip;
      if (_clipMap.TryGetValue(sound, out clip))
      {
        Sounds.Stop();
        Sounds.loop = false;
        Sounds.clip = clip;
        Sounds.Play();
      }
    }

    public void Stop(string sound)
    {
      AudioClip clip;
      if (_clipMap.TryGetValue(sound, out clip))
      {
        if (Sounds.clip.name == sound)
        {
          Sounds.Stop();
        }
        else
        {
          Loops.Stop();
        }
      }
    }

    public void PlayLoop(string sound)
    {
      AudioClip clip;
      if (_clipMap.TryGetValue(sound, out clip))
      {
        Loops.Stop();
        Loops.loop = true;
        Loops.clip = clip;
        Loops.Play();
      }
    }

    private AudioSource Sounds
    {
      get { return _sounds ?? (_sounds = gameObject.AddComponent<AudioSource>()); }
    }

    private AudioSource Loops
    {
      get { return _themes ?? (_themes = gameObject.AddComponent<AudioSource>()); }
    }


    private void Awake()
    {
      foreach (var audioClip in _clips)
      {
        _clipMap[audioClip.name.ToLower()] = audioClip;
      }
    }
  }
}
