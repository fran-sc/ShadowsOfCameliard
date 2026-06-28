using UnityEngine;

// -----------------------------------------------------------------------------
// ProyectileDamageSource
//
// Responsabilidades:
// - Extiende DamageSource para proyectiles que se destruyen al impactar.
// - Aplica daño a través de la clase base y destruye el proyectil.
// -----------------------------------------------------------------------------
public class ProyectileDamageSource : DamageSource
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // Destruimos el proyectil al colisionar con cualquier objeto
        Destroy(gameObject);
    }
}
