using UnityEngine;


/*

  
  TODO:
    Whisker compute shader
    Whisker shader making sure ids r proper

*/

public class Whiskers : MonoBehaviour
{
    public Shader stalkShader;
    public Shader tipShader;
    public ComputeShader computeShader;

    public GameObject Select3D;

    private ComputeBuffer _vertBuffer;
    private ComputeBuffer _ogBuffer;

    public const int threadX = 8;
    public const int threadY = 8;
    public const int threadZ = 8;

    public const int strideX = 12;
    public const int strideY = 12;
    public const int strideZ = 12;

    public GameObject handL;
    public GameObject handR;
    //public GameObject audioObj;
    /*
        
        float3 pos 
        float3 vel
        float3 nor
        float2 uv
        float  ribbonID
        float  life 
        float3 debug

    */

    public const int VERT_SIZE = 16;


    private int gridX { get { return threadX * strideX; } }
    private int gridY { get { return threadY * strideY; } }
    private int gridZ { get { return threadZ * strideZ; } }

    private int vertexCount { get { return gridX * gridY * gridZ; } }



    //private int totalRibbonLength { get { return (int)Mathf.Floor( (float)vertexCount / ribbonWidth ); } }

    private int _kernel;
    private Material stalkMat;
    private Material tipMat;

    private Vector3 p1;
    private Vector3 p2;

    private int vertsWidth;
    private int vertsHeight;

 
    //We initialize the buffers and the material used to draw.
    void Start ()
    {
        print( vertexCount );
        print(Mathf.Pow( vertexCount / 8.0f , .5f ));
        vertsWidth  = (int)Mathf.Pow( vertexCount / 8.0f , .5f );
        vertsHeight = (int)Mathf.Pow( vertexCount / 8.0f , .5f );
        print( vertsWidth );


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
      
      stalkMat.SetPass(0);
      stalkMat.SetBuffer("buf_Points", _vertBuffer);
      int numVertsTotal = 7 * 2 * vertsWidth * vertsHeight;
      Graphics.DrawProcedural(MeshTopology.Lines, numVertsTotal);

      tipMat.SetPass(0);
      tipMat.SetBuffer("buf_Points", _vertBuffer);
      numVertsTotal = vertsWidth * vertsHeight;
      Graphics.DrawProcedural(MeshTopology.Triangles, 6 * numVertsTotal);

      //print("ss");


    }

    private void createBuffers() {

      _vertBuffer = new ComputeBuffer( vertexCount ,  VERT_SIZE * sizeof(float));
      _ogBuffer = new ComputeBuffer( vertexCount ,  3 * sizeof(float));

      float[] inValues = new float[VERT_SIZE * vertexCount];
      float[] ogValues = new float[3 * vertexCount];

      int index = 0;
      int indexOG = 0;

      float a = .3f;
      float c = .8f;

      for (int z = 0; z < gridZ; z++) {
        for (int y = 0; y < gridY; y++) {
          for (int x = 0; x < gridX; x++) {

            int id = x + y * gridX + z * gridX * gridY; 

            int stalkID = id / 8;
            int tipID = id % 8;

            float uvX = (float)(stalkID % vertsWidth ) / vertsWidth;
            float uvY = Mathf.Floor( (float)(stalkID / vertsWidth)) / vertsWidth;
            
           // if( uvX == 0 ){ print( uvY ); }

            float u = uvY * 2.0f * Mathf.PI;
            float v = uvX * 2.0f * Mathf.PI;

            float xV = 1.2f * uvX;
            float zV = 1.2f * uvY;
            float yV = (float)tipID / 8 + 1.0f;

            // /xV  = uvX / 10;
            // /yV  = 1;
            // /zV  =( uvY -.5f)* 10;

                //pos
            ogValues[indexOG++] = xV;
            ogValues[indexOG++] = yV;
            ogValues[indexOG++] = zV;



            //pos
            // need to be slightly different to not get infinte forces
            inValues[index++] = xV;
            inValues[index++] = yV;
            inValues[index++] = zV;
           
            //vel
            inValues[index++] = Random.Range(-.01f , .01f );
            inValues[index++] = Random.Range(-.01f , .01f );
            inValues[index++] = Random.Range(-.01f , .01f );

            //nor
            inValues[index++] = 0f;
            inValues[index++] = 1f;
            inValues[index++] = 0f;

            //uv
            inValues[index++] = uvX;
            inValues[index++] = uvY;

            //ribbon id
            inValues[index++] = 0f;

            //life
            inValues[index++] = -1f;

            //debug
            inValues[index++] = 1.0f;
            inValues[index++] = 1.0f;
            inValues[index++] = 1.0f;

          }
        }
      }

      _vertBuffer.SetData(inValues);
      _ogBuffer.SetData(ogValues);

    }
 
    //For some reason I made this method to create a material from the attached shader.
    private void createMaterial(){

      stalkMat = new Material( stalkShader );
      tipMat = new Material( tipShader );

    }
 
    //Remember to release buffers and destroy the material when play has been stopped.
    void ReleaseBuffer(){

      _vertBuffer.Release(); 
      _ogBuffer.Release(); 
      DestroyImmediate( tipMat );
      DestroyImmediate( stalkMat );

    }


    private void Dispatch() {

      
      computeShader.SetVector("_HandL", handL.transform.position);
      computeShader.SetVector("_HandR", handR.transform.position);
      int L = handL.GetComponent<controllerInfo>().triggerDown;
      int R = handR.GetComponent<controllerInfo>().triggerDown;

      Vector3 val = Select3D.GetComponent<Select3D>().Value;
      computeShader.SetVector("_Interface" , val );
    
      //computeShader.SetInt( "_TriggerL", L );
      //computeShader.SetInt( "_TriggerR", R );
      /*computeShader.SetFloat( "_Drawing" , drawing );

      computeShader.SetFloat( "_DeltaTime"    , Time.deltaTime );
      computeShader.SetFloat( "_Time"         , Time.time      );
      computeShader.SetInt( "_RibbonWidth"  , ribbonWidth    );

      Texture2D audioTexture = audioObj.GetComponent<audioSourceTexture>().AudioTexture;

      computeShader.SetTexture(_kernel,"_Audio", audioTexture);
      */

      computeShader.SetBuffer(_kernel, "vertBuffer", _vertBuffer);
      computeShader.SetBuffer(_kernel, "ogBuffer", _ogBuffer);

      computeShader.Dispatch(_kernel, strideX , strideY , strideZ );

    }

}