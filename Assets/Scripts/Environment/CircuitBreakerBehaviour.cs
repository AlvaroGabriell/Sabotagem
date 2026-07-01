using System.Collections.Generic;
using UnityEngine;

public class CircuitBreakerBehaviour : MonoBehaviour, IInteractable
{
    private readonly Dictionary<PlayerController, int> overlaps = new();
    public Animation circuitBreakerAnimation;
    public bool isActivated = false;

    [SerializeField] private float interactionCooldown = 0.5f;
    private float nextInteractionTime;

    [SerializeField] private Renderer[] renderers;

    void Start()
    {
        SetColor(Color.green);
    }

    public void Interact(PlayerController player)
    {
        if(Time.time < nextInteractionTime) return;

        nextInteractionTime = Time.time + interactionCooldown;

        if (!isActivated)
        {
            isActivated = true;
            SetColor(Color.red);
            circuitBreakerAnimation.Play("CircuitBreakerToDown");
            FindAnyObjectByType<LevelBootstrap>().CompleteLevel();
        }
        else
        {
            isActivated = false;
            SetColor(Color.green);
            circuitBreakerAnimation.Play("CircuitBreakerToUp");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody == null) return;
        if(!other.attachedRigidbody.TryGetComponent<PlayerController>(out var player)) return;

        if (!overlaps.ContainsKey(player))
        {
            overlaps[player] = 0;
            player.CurrentInteractable = this;
        }

        overlaps[player]++;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (!other.attachedRigidbody.TryGetComponent(out PlayerController player))
            return;

        if (!overlaps.ContainsKey(player))
            return;

        overlaps[player]--;

        if (overlaps[player] <= 0)
        {
            overlaps.Remove(player);
            player.CurrentInteractable = null;
        }
    }

    private void SetColor(Color color)
    {
        foreach(Renderer renderer in renderers)
        {
            foreach(Material mat in renderer.materials)
            {
                Color current = mat.GetColor("_BaseColor");
                current.r = color.r;
                current.g = color.g;
                current.b = color.b;

                mat.SetColor("_BaseColor", current);
            }
        }
    }
}
