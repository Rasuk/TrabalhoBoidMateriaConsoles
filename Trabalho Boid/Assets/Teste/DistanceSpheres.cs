using UnityEngine;

public class DistanceSpheres : MonoBehaviour
{
    public ComputeShader computeShader;
    public GameObject spherePrefab; 
    public int sphereCount = 100;

    private ComputeBuffer sphereBuffer;
    private SphereData[] spheres;
    private GameObject[] sphereObjects;
    private Vector3 PosicaoInicial;
   

    struct SphereData
    {
        public Vector3 position;
        public float distance;
        public Color color;
    }
    public float DeltaTime;
    private int kernelHandle; 
    void Start()
    {
       
        kernelHandle = computeShader.FindKernel("CSMain");
        PosicaoInicial = new Vector3(0.0f, 0.0f, 0.0f);
        spheres = new SphereData[sphereCount];
        sphereObjects = new GameObject[sphereCount];

        for (int i = 0; i < sphereCount; i++)
        {
        
            spheres[i].position = new Vector3(
                Random.Range(-10.0f, 10.0f),
                Random.Range(-10.0f, 10.0f),
                Random.Range(-10.0f, 10.0f)
            );
            
         
            spheres[i].distance = 0.0f;
            spheres[i].color = Color.white;

         
            sphereObjects[i] = Instantiate(spherePrefab, new Vector3(PosicaoInicial.x+5*i,0,0), Quaternion.identity);
            sphereObjects[i].GetComponent<Renderer>().material.color = spheres[i].color;
        }

       
        sphereBuffer = new ComputeBuffer(sphereCount, sizeof(float) * 8); 
        sphereBuffer.SetData(spheres);
        computeShader.SetBuffer(kernelHandle, "sphereBuffer", sphereBuffer);
    }

    void Update()
    {
        DeltaTime = Time.deltaTime;
        computeShader.SetFloat("deltaTime", DeltaTime);
        computeShader.Dispatch(kernelHandle, sphereCount, 1, 1);

     
        sphereBuffer.GetData(spheres);

        
        for (int i = 0; i < sphereCount; i++)
        {
            sphereObjects[i].transform.position = spheres[i].position;
            sphereObjects[i].GetComponent<Renderer>().material.color = spheres[i].color;
        }
        Debug.Log(spheres[0].position.x);
    }

    void OnDestroy()
    {
     
        if (sphereBuffer != null) sphereBuffer.Release();
    }
}
