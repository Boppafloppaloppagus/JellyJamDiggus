using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFoodAnim : MonoBehaviour
{
    public SlimeNavAgent jellyBehavior;
    public void EatFood()
    {
        jellyBehavior.foodStuff.SetActive(false);
        jellyBehavior.foodStart = false;
        if (!jellyBehavior.jellyAudioSource.isPlaying || jellyBehavior.jellyAudioSource.clip == jellyBehavior.walkAudio)
        {
            jellyBehavior.jellyAudioSource.clip = jellyBehavior.eatAudio;
            jellyBehavior.jellyAudioSource.loop = false;
            jellyBehavior.jellyAudioSource.Play();
        }
    }
}
