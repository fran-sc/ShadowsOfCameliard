using System;
using System.Collections;
using UnityEngine;

// -----------------------------------------------------------------------------
// AudioManager
//
// Responsabilidades:
// - Centraliza la reproducción de efectos de sonido y música del juego.
// - Gestiona los volúmenes globales de música y efectos.
// - Permite hacer fade-out de la música en curso mediante corrutina.
//
// Atributos principales:
// - musicSource / effectsSource: AudioSources separados para música y efectos.
// - soundEffects: tabla de clips indexada por el enum Effect.
// - musicVolume / effectsVolume: volúmenes globales (0–1).
//
// Notas:
// - Los efectos se reproducen con PlayOneShot para evitar cortes entre sonidos.
// - Solo puede haber un fade de música activo a la vez.
// -----------------------------------------------------------------------------
public class AudioManager : PersistentSingleton<AudioManager>
{
    public enum Effect
    {
        None,
        SwordAttack1,
        SwordAttack2,
        SwordAttack3,
        ShieldBlock,
        AxeAttack1,
        AxeAttack2,
        BowAttack,
        OrcDeath,
        SkeletonDeath,
        PlayerDeath,
        ItemCollect,
        Explosion,
        Heal
    }

    [Serializable]
    private class SoundEntry
    {
        public Effect type;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;

    [Header("Sound Effects")]
    [SerializeField] private SoundEntry[] soundEffects;

    [Header("Global Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float effectsVolume = 1f;

    private Coroutine musicFadeCoroutine;

    protected override void Awake()
    {
        base.Awake();

        ConfigureAudioSources();
    }

    private void ConfigureAudioSources()
    {
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;
        }

        if (effectsSource != null)
        {
            effectsSource.loop = false;
            effectsSource.playOnAwake = false;
            effectsSource.volume = effectsVolume;
        }
    }

    // -----------------------------------------------------------------------------
    // PlayEffect (por enum)
    //
    // - Busca el AudioClip asociado al tipo de efecto y lo reproduce con volumen
    //   combinado (efectsVolume × volumen del entry).
    // -----------------------------------------------------------------------------
    public void PlayEffect(Effect soundType)
    {
        if (effectsSource == null)
        {
            Debug.LogWarning("AudioManager: No hay AudioSource asignado para efectos.");
            return;
        }

        SoundEntry soundEntry = GetSoundEntry(soundType);

        if (soundEntry == null)
        {
            Debug.LogWarning($"AudioManager: No se ha encontrado el sonido {soundType}.");
            return;
        }

        if (soundEntry.clip == null)
        {
            Debug.LogWarning($"AudioManager: El sonido {soundType} no tiene AudioClip asignado.");
            return;
        }

        float finalVolume = effectsVolume * soundEntry.volume;
        effectsSource.PlayOneShot(soundEntry.clip, finalVolume);
    }

    public void PlayEffect(AudioClip clip, float volume = 1f)
    {
        if (effectsSource == null)
        {
            Debug.LogWarning("AudioManager: No hay AudioSource asignado para efectos.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Se ha intentado reproducir un AudioClip nulo.");
            return;
        }

        float finalVolume = effectsVolume * Mathf.Clamp01(volume);
        effectsSource.PlayOneShot(clip, finalVolume);
    }

    // -----------------------------------------------------------------------------
    // PlayMusic
    //
    // - Inicia la reproducción de un clip de música en bucle.
    // - Si el clip ya está sonando y restartIfSameClip es false, solo restaura el volumen.
    // - Cancela cualquier fade en curso antes de reproducir.
    // -----------------------------------------------------------------------------
    public void PlayMusic(AudioClip musicClip, bool restartIfSameClip = false)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager: No hay AudioSource asignado para música.");
            return;
        }

        if (musicClip == null)
        {
            Debug.LogWarning("AudioManager: Se ha intentado reproducir una música nula.");
            return;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = null;
        }

        if (musicSource.clip == musicClip && musicSource.isPlaying && !restartIfSameClip)
        {
            musicSource.volume = musicVolume;
            return;
        }

        musicSource.clip = musicClip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
        {
            return;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = null;
        }

        musicSource.Stop();
        musicSource.volume = musicVolume;
    }

    // -----------------------------------------------------------------------------
    // StopMusicWithFade
    //
    // - Detiene la música con un fundido de volumen en fadeDuration segundos.
    // - Solo actúa si la música está reproduciéndose actualmente.
    // -----------------------------------------------------------------------------
    public void StopMusicWithFade(float fadeDuration = 1f)
    {
        if (musicSource == null)
        {
            return;
        }

        if (!musicSource.isPlaying)
        {
            return;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(StopMusicFadeCoroutine(fadeDuration));
    }

    // -----------------------------------------------------------------------------
    // StopMusicFadeCoroutine
    //
    // - Interpola el volumen de la música desde el valor actual hasta 0 a lo largo
    //   de fadeDuration segundos y detiene la reproducción al finalizar.
    // - Restaura el volumen configurado para futuros PlayMusic.
    // -----------------------------------------------------------------------------
    private IEnumerator StopMusicFadeCoroutine(float fadeDuration)
    {
        if (fadeDuration <= 0f)
        {
            musicSource.Stop();
            musicSource.volume = musicVolume;
            musicFadeCoroutine = null;
            yield break;
        }

        float startVolume = musicSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = elapsedTime / fadeDuration;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t);

            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = musicVolume;
        musicFadeCoroutine = null;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null && musicFadeCoroutine == null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetEffectsVolume(float volume)
    {
        effectsVolume = Mathf.Clamp01(volume);

        if (effectsSource != null)
        {
            effectsSource.volume = effectsVolume;
        }
    }

    private SoundEntry GetSoundEntry(Effect soundType)
    {
        foreach (SoundEntry soundEntry in soundEffects)
        {
            if (soundEntry.type == soundType)
            {
                return soundEntry;
            }
        }

        return null;
    }
}