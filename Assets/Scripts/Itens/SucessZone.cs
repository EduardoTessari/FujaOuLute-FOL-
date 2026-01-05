using UnityEngine;

public class SucessZone : MonoBehaviour
{
    public bool isBarInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Quando o preenchimento entra na zona
        isBarInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Quando o preenchimento sai da zona
        isBarInside = false;
    }
}
