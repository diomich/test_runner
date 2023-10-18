using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// Simple implementation of ISoundManager interface. Internally provides a reusable set
    /// of audio sources to play several one-shot sounds simultaneously. Available sounds can
    /// be set up in the Editor. 
    /// </summary>
    public class SoundManagerSimple : MonoBehaviour, ISoundManager
    {
        [Serializable]
        class SoundDesc
        {
            public string Id;
            public AudioClip Clip;
        }

        [SerializeField] 
        private List<SoundDesc> _knownSounds;

        [SerializeField] 
        private int _audioSourceSfxCount = 8;
        
        private AudioSource _backgroundSource;
        
        private Dictionary<string, AudioClip> _clipsBase;

        private List<AudioSource> _freeSources;
        private List<AudioSource> _activeSources;
        private List<AudioSource> _finishedSources;

        private void Awake()
        {
            _clipsBase = new Dictionary<string, AudioClip>(_knownSounds.Count);
            for (int i = 0; i < _knownSounds.Count; i++)
            {
                _clipsBase.Add(_knownSounds[i].Id, _knownSounds[i].Clip);
            }

            _freeSources = new List<AudioSource>(_audioSourceSfxCount);
            for (int i = 0; i < _audioSourceSfxCount; i++)
            {
                GameObject goSfx = new GameObject("sfx");
                goSfx.transform.SetParent(this.transform);
                AudioSource audioSource = goSfx.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
                _freeSources.Add(audioSource);
            }

            _activeSources = new List<AudioSource>(_audioSourceSfxCount);
            _finishedSources = new List<AudioSource>(_audioSourceSfxCount);

            GameObject goBack = new GameObject("background");
            goBack.transform.SetParent(this.transform);
            _backgroundSource = goBack.AddComponent<AudioSource>();
            _backgroundSource.playOnAwake = false;
            _backgroundSource.loop = true;
            _backgroundSource.volume = 0.2f;
        }

        void Update()
        {
            for (int i = 0; i < _activeSources.Count; i++)
            {
                AudioSource sourceIt = _activeSources[i]; 
                if (sourceIt.isPlaying == false)
                {
                    _finishedSources.Add(sourceIt);
                }
            }

            if (_finishedSources.Count > 0)
            {
                for (int i = 0; i < _finishedSources.Count; i++)
                {
                    AudioSource sourceIt = _finishedSources[i];
                    _activeSources.Remove(sourceIt);
                    _freeSources.Add(sourceIt);
                }

                _finishedSources.Clear();
            }
        }

        void ISoundManager.TryPlayOneshot(string id)
        {
            if (id != null && _clipsBase.ContainsKey(id))
            {
                (this as ISoundManager).PlayOneshot(id);
            }
        }

        void ISoundManager.PlayOneshot(string id)
        {
            AudioSource source = GetSourceForSfx();
            source.clip = ClipForId(id);
            source.Play();
        }

        void ISoundManager.PlayBackground(string id)
        {
            _backgroundSource.clip = ClipForId(id);
            _backgroundSource.Play();
        }

        void ISoundManager.PauseBackground()
        {
            Debug.Assert(_backgroundSource.isPlaying == true);
            _backgroundSource.Pause();
        }

        void ISoundManager.ResumeBackground()
        {
            Debug.Assert(_backgroundSource.isPlaying == false);
            _backgroundSource.Play();
        }

        void ISoundManager.StopBackground()
        {
            Debug.Assert(_backgroundSource.isPlaying == true, "illegal stop background");
            _backgroundSource.Stop();
            _backgroundSource.clip = null;
        }

        private AudioClip ClipForId(string id)
        {
            Debug.Assert(_clipsBase.ContainsKey(id) == true, "unknown sound");
            return _clipsBase[id];
        }

        private AudioSource GetSourceForSfx()
        {
            AudioSource result = null;
            if (_freeSources.Count > 0)
            {
                int index = _freeSources.Count - 1;
                result = _freeSources[index];
                _freeSources.RemoveAt(index);
                _activeSources.Add(result);
            }
            else
            {
                // take the oldest one
                result = _activeSources[0];
                result.Stop();
            }

            return result;
        }
    }
}
