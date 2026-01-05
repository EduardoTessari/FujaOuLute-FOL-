using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // (Se você usar TextMeshPro)

public class UIManager : MonoBehaviour
{
    // --- 1. O SINGLETON (O "Telefone" Global) ---
    public static UIManager instance;

    [Header("Conexões da UI de Coleta")]
    public Slider progressBar;
    public Slider skillCheckBar;
    public GameObject skillCheckGroup;
    public SucessZone successZone;
    public GameObject btnToPressPrompt; // O seu "Aperte E"

    [Header("Configurações do Skill Check")]
    public KeyCode skillCheckKey = KeyCode.F;
    public float skillCheckDuration = 10f;
    public float skillCheckSpeed = 0.5f;

    // --- 2. O RESULTADO (O que o Coletável vai ler) ---
    [HideInInspector] // Esconde do Inspector
    public bool wasSkillCheckSuccessful;


    private void Awake()
    {
        // Lógica do Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // --- 3. AS FUNÇÕES DE CONTROLE ---

    public void ShowProgressBar(bool show)
    {
        progressBar.gameObject.SetActive(show);
        if (show) progressBar.value = 0; // Reseta a barra
    }

    public void UpdateProgressBar(float value) // (Valor de 0 a 1)
    {
        progressBar.value = value;
    }

    public void ShowInteractPrompt(bool show)
    {
        if (btnToPressPrompt != null)
            btnToPressPrompt.SetActive(show);
    }

    // --- 4. A CORROTINA (A LÓGICA DO SKILL CHECK) ---
    // Esta é a função que a Árvore vai chamar!
    public IEnumerator DoSkillCheckProcess(CollectItem collectable)
    {
        Debug.Log("Skill check started!");
        skillCheckGroup.SetActive(true);
        // Não precisamos mais do "wasSkillCheckSuccessful"

        float timer = 0f;
        float oscillationTimer = 0f;
        bool skillCheckUsed = false;

        while (timer < skillCheckDuration && !skillCheckUsed)
        {
            oscillationTimer += Time.deltaTime; // <-- LIGUE O CRONÔMETRO DO MOVIMENTO!

            // ... (lógica da oscilação da barra, igual) ...
            float oscillation = (-Mathf.Cos(oscillationTimer * skillCheckSpeed) + 1f) / 2f;
            skillCheckBar.value = oscillation;

            if (Input.GetKeyDown(skillCheckKey))
            {
                skillCheckUsed = true;
                if (successZone.isBarInside)
                {
                    Debug.Log("SKILL CHECK SUCESSO! (Pelo UIManager)");
                    AudioManager.instance.PlaySkillCheckSound(true);

                    // A MÁGICA! Ele chama uma função PÚBLICA no "Chefe"
                    collectable.ApplySkillCheckBonus();

                    // AudioManager.instance.PlaySkillCheckSound(true);
                }
                else
                {
                    Debug.Log("SKILL CHECK FALHA! (Pelo UIManager)");
                    // AudioManager.instance.PlaySkillCheckSound(false);
                    AudioManager.instance.PlaySkillCheckSound(false);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Skill check finished.");
        skillCheckGroup.SetActive(false);
    }
    public void HideSkillCheckUI()
    {
        // (Nós ainda precisamos de uma forma de parar a corrotina
        // que está rodando dentro dele... vamos ajustar isso)

        // Por enquanto, apenas desliga o grupo
        Debug.Log("Skill check canceled by CollectItem.");
        skillCheckGroup.SetActive(false);
    }
}