using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform destination;
    public LayerMask deathZoneLayer;
    public Material defaultMaterial;
    public Material shieldMaterial;
    private bool shieldEnabled;
    private IEnumerator shieldCoroutine;

    void Start()
    {
        StartCoroutine(MoveToFinish());
    }

    private IEnumerator MoveToFinish()
    {
        yield return new WaitForSeconds(2);
        agent.SetDestination(destination.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((deathZoneLayer & (1 << other.transform.gameObject.layer)) != 0)
        {
            if (!shieldEnabled)
                Debug.Log("Death");
        }
    }

    public void EnableShield()
    {
        shieldCoroutine = ShieldCoroutine();
        StartCoroutine(shieldCoroutine);
    }

    public IEnumerator ShieldCoroutine()
    {
        gameObject.GetComponent<MeshRenderer>().material = shieldMaterial;
        shieldEnabled = true;
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        shieldEnabled = false;
    }

    public void DisableShield()
    {
        shieldEnabled = false;
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
        }
    }
}
