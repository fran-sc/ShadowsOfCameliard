using System;
using UnityEngine;

// -----------------------------------------------------------------------------
// WelcomeMessage
//
// Responsabilidades:
// - Muestra un mensaje de bienvenida al cargar la escena por primera vez.
// - Espera a que el fade in termine antes de mostrar el mensaje.
//
// Atributos principales:
// - welcomeMessage: texto del mensaje de bienvenida.
// - delay: tiempo de espera; se sincroniza con FadeDuration si hay UIFade en escena.
//
// Notas:
// - El mensaje solo se muestra una vez por instancia (wasMessageShown).
// -----------------------------------------------------------------------------
public class WelcomeMessage : PersistentSingleton<WelcomeMessage>
{
    [Header("Welcome Message Settings")]
    [TextArea(3, 10)][SerializeField] string welcomeMessage;
    [SerializeField] bool showMessageOnStart = true;
    [SerializeField] float delay = 1f;

    bool wasMessageShown = false;

    void Start()
    {
        if (showMessageOnStart && !wasMessageShown)
        {
            wasMessageShown = true;

            // Hacemos un pequeño retraso antes de mostrar el mensaje para que se complete la transición de la escena y el fade-in.
            UIFade fade = FindAnyObjectByType<UIFade>();
            if (fade != null)
            {
                delay = fade.FadeDuration;
            }

            Invoke(nameof(ShowWelcomeMessage), delay); 
        }    
    }

    void ShowWelcomeMessage()
    {
        MessageManager.Instance.ShowMessage(welcomeMessage);
    }
}
