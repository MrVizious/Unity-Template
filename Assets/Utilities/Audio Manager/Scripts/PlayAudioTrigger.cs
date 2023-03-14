using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Audio
{
    public class PlayAudioTrigger : MonoBehaviour
    {
        public enum AudioType
        {
            Sound,
            Music
        }
        public AudioClip clip;
        public AudioType audioType;

        [ShowIf("audioType", AudioType.Sound)]
        public float minPitchRange, maxPitchRange;

        [ShowIf("audioType", AudioType.Music)]
        public float secondsToFadeIn, secondsToFadeOut;

        [Button]
        public void Invoke()
        {
            switch (audioType)
            {
                case AudioType.Sound:
                    AudioManager.Instance.PlaySound(clip, minPitchRange, maxPitchRange);
                    break;
                case AudioType.Music:
                    AudioManager.Instance.PlayMusic(clip, secondsToFadeIn, secondsToFadeOut);
                    break;
            }
        }
    }
}
