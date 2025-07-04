using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudionManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private AudioSource audio;
    void Start()
    {
        EventBus.Bus.AddListener<string>(EventId.OnSound, PlaySound);
        
    }
	private void OnDestroy() {
		EventBus.Bus.RemoveListener<string>(EventId.OnSound, PlaySound);
	}

	private void PlaySound(string clipname) {
        audio.PlayOneShot(clips.FirstOrDefault(x=>x.name==clipname));
	}
}
