using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private ParticleSystem victoryEffect; // confetti
    [SerializeField] private Transform finishPoint;
    [SerializeField] private Transform startPoint;
    [SerializeField] private LayerMask deathZoneLayer;
    [SerializeField] private LayerMask finishZoneLayer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material shieldMaterial;
    [SerializeField] private GameObject deathEffectPrefab;

    private bool isDead;
    private bool shieldEnabled;
    private IEnumerator shieldCoroutine;

    public UnityEvent victoryEvent;

    void Start()
    {
        pathLine.positionCount = 0;
        StartMoving();
    }

    private void Update()
    {
        if (!isDead && agent.hasPath)
        {
            DrawPath();
        }
    }

    public void StartMoving()
    {
        StartCoroutine(MoveToFinish());
    }

    private IEnumerator MoveToFinish()
    {
        yield return new WaitForSeconds(2);
        agent.SetDestination(finishPoint.position);
    }

    private void DrawPath()
    {
        pathLine.positionCount = agent.path.corners.Length;
        pathLine.SetPositions(agent.path.corners);
    }

    private void OnTriggerStay(Collider other)
    {
        if ((deathZoneLayer & (1 << other.transform.gameObject.layer)) != 0)
        {
            if (!shieldEnabled && !isDead)
            {
                StartCoroutine(Death());
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
        agent.Warp(startPoint.position);
    }

    private IEnumerator Death()
    {
        isDead = true;
        pathLine.positionCount = 0;
        DisableShield();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        yield return StartCoroutine(EnableDeathEffect());
        ResetPlayer();
        isDead = false;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        StartMoving();
    }

    private IEnumerator Victory()
    {
        victoryEffect.Play();
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
