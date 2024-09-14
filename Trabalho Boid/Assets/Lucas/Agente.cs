using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEngine.GraphicsBuffer;

public class Agente : MonoBehaviour
{
    public Vector3 velocity;
    public float maxSpeed = 2f;
    public float maxForce = 0.5f;

    public float neighborRadius = 1.5f;  
    public float separationWeight = 2f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;
    


    public Transform controlador;
    public float followWeight = 0.1f;
    void Start()
    {
        velocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        velocity = velocity.normalized * maxSpeed;
    }

    
    void Update()
    {
        Profiler.BeginSample("Medição Update Agente");
        Agente[] Agentes = FindObjectsOfType<Agente>();
        Vector3 separation = Separation(Agentes) * separationWeight;
        Vector3 alignment = Alignment(Agentes) * alignmentWeight;
        Vector3 cohesion = Cohesion(Agentes) * cohesionWeight;
        Vector3 follow = FollowControlador() * followWeight;
        velocity += separation + alignment + cohesion + follow;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;
        
        Profiler.EndSample();
    }


    Vector3 Separation(Agente[] Agentes)
    {
        Vector3 separationForce = Vector3.zero;
        int count = 0;

        foreach (Agente otherAgente in Agentes)
        {
            float distance = Vector3.Distance(transform.position, otherAgente.transform.position);
            if (otherAgente != this && distance < neighborRadius)
            {
               
                Vector3 away = transform.position - otherAgente.transform.position;
                separationForce += away.normalized / distance;  
                count++;
            }
        }

        if (count > 0)
        {
            separationForce /= count;  
        }

        return separationForce;
    }

   
    Vector3 Alignment(Agente[] Agentes)
    {
        Vector3 alignmentForce = Vector3.zero;
        int count = 0;

        foreach (Agente otherAgente in Agentes)
        {
            float distance = Vector3.Distance(transform.position, otherAgente.transform.position);
            if (otherAgente != this && distance < neighborRadius)
            {
                alignmentForce += otherAgente.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            alignmentForce /= count;
            alignmentForce = alignmentForce.normalized * maxSpeed; 
            alignmentForce -= velocity; 
            alignmentForce = Vector3.ClampMagnitude(alignmentForce, maxForce); 
        }

        return alignmentForce;
    }


    Vector3 Cohesion(Agente[] Agentes)
    {
        Vector3 cohesionForce = Vector3.zero;
        int count = 0;

        foreach (Agente otherAgente in Agentes)
        {
            float distance = Vector3.Distance(transform.position, otherAgente.transform.position);
            if (otherAgente != this && distance < neighborRadius)
            {
                cohesionForce += otherAgente.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            cohesionForce /= count;
            cohesionForce = (cohesionForce - transform.position).normalized * maxSpeed;
            cohesionForce -= velocity;
            cohesionForce = Vector3.ClampMagnitude(cohesionForce, maxForce);
        }

        return cohesionForce;
    }
    Vector3 FollowControlador()
    {
        Vector3 controladorPos = controlador.position;
        Vector3 toControlador = (controladorPos - transform.position).normalized * maxSpeed;
        Vector3 followForce = toControlador - velocity;
        followForce = Vector3.ClampMagnitude(followForce, maxForce);
        return followForce;
    }

}