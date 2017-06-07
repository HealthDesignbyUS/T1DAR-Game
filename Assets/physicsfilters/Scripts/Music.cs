using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class MusicUtil {
    public static IEnumerator PlayYield(this AudioSource audioSource, AudioClip clip) {
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSecondsRealtime(clip.length);
    }

    //Having fun with yields and extensions... sorry if this makes anyone rage.
    public static IEnumerator LoopWithYield<T>(this IEnumerable<T> enumerable, Func<T, IEnumerator> action) {
        while (true) {
            foreach (T i in enumerable.ToArray()) {
                yield return action(i);
                yield return null;
            }
        }
    }
}

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour {

    public AudioClip[] Clips;

    IEnumerator Start () {
	    AudioSource audioSource = GetComponent<AudioSource>();
        yield return Clips.LoopWithYield(clip => audioSource.PlayYield(clip));
	}
}
