using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform destination;
    public Transform startPosition;
    public LayerMask deathZoneLayer;
    public Material defaultMaterial;
    public Material shieldMaterial;
    private bool shieldEnabled;
    private IEnumerator shieldCoroutine;
    public GameObject deathEffectPrefab;
    public GameObject deathEffectObject;
    private bool isDead;

    void Start()
    {
        StartCoroutine(MoveToFinish());
    }

    private IEnumerator MoveToFinish()
    {
        yield return new WaitForSeconds(2);
        agent.SetDestination(destination.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if ((deathZoneLayer & (1 << other.transform.gameObject.layer)) != 0)
        {
            if (!shieldEnabled && !isDead)
            {
                StartCoroutine(ResetPlayer());
            }
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

    private IEnumerator ResetPlayer()
    {
        Debug.Log("Dead");
        isDead = true;
        EnableDeathEffect();
        DisableShield();
        agent.isStopped = true;
        agent.ResetPath();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(3);
        isDead = false;
        Destroy(deathEffectObject);
        transform.position = startPosition.position;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        StartCoroutine(MoveToFinish());
    }

    private void EnableDeathEffect()
    {
        deathEffectObject = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    }
}
