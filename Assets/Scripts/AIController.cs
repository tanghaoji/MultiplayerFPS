using UnityEngine;
using System.Collections;

/// <summary>
/// A class to control the bot's behaviour
/// </summary>
public class AIController : MonoBehaviour {

    public NavMeshAgent nma;

    private GameObject[] targets;

    void Update()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
        if (targets.Length > 0)
        {
            setTarget();
        }
        
    }

    public void setTarget()
    {
        if (targets.Length < 1) return;
        int targetIdx = Random.Range(0, targets.Length);
        nma.SetDestination(targets[targetIdx].transform.position);
    }

}
