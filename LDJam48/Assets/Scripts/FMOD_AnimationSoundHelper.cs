using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMOD_AnimationSoundHelper : MonoBehaviour {
    public string soundToPlay = "";
    // Start is called before the first frame update

    public void PlaySound () {
        AudioManager.instance.PlaySFX (soundToPlay);
    }
}