using System.Collections;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private float blinkInterval = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Look through all children (including inactive ones).
        meshRenderer = GetComponentInChildren<MeshRenderer>(true);

        if (meshRenderer == null)
        {
            Debug.LogError("No MeshRenderer found in children of " + gameObject.name);
            return;
        }

        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            meshRenderer.enabled = !meshRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

        private void OnDisable()
    {
        // Ensure we stop the blinking if this object is deactivated
        StopAllCoroutines();
    }
}
