using System.Collections;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private float blinkInterval = 0.1f;

    void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>(true);
    }

    private void OnEnable()
    {
        // whenever this GameObject becomes active, kick off the blink again
        StartCoroutine(BlinkRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            meshRenderer.enabled = !meshRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}