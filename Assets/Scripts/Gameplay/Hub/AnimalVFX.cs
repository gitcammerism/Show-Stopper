using UnityEngine;

// This script manages the display of VFX for the animals in the hub.

public class AnimalVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem frustrationVFX;
    [SerializeField] private ParticleSystem affectionVFX;

    private void Awake()
    {
        HideAll();
    }

    public void SetMood(bool isFrustrated)
    {
        SetParticleState(frustrationVFX, isFrustrated);
        SetParticleState(affectionVFX, !isFrustrated);
    }

    public void HideAll()
    {
        SetParticleState(frustrationVFX, false);
        SetParticleState(affectionVFX, false);
    }

    // Turns particple systems on and off
    private void SetParticleState(ParticleSystem particleSystem, bool shouldPlay)
    {
        if (particleSystem == null) return;

        if (shouldPlay)
        {
            if (!particleSystem.gameObject.activeSelf)
                particleSystem.gameObject.SetActive(true);

            if (!particleSystem.isPlaying)
                particleSystem.Play();
        }
        else
        {
            if (particleSystem.isPlaying)
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (particleSystem.gameObject.activeSelf)
                particleSystem.gameObject.SetActive(false);
        }
    }
}