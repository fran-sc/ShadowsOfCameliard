using UnityEngine;

// -----------------------------------------------------------------------------
// DamageSource
//
// Responsabilidades:
// - Aplica daño y knockback al contacto con colisionadores.
// - Sirve como fuente de daño base para armas cuerpo a cuerpo.
//
// Atributos principales:
// - damage: puntos de daño aplicados al impactar.
// - pushForce: fuerza del empuje aplicado al objetivo.
//
// Notas:
// - ExplosionDamageSource y ProyectileDamageSource extienden esta clase.
// - SetWeaponData permite configurar el daño en tiempo de ejecución.
// -----------------------------------------------------------------------------
public class DamageSource : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float pushForce;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Aplicamos daño al enemigo (si recibe daño)
        DamageReceiver enemyDamage = collision.gameObject.GetComponent<DamageReceiver>();
        if (enemyDamage != null)
        {
            enemyDamage.TakeDamage(damage);
        }

        // Aplicamos desplazamiento al enemigo (si se mueve)
        PushBack pushBack = collision.gameObject.GetComponent<PushBack>();
        if (pushBack != null)
        {
            pushBack.Push(
                (collision.transform.position - transform.position).normalized, 
                pushForce);
        }
    }
    
    public void SetWeaponData(int damage, float pushForce)
    {
        this.damage = damage;
        this.pushForce = pushForce;
    }
}
