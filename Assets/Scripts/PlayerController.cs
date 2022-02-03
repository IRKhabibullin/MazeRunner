using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform destination;
    public Transform startPosition;
    public LayerMask deathZoneLayer;
    public LayerMask finishZoneLayer;
    public Material defaultMaterial;
    public Material shieldMaterial;
    private bool shieldEnabled;
    private IEnumerator shieldCoroutine;
    public GameObject deathEffectPrefab;
    private bool isDead;
    public GameObject victoryEffect;
    public UnityEvent victoryEvent;

    void Start()
    {
        StartMoving();
    }

    public void StartMoving()
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
                StartCoroutine(Death());
                ResetPlayer();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((finishZoneLayer & (1 << other.transform.gameObject.layer)) != 0)
        {
            StartCoroutine(Victory());
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

    public void ResetPlayer()
    {
        agent.isStopped = true;
        agent.ResetPath();
        agent.Warp(startPosition.position);
    }

    private IEnumerator Death()
    {
        isDead = true;
        DisableShield();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        yield return StartCoroutine(EnableDeathEffect());
        isDead = false;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        StartCoroutine(MoveToFinish());
    }

    private IEnumerator Victory()
    {
        victoryEffect.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(3);
        victoryEvent?.Invoke();
    }

    private IEnumerator EnableDeathEffect()
    {
        var deathEffectObject = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3);
        Destroy(deathEffectObject);
    }
}
