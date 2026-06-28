using UnityEngine;

// -----------------------------------------------------------------------------
// Eagle
//
// Responsabilidades:
// - Hace volar al águila en una dirección al detectar una colisión.
// - Activa el árbol mágico asociado como efecto de la interacción.
// - Se destruye automáticamente tras flyAwayDuration segundos.
//
// Atributos principales:
// - flyAwayDirection: dirección normalizada de vuelo.
// - flyAwaySpeed: velocidad de desplazamiento.
// - magicTreeController: referencia al árbol que se activa al volar.
// -----------------------------------------------------------------------------
public class Eagle : MonoBehaviour
{
    [SerializeField] Vector2 flyAwayDirection = new Vector2(1, 1);
    [SerializeField] float flyAwaySpeed = 5f;
    [SerializeField] float flyAwayDuration = 3f;
    [SerializeField] AudioClip eagleSound;

    [SerializeField] MagicTreeController magicTreeController; // Referencia al MagicTreeController
    Animator anim;
    bool flyAway = false;
    
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (flyAway)
        {
            transform.Translate(flyAwayDirection.normalized * flyAwaySpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetTrigger("fly");

        flyAway = true;

        // Reproducimos el sonido del águila
        if (eagleSound != null)
        {
            AudioManager.Instance.PlayEffect(eagleSound);
        }

        // Si tenemos una referencia al MagicTreeController, activamos el árbol mágico
        if (magicTreeController != null)
        {
            magicTreeController.ActivateTree();
        }

        Destroy(gameObject, flyAwayDuration);
    }

}
