using UnityEngine;

public class MobTrigger : MonoBehaviour
{
    public int waveIndex; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MobTriggerManager manager = FindObjectOfType<MobTriggerManager>();
            if (manager != null)
                manager.StartWave(waveIndex);
            Debug.Log($"🚪 Player entered trigger for wave {waveIndex}");
        }
    }
}
