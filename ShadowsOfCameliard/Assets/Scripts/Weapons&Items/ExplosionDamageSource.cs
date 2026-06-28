using UnityEngine;

// -----------------------------------------------------------------------------
// ExplosionDamageSource
//
// Responsabilidades:
// - Extiende DamageSource para el daño de área de la bomba.
// - Solo aplica daño si la bomba ya ha explotado (HasExploded).
// - Gestiona la destrucción de blockades antes de delegar en la clase base.
//
// Notas:
// - Los blockades reciben tratamiento especial: se destruyen sin daño estándar.
// -----------------------------------------------------------------------------
public class ExplosionDamageSource : DamageSource
{
    // Bloquea el daño hasta que la bomba haya explotado; destruye blockades aparte
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GetComponentInParent<BombController>().HasExploded)
            return;
            
        if (collision.gameObject.GetComponent<BlockadeDestroy>() != null)
        {
            collision.gameObject.GetComponent<BlockadeDestroy>().DestroyBlockade();
            return;
        }

        base.OnTriggerEnter2D(collision);
    }
}
