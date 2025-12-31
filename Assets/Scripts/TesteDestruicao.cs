using UnityEngine;

public class TesteDestruicao : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    public void Awake()
    {
        Destroy(gameObject, lifeTime);
    }
}
