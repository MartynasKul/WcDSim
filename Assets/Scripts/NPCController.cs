using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform counterPoint;
    public Transform exitPoint;
    public AudioSource entrySound;
    public float waitTimeAtCounter = 15f;
    private bool hasBurger = false;
    private NavMeshAgent agent;

    private enum NPCState { Entering, Waiting, Exiting }
    private NPCState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Vector3 dest= new Vector3(3.308f,-0.1227435f, -7.37f);
        currentState = NPCState.Entering;
        agent.SetDestination(counterPoint.position);
        entrySound.Play();
        Debug.Log($"{gameObject.name} is entering the store, heading to the counter.");
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Burger"))
        {
            GiveBurger();
            Destroy(other.gameObject);
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case NPCState.Entering:
                Debug.Log("Entering state");
                if (Vector3.Distance(transform.position, counterPoint.position) < 2f)
                {
                    currentState = NPCState.Waiting;
                    StartCoroutine(WaitForBurger());
                    
                }
                break;

            case NPCState.Waiting:
                    Debug.Log("I WAIT BURGER MASTER");
                    //WaitForBurger();
                break;

            case NPCState.Exiting:
                if (Vector3.Distance(transform.position, exitPoint.position) < 2f)
                {
                    Destroy(gameObject); // Remove NPC once they reach the exit
                    Debug.Log($"{gameObject.name} has exited the store.");
                }
                break;
        }
    }

    IEnumerator WaitForBurger()
    {
        float elapsedTime = 0f;
        while (!hasBurger && elapsedTime < waitTimeAtCounter)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (hasBurger)
        {
            Debug.Log("Yippi i goot burgy master");
            ExitStore();
        }
        else
        {
            Debug.Log($"{gameObject.name} is leaving because no burger was given.");
            ExitStore();
        }
    }

    public void GiveBurger()
    {
        if(currentState == NPCState.Waiting)
        {
            hasBurger = true;
            Debug.Log($"{gameObject.name} received a burger and is leaving.");
            ExitStore();
        }
        
    }

    private void ExitStore()
    {
        currentState = NPCState.Exiting;
        //Vector2 dest = new Vector2(exitPoint.position.x,exitPoint.position.z);
        agent.SetDestination(exitPoint.position);
        Debug.Log($"{gameObject.name} is heading to the exit.");
    }
}
