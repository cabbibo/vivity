using UnityEngine;
 
//This game object invokes PlaneComputeShader (when attached via drag'n drop in the editor) using the PlaneBufferShader (also attached in the editor)
//to display a grid of points moving back and forth along the z axis.
public class Tube : MonoBehaviour
{
    public Shader shader;
    public ComputeShader computeShader;

    private ComputeBuffer _vertBuffer;
    private ComputeBuffer _ogBuffer;
    private ComputeBuffer _pointsBuffer;

    public const int threadX = 4;
    public const int threadY = 4;
    public const int threadZ = 4;

    public const int strideX = 4;
    public const int strideY = 4;
    public const int strideZ = 4;

    public int ribbonWidth = 64;
    private int ribbonLength;

    public GameObject[] Points;


    public GameObject handL;
    public GameObject handR;
    public GameObject audioObj;

    public float pointSize;
    public float tipSize;
    public float baseSize;


    private float[] pointValues;
    private int pointsLength;
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



    private int totalRibbonLength { get { return (int)Mathf.Floor( (float)vertexCount / ribbonWidth ); } }

    private int _kernel;
    private Material material;

    private Vector3 p1;
    private Vector3 p2;

    private Matrix4x4 m;

 
    //We initialize the buffers and the material used to draw.
    void Start ()
    {
        //print( vertexCount );
        //print( totalRibbonLength );
        //print( ribbonWidth* (totalRibbonLength-1) );

        pointsLength = Points.Length;
        pointValues = new float[16 * pointsLength];
      
        ribbonLength = totalRibbonLength;


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
      material.SetInt("_RibbonWidth" , ribbonWidth);
      int numVertsTotal = (ribbonWidth-1) * 3 * 2 * (totalRibbonLength-1);

      Graphics.DrawProcedural(MeshTopology.Triangles, numVertsTotal);


    }

    private void createBuffers() {

      _vertBuffer     = new ComputeBuffer( vertexCount ,  VERT_SIZE * sizeof(float));
      _ogBuffer       = new ComputeBuffer( vertexCount ,  3 * sizeof(float));
      _pointsBuffer   = new ComputeBuffer( pointsLength ,  16 * sizeof(float));

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

            float uvX = (float)(id % ribbonWidth ) / ribbonWidth;
            float uvY = Mathf.Floor( (float)(id / ribbonWidth)) / totalRibbonLength;
            
           // if( uvX == 0 ){ print( uvY ); }

            float u = uvX * 2.0f * Mathf.PI;
            float v = uvY * 2.0f * Mathf.PI;

            float xV = Random.Range(-.1f, .1f);
            float zV = Random.Range(-.1f, .1f);
            float yV = Random.Range(-.1f, .1f);
            yV += 1.25f;

            // /xV  = uvX / 10;
            // /yV  = 1;
            // /zV  =( uvY -.5f)* 10;

                //pos
            ogValues[indexOG++] = xV;
            ogValues[indexOG++] = yV;
            ogValues[indexOG++] = zV;



            //pos
            // need to be slightly different to not get infinte forces
            inValues[index++] = xV * 1.1f;
            inValues[index++] = yV * 1.1f;
            inValues[index++] = zV * 1.1f;
           
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

      material = new Material( shader );

    }
 
    //Remember to release buffers and destroy the material when play has been stopped.
    void ReleaseBuffer(){

      _vertBuffer.Release(); 
      _ogBuffer.Release(); 
      DestroyImmediate( material );

    }


    private void Dispatch() {

      computeShader.SetVector("_HandL", handL.transform.position);
      computeShader.SetVector("_HandR", handR.transform.position);
      //int L = handL.GetComponent<controllerInfo>().triggerDown;
      //int R = handR.GetComponent<controllerInfo>().triggerDown;
      //computeShader.SetInt( "_TriggerL", L );
      //computeShader.SetInt( "_TriggerR", R );
      //computeShader.SetFloat( "_Drawing" , drawing );

      computeShader.SetFloat( "_DeltaTime"    , Time.deltaTime );
      computeShader.SetFloat( "_Time"         , Time.time      );

      computeShader.SetFloat( "_PointSize"    , pointSize    );
      computeShader.SetFloat( "_TipSize"      , tipSize      );
      computeShader.SetFloat( "_BaseSize"     , baseSize     );


      computeShader.SetInt( "_RibbonWidth"   , ribbonWidth   );
      computeShader.SetInt( "_RibbonLength"   , ribbonLength  );
      computeShader.SetInt( "_PointsLength"  , pointsLength  );

      //Texture2D audioTexture = audioObj.GetComponent<audioSourceTexture>().AudioTexture;

      ///computeShader.SetTexture(_kernel,"_Audio", audioTexture);

      for( int i = 0; i < pointsLength; i++ ){
        m = Points[i].transform.localToWorldMatrix;
        for( int j = 0; j < 16; j++ ){
          int x = j % 4;
          int y = (int) Mathf.Floor(j / 4);
          pointValues[ j + 16 * i ] = m[x,y];
        }
      }


      _pointsBuffer.SetData(pointValues);



      computeShader.SetBuffer(_kernel, "pointsBuffer", _pointsBuffer );
      computeShader.SetBuffer(_kernel, "vertBuffer", _vertBuffer);
      computeShader.SetBuffer(_kernel, "ogBuffer", _ogBuffer);

      computeShader.Dispatch(_kernel, strideX , strideY , strideZ );

    }

}