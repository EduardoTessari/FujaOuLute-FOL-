using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // --- 1. O SINGLETON (O "Telefone" Global) ---
    // (O mesmo truque que usamos no UIManager)
    public static AudioManager instance;

    [Header("Componentes do Maestro")]
    // (Vamos ter 2 alto-falantes: 1 para sons curtos, 1 para loops)
    [SerializeField] private AudioSource _sfxSource; // Para "PlayOneShot" (acerto, falha, plin)
    [SerializeField] private AudioSource _loopSource; // Para "Play/Stop" (coleta, música)
    [SerializeField] private AudioSource _musicSource; // NOVO: Exclusivo para BG

    [Header("Música")]
    public AudioClip backgroundMusic;

    [Header("A Jukebox - Sons de UI/Feedback")]
    // (Como você planejou, a "biblioteca" de sons vive aqui)
    public AudioClip skillCheckSuccess;
    public AudioClip skillCheckFail;
    public AudioClip depositSuccess;
    public AudioClip depositFail;
    // ... (futuramente: som de "inventário vazio", etc.)

    [Header("A Jukebox - Sons de Coleta")]
    public AudioClip woodCollectLoop;
    public AudioClip vineCollectLoop;
    public AudioClip waterCollectLoop;
    public AudioClip foodCollectLoop;
    public AudioClip footstepSound;
    // ... (futuramente: som de "item coletado")


    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); } else { instance = this; }

        // Configura os 3 tocadores
        if (_sfxSource == null) { _sfxSource = gameObject.AddComponent<AudioSource>(); }

        if (_loopSource == null)
        {
            _loopSource = gameObject.AddComponent<AudioSource>();
            _loopSource.loop = true;
        }

        // --- CONFIGURAÇÃO DA MÚSICA ---
        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;  // Música sempre em loop
            _musicSource.volume = 0.5f; // Dica: Música geralmente é mais baixa que SFX
        }
    }

    private void Start()
    {
        // --- TOCA A MÚSICA ASSIM QUE O JOGO COMEÇA ---
        if (backgroundMusic != null)
        {
            _musicSource.clip = backgroundMusic;
            _musicSource.Play();
        }
    }

    // --- 2. AS FUNÇÕES PÚBLICAS (O que os outros scripts vão chamar) ---

    // (Exatamente como você planejou!)
    public void PlaySkillCheckSound(bool didSuccess)
    {
        AudioClip clipToPlay = (didSuccess) ? skillCheckSuccess : skillCheckFail;
        if (clipToPlay != null)
        {
            _sfxSource.PlayOneShot(clipToPlay); // Toca no alto-falante de EFEITOS
        }
    }

    // (Exatamente como você planejou!)
    public void PlayDepositSound(bool didSuccess)
    {
        AudioClip clipToPlay = (didSuccess) ? depositSuccess : depositFail;
        if (clipToPlay != null)
        {
            _sfxSource.PlayOneShot(clipToPlay); // Toca no alto-falante de EFEITOS
        }
    }

    // (A lógica do "if (arvore) play.chopaudio)" que você sugeriu!)
    public void PlayCollectingLoop(ItemList item)
    {
        AudioClip clipToPlay = null;
        switch (item)
        {
            case ItemList.Wood:
                clipToPlay = woodCollectLoop;
                break;
            case ItemList.Vine:
                clipToPlay = vineCollectLoop;
                break;
            case ItemList.Water:
                clipToPlay = waterCollectLoop;
                break;
            case ItemList.Food:
                clipToPlay = foodCollectLoop;
                break;
        }

        if (clipToPlay != null)
        {
            _loopSource.clip = clipToPlay;
            _loopSource.Play(); // Toca no alto-falante de LOOP
        }
    }

    public void PlayFootstep()
    {
        // Dica de Polimento: Podemos variar o pitch aqui no futuro para não ficar robótico
        if (footstepSound != null)
        {
            _sfxSource.PlayOneShot(footstepSound);
        }
    }
    public void StopCollectingLoop()
    {
        _loopSource.Stop();
    }
}