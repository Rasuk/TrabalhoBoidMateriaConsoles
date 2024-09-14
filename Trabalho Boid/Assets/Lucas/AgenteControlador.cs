using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class AgenteControlador : MonoBehaviour
{
    
    public GameObject agentePrefab;
    public List<GameObject> agentesPrefabs = new List<GameObject>();
    public int agenteCount = 100;
    public Vector3 spawnArea = new Vector3(100, 0, 100);


    public ComputeShader agenteComputeShader;

    private ComputeBuffer positionBuffer;
    private ComputeBuffer velocityBuffer;

    private Vector3[] boidPositions;
    private Vector3[] boidVelocities;

    public Transform controlador; 

    public float neighborRadius;
    public float separationWeight;
    public float alignmentWeight;
    public float cohesionWeight;
    public float followWeight;
    public float maxSpeed;
    public float maxForce;
    
    void Start()
    {
        Profiler.BeginSample("Medição tempo Metodo Start");
        boidPositions = new Vector3[agenteCount];
        boidVelocities = new Vector3[agenteCount];
        for (int i = 0; i < agenteCount; i++)
        {
           
            Vector3 position = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),0,
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2));
            boidPositions[i] = position;
            boidVelocities[i] = new Vector3(Random.Range(-1f, 1f),0,Random.Range(-1f, 1f));
           GameObject boid =  Instantiate(agentePrefab, position, Quaternion.identity);
           boid.GetComponent<Agente>().controlador = controlador;
           agentesPrefabs.Add(boid);

        }
        positionBuffer = new ComputeBuffer(agenteCount, sizeof(float) * 3);
        velocityBuffer = new ComputeBuffer(agenteCount, sizeof(float) * 3);
        positionBuffer.SetData(boidPositions);
        velocityBuffer.SetData(boidVelocities);
        Profiler.EndSample();
    }

    void Update()
    {
        Profiler.BeginSample("Medição Update");
        if (agenteComputeShader != null)
        {

           
            agenteComputeShader.SetFloat("neighborRadius", neighborRadius);
            agenteComputeShader.SetFloat("separationWeight", separationWeight);
            agenteComputeShader.SetFloat("alignmentWeight", alignmentWeight);
            agenteComputeShader.SetFloat("cohesionWeight", cohesionWeight);
            agenteComputeShader.SetFloat("followWeight", followWeight);
            agenteComputeShader.SetFloat("maxSpeed", maxSpeed);
            agenteComputeShader.SetFloat("maxForce", maxForce);
            agenteComputeShader.SetVector("controladorPos", controlador.position);
            agenteComputeShader.SetBuffer(0, "positions", positionBuffer);
            agenteComputeShader.SetBuffer(0, "velocities", velocityBuffer);
            agenteComputeShader.SetInt("boidCount", agenteCount); 
            int threadGroups = Mathf.CeilToInt((float)agenteCount / 256f);
            agenteComputeShader.Dispatch(0, threadGroups, 1, 1);
            positionBuffer.GetData(boidPositions);
            velocityBuffer.GetData(boidVelocities);
            for (int i = 0; i < agenteCount; i++)
            {
                agentesPrefabs[i].GetComponent<Transform>().position = boidPositions[i];
                
            }

        }
        else
        {
            for (int i = 0; i < agenteCount; i++)
            {
                agentesPrefabs[i].GetComponent<Agente>().cohesionWeight= cohesionWeight;
                agentesPrefabs[i].GetComponent<Agente>().separationWeight= separationWeight;
                agentesPrefabs[i].GetComponent<Agente>().alignmentWeight= alignmentWeight;
                agentesPrefabs[i].GetComponent<Agente>().maxForce= maxForce;
                agentesPrefabs[i].GetComponent<Agente>().neighborRadius= neighborRadius;
                agentesPrefabs[i].GetComponent<Agente>().maxSpeed= maxSpeed;

            }
        }
        Profiler.EndSample();
        
    }


}
