using UnityEngine;

[EventBusSubscriber]
public class HealthEventHandlers
{
    [SubscribeEvent(priority: -1000)] // Prioridade baixa pra rodar por último, garantindo que o dano vai ser aplicado ao final. 
    private static void OnPlayerDamage(HealthEvents.DamageEvent evt)
    {
        if(evt.target is not PlayerController player) return;

        ParticleManager.Instance.SpawnParticle("clouds", Utils.GetVisualCenter(player.gameObject) + new Vector3(0, 0, -0.72f));
        player.transform.position = player.LastSafePos;
        player.Rb.linearVelocity = Vector3.zero;

        if(player.IsCamouflaged) player.SetCamouflaged(false);
        
        evt.damage = 0; // Anula o dano, já que a punição por morrer é ser teleportado de volta pro último safe spot.
    }
}
