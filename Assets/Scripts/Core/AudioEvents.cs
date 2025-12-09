using UnityEngine;

public class AudioEvents : MonoBehaviour
{
    [Header("Audio Source Hook")]
    public AudioSource src;

    [Header("Audio Clip Hook(s)")]
    public AudioClip attackHit;
    public AudioClip parry;
    public AudioClip dash;
    public AudioClip uiBlip;

    public void PlayAttackHit() => src.PlayOneShot(attackHit);
    public void PlayParry() => src.PlayOneShot(parry);
    public void PlayDash() => src.PlayOneShot(dash);
    public void PlayUIBlip() => src.PlayOneShot(uiBlip);
}