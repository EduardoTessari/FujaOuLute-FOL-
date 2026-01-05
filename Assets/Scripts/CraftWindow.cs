using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftWindow : MonoBehaviour
{
    [Header("Dados")]
    public CraftData receitaAtual;

    [Header("Referências Externas")]
    public InventoryUI inventarioDoJogador; // O script que você acabou de me passar

    [Header("UI References - Janela")]
    public Button botaoCraftar;             // O botão de "Criar Item"
    public InventorySlot slotResultado;     // O slot grande do resultado

    [Header("UI References - Grids")]
    public Transform containerRequisitos;   // Grid de cima (O que a receita PEDE)
    public Transform containerItensJogador; // Grid de baixo (O que o jogador TEM)

    [Header("Prefabs")]
    public GameObject prefabInventorySlot;

    private void Start()
    {
        // Se arrastar o botão no inspector, já configura o clique dele
        if (botaoCraftar != null)
        {
            botaoCraftar.onClick.AddListener(TentarCraftar);
        }

        if (receitaAtual != null)
            AtualizarJanela();
    }

    private void OnEnable()
    {
        AtualizarJanela();
    }

    public void SetReceita(CraftData novaReceita)
    {
        receitaAtual = novaReceita;
        AtualizarJanela();
    }

    public void AtualizarJanela()
    {
        //Debug.Log("1. AtualizarJanela foi chamado!");

        // --- 1. LIMPEZA ---
        foreach (Transform child in containerRequisitos) Destroy(child.gameObject);
        foreach (Transform child in containerItensJogador) Destroy(child.gameObject);

        if (receitaAtual == null)
        {
            if (slotResultado != null) slotResultado.ClearSlot();
            if (botaoCraftar != null) botaoCraftar.interactable = false;
            return;
        }

        // --- 2. MOSTRAR RESULTADO ---
        if (slotResultado != null)
        {
            slotResultado.SetupForCrafting(receitaAtual.itemResultado, receitaAtual.quantidadeResultado);
        }

        // Variável para controlar se podemos liberar o botão
        bool temTodosMateriais = true;

        // --- 3. LOOP DOS REQUISITOS ---
        if (receitaAtual.ingredientesNecessarios != null)
        {
            foreach (var ingrediente in receitaAtual.ingredientesNecessarios)
            {
                // A. Grid de Cima: O que PRECISA
                CriarSlot(containerRequisitos, ingrediente.item, ingrediente.quantidade);

                // B. Consultar o Inventário
                // Aqui usamos o seu método "GetItemCount"
                int qtdQueTenho = inventarioDoJogador.GetItemCount(ingrediente.item);

                //Debug.Log($"2. Verificando item: {ingrediente.item.name}. O Inventário diz que tem: {qtdQueTenho}");

                // C. Grid de Baixo: O que TENHO
                // (Só mostra se tiver pelo menos 1, como combinamos)
                if (qtdQueTenho > 0)
                {
                    CriarSlot(containerItensJogador, ingrediente.item, qtdQueTenho);
                }

                // D. Verificação Lógica
                if (qtdQueTenho < ingrediente.quantidade)
                {
                    temTodosMateriais = false;
                }
            }
        }

        // --- 4. ATUALIZAR BOTÃO ---
        if (botaoCraftar != null)
        {
            botaoCraftar.interactable = temTodosMateriais;
        }
    }

    // Função chamada ao clicar no botão
    public void TentarCraftar()
    {
        if (receitaAtual == null) return;

        // Verifica de novo só por segurança (evita bugs se dropar item com a janela aberta)
        foreach (var ing in receitaAtual.ingredientesNecessarios)
        {
            if (inventarioDoJogador.GetItemCount(ing.item) < ing.quantidade)
            {
                Debug.Log("Tentou craftar sem materiais suficientes!");
                AtualizarJanela(); // Atualiza visual para bloquear botão
                return;
            }
        }

        // 1. Remove os materiais (Consome)
        foreach (var ing in receitaAtual.ingredientesNecessarios)
        {
            inventarioDoJogador.RemoveItem(ing.item, ing.quantidade);
        }

        // 2. Adiciona o item novo
        inventarioDoJogador.AddItem(receitaAtual.itemResultado, receitaAtual.quantidadeResultado);

        // 3. Atualiza a tela (os números vão diminuir no grid de baixo)
        AtualizarJanela();

        Debug.Log("Item craftado com sucesso!");
    }

    // Funçãozinha auxiliar pra instanciar o prefab
    void CriarSlot(Transform container, ItemData item, int quantidade)
    {
        GameObject novoSlotObj = Instantiate(prefabInventorySlot, container);
        InventorySlot slotScript = novoSlotObj.GetComponent<InventorySlot>();

        if (slotScript != null)
        {
            // O slot já vai ficar "cinza" e sem clique graças àquele seu SetupForCrafting
            slotScript.SetupForCrafting(item, quantidade);
        }
    }
}