using UnityEngine;

public class DangerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerMover player))
        {
            if (!player.IsShielded)
            {
                player.Die();
            }
        }
    }
}
