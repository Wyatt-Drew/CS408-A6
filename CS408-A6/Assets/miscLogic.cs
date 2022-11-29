using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miscLogic : MonoBehaviour
{
    public AudioSource SandAudio;
    private float sandVolume = 0.5f;
    private float emissionRate = 50f;
    public ParticleSystem part;
    void Start()
    {
        toggleSandVolume(false);
        SandAudio.Play();
    }
    // Update is called once per frame
    void Update()
    {
        //Creative feature (Sand sound)
        //Creative feature (Change emission rate)
        foreach (char c in Input.inputString.ToLower())
        {
            switch (c)
            {
                case '+':
                    {
                        emissionRate += 10.0f;
                        if (emissionRate > 60.0f)
                            emissionRate = 60.0f;
                        var emission = part.emission;
                        emission.rateOverTime = emissionRate;
                        sandVolume = emissionRate / 100f;
                        SandAudio.volume = sandVolume;
                        break;
                    }
                case '-':
                    {
                        emissionRate -= 10.0f;
                        if (emissionRate < 0)
                            emissionRate = 0;
                        var emission = part.emission;
                        emission.rateOverTime = emissionRate;
                        sandVolume = emissionRate / 100f;
                        SandAudio.volume = sandVolume;
                        break;
                    }
            }
        }
    }
    //Creative feature (pause emission)
    public void toggleEmission(bool toggle)
    {
        part.Play(); //Needed to allow particles to spawn at all
        var emission = part.emission;
        emission.enabled = toggle;
        toggleSandVolume(toggle);
        //part.Clear();  //clears all particles
    }
    //Creative feature (Sand sound)
    public void toggleSandVolume(bool active)
    {
        if (active)
            SandAudio.volume = sandVolume;
        else
            SandAudio.volume = 0;
    }
}
