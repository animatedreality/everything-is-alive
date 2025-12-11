using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(AudioSource))]
public class XRInteractionAudioTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip hoverEnterClip;
    [SerializeField] private AudioClip hoverExitClip;
    [SerializeField] private AudioClip selectEnterClip;
    [SerializeField] private AudioClip selectExitClip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    private XRBaseInteractable interactable;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        PlayClip(hoverEnterClip);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        PlayClip(hoverExitClip);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        PlayClip(selectEnterClip);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        PlayClip(selectExitClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
