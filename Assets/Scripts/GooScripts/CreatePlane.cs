using UnityEngine;
 
//This game object invokes PlaneComputeShader (when attached via drag'n drop in the editor) using the PlaneBufferShader (also attached in the editor)
//to display a grid of points moving back and forth along the z axis.
public class CreatePlane : MonoBehaviour
{
    public Shader shader;
    public ComputeShader computeShader;

    private ComputeBuffer _vertBuffer;

    public const int threadX = 4;
    public const int threadY = 4;
    public const int threadZ = 4;

    public const int strideX = 32;
    public const int strideY = 32;
    public const int strideZ = 32;


    public GameObject hand1;
    public GameObject hand2;

    //pos vel
    public const int VERT_SIZE = 6;


    private int gridX { get { return threadX * strideX; } }
    private int gridY { get { return threadY * strideY; } }
    private int gridZ { get { return threadZ * strideZ; } }
    private int vertexCount { get { return gridX * gridY * gridZ; } }



    
    private int _kernel;
    private Material material;

    private Vector3 p1;
    private Vector3 p2;


 
    public const int VertCount = 16384; //32*32*4*4 (Groups*ThreadsPerGroup)
 
    //We initialize the buffers and the material used to draw.
    void Start ()
    {
        createBuffers();
        createMaterial();
        _kernel = computeShader.FindKernel("CSMain");
    }
 
    //When this GameObject is disabled we must release the buffers or else Unity complains.
    private void OnDisable(){
        ReleaseBuffer();
    }
 
    //After all rendering is complete we dispatch the compute shader and then set the material before drawing with DrawProcedural
    //this just draws the "mesh" as a set of points
    private void OnPostRender () {
      
        Dispatch();
        
        material.SetPass(0);
        material.SetBuffer("buf_Points", _vertBuffer);
        Graphics.DrawProcedural(MeshTopology.Points, VertCount);


    }

    private void createBuffers() {

      _vertBuffer = new ComputeBuffer(vertexCount,  VERT_SIZE * sizeof(float));

      float[] inValues = new float[VERT_SIZE * vertexCount];

      int index = 0;
      for (int z = 0; z < gridZ; z++) {
        for (int y = 0; y < gridY; y++) {
          for (int x = 0; x < gridX; x++) {

            //xyz
            inValues[index++] = (Random.value - .5f ) * 1f;
            inValues[index++] = (Random.value - .5f ) * 1f;
            inValues[index++] = (Random.value - .5f ) * 1f;
           
            //xyz
            inValues[index++] = (Random.value - .5f ) * .001f;
            inValues[index++] = (Random.value - .5f ) * .001f;
            inValues[index++] = (Random.value - .5f ) * .001f;

          }
        }
      }

      _vertBuffer.SetData(inValues);
    }
 
    //For some reason I made this method to create a material from the attached shader.
    private void createMaterial(){

      material = new Material(shader);

    }
 
    //Remember to release buffers and destroy the material when play has been stopped.
    void ReleaseBuffer(){

        _vertBuffer.Release(); 
        DestroyImmediate(material);

    }


    private void Dispatch() {

      computeShader.SetVector("_Hand1Pos", hand1.transform.position);
      computeShader.SetVector("_Hand2Pos", hand2.transform.position);

      computeShader.SetFloat( "_DeltaTime"    , Time.deltaTime );
      computeShader.SetFloat( "_Time"         , Time.time      );

      computeShader.SetBuffer(_kernel, "vertBuffer", _vertBuffer);

      computeShader.Dispatch(_kernel, strideX , strideY , strideZ );

    }
}