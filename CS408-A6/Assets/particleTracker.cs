using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleTracker : MonoBehaviour
{
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }



    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        Rigidbody rb = other.GetComponent<Rigidbody>();
        int i = 0;


        while (i < numCollisionEvents)
        {
            //Debug.Log("in");
            if (rb)
            {
                Vector3 pos = collisionEvents[i].intersection;
                FindObjectOfType<terrainManager>().hit(pos);
            }
            i++;
        }
    }
}
