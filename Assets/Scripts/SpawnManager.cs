using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Configuração do Ninho")]
    [SerializeField] private GameObject enemyPrefab; // O Sapo
    [SerializeField] private float spawnInterval = 5f; // Tempo entre nascimentos
    [SerializeField] private float spawnRadius = 3f; // Tamanho da área do ninho
    [SerializeField] private int maxEnemies = 5; // Para não lagar o jogo

    [Header("Configuração de Colisão")]
    [SerializeField] private float objectRadius = 0.5f; // O espaço que a árvore/sapo ocupa
    [SerializeField] private LayerMask obstacleLayer;   // O que conta como "obstáculo"? (Padrão: Everything)
    [SerializeField] private int maxSpawnAttempts = 10; // Quantas vezes tentar antes de desistir (pra não travar o jogo)

    private void Start()
    {
        // Começa a gerar a prole
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true) // Loop infinito
        {
            yield return new WaitForSeconds(spawnInterval);

            // Conta quantos filhos este ninho tem ativos agora
            // (Isso é uma forma simples de limitar. O ideal seria lista, mas para V1 serve)
            if (transform.childCount < maxEnemies)
            {
                SpawnObject();
            }
        }
    }

    private void SpawnObject()
    {
        // Tenta achar um lugar vazio X vezes
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // 1. Sorteia o ponto
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomPos.x, randomPos.y, 0);

            // 2. O "SENSOR DE ESTACIONAMENTO"
            // Verifica se tem algum colisor nesse ponto
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, objectRadius, obstacleLayer);

            // 3. Se hit for NULL, significa que o lugar está vazio!
            if (hit == null)
            {
                GameObject newObject = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                newObject.transform.parent = transform;
                return; // Sucesso! Sai da função.
            }

            // Se chegou aqui, é porque tinha algo. O loop roda de novo para tentar outro lugar.
        }

        Debug.LogWarning($"Spawner {gameObject.name} não conseguiu achar lugar vazio após {maxSpawnAttempts} tentativas!");
    }

    // Desenha o ninho no editor para você ver onde eles nascem
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}