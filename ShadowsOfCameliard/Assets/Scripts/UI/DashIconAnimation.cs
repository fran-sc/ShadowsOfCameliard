using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------
// DashIconAnimation
//
// Responsabilidades:
// - Actualiza visualmente el icono de dash según el cooldown actual del jugador.
// - Reproduce animaciones de activación y disparo del dash.
//
// Atributos principales:
// - cooldownImage: imagen UI cuyo fill representa el progreso del cooldown.
// - anim: componente Animator que controla las animaciones del icono.
//
// Notas:
// - El fill de la imagen se mapea directamente al valor normalizado DashCooldownNormalized.
// - La animación "active" se dispara automáticamente cuando el dash está listo.
// -----------------------------------------------------------------------------
public class DashIconAnimation : MonoBehaviour
{
    Image cooldownImage;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        cooldownImage = GetComponent<Image>();
    }

    void Update()
    {
        // Mapea el cooldown normalizado del dash al fill de la imagen de progreso
        cooldownImage.fillAmount = PlayerController.Instance.DashCooldownNormalized;

        // Cuando el fill alcanza 1 el dash está listo; dispara la animación de activación
        if (cooldownImage.fillAmount >= 1f)
        {
            Active();
        }
    }

    // -----------------------------------------------------------------------------
    // Active
    //
    // - Reproduce la animación que indica que el dash está disponible.
    // -----------------------------------------------------------------------------
    public void Active()
    {
        anim.SetTrigger("active");
    }

    // -----------------------------------------------------------------------------
    // Fire
    //
    // - Reproduce la animación de uso del dash al ser activado por el jugador.
    // -----------------------------------------------------------------------------
    public void Fire()
    {
        anim.SetTrigger("fire");
    }

    // -----------------------------------------------------------------------------
    // Reset
    //
    // - Reinicia el Animator al estado inicial, cancelando cualquier animación en curso.
    //
    // Notas:
    // - Se utiliza Rebind() en lugar de un trigger para garantizar un reset completo.
    // -----------------------------------------------------------------------------
    public void Reset()
    {
        anim.Rebind();
    }
}
