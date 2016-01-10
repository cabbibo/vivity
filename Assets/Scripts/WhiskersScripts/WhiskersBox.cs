using UnityEngine;


/*

  
  TODO:
    Whisker compute shader
    Whisker shader making sure ids r proper

*/

public class WhiskersBox : MonoBehaviour
{
    public Shader stalkShader;
    public Shader tipShader;
    public Shader baseShader;
    public ComputeShader computeShader;

    public GameObject Select3D;
    public GameObject mainObject;

    private ComputeBuffer _vertBuffer;
    private ComputeBuffer _ogBuffer;
    private ComputeBuffer _transBuffer;


    public int threadSize = 6;
    public int strideSize = 4;

    public int threadX { get { return threadSize; } }
    public int threadY { get { return threadSize; } }
    public int threadZ { get { return threadSize; } }
    public int strideX { get { return strideSize; } }
    public int strideY { get { return strideSize; } }
    public int strideZ { get { return strideSize; } }
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

    private float[] transValues = new float[32];



    //private int totalRibbonLength { get { return (int)Mathf.Floor( (float)vertexCount / ribbonWidth ); } }

    private int _kernel;
    private Material stalkMat;
    private Material tipMat;
    private Material baseMat;

    private Vector3 p1;
    private Vector3 p2;

    private int vertsWidth;
    private int vertsHeight;

 
    //We initialize the buffers and the material used to draw.
    void Start ()
    {
//        print( vertexCount );
//        print(Mathf.Pow( vertexCount / 8.0f , .5f ));
        vertsWidth  = (int)Mathf.Pow( vertexCount / ( 8.0f * 6.0f ), .5f );
        vertsHeight = (int)Mathf.Pow( vertexCount / ( 8.0f * 6.0f ), .5f );
//        print( vertsWidth );
         mainObject.GetComponent<Renderer>().enabled = false;


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
     // print("ss");
     
      Dispatch();

      int numVertsTotal;
      
      
      stalkMat.SetPass(0);
      stalkMat.SetBuffer("buf_Points", _vertBuffer);
      numVertsTotal = 7 * 2 * vertsWidth * vertsHeight * 6;
      Graphics.DrawProcedural(MeshTopology.Lines, numVertsTotal);

      tipMat.SetPass(0);
      tipMat.SetBuffer("buf_Points", _vertBuffer);
      numVertsTotal = vertsWidth * vertsHeight * 6;
      Graphics.DrawProcedural(MeshTopology.Triangles, 6 * numVertsTotal);

      baseMat.SetPass(0);
      baseMat.SetBuffer("buf_Points", _vertBuffer);
      baseMat.SetInt( "_SideWidth" , vertsWidth );
      numVertsTotal = vertsWidth * vertsHeight * 6;
      Graphics.DrawProcedural(MeshTopology.Triangles, 6 * numVertsTotal);

      //print("ss");


    }

    private Vector3 createFinalVec( float xV , float yV , float zV ){

      float nX = Mathf.Sin( xV * 20.0f + zV * 3.0f ) + Mathf.Sin( xV * 2.0f + zV * 30.0f + Mathf.Sin( zV * 200.0f) * 20.0f ) + Mathf.Sin( xV * 20.0f + zV * 300.0f );
      float nZ = Mathf.Sin( xV * 4.0f + zV * 15.0f ) + Mathf.Sin( xV * 20.0f + zV * 10.0f + Mathf.Sin( xV * 100.0f) * 10.0f ) + Mathf.Sin( xV * 3.0f + zV * 30.0f );

      Vector2 uv = new Vector2( xV , zV );
      float l = Mathf.Pow( uv.magnitude , 2.0f );
      nX *= .002f;
      nZ *= .002f;
      Vector3 vec  = new Vector3( xV * .95f + nX , yV + nX + nZ + (.5f - l) * 0.0000f , zV * .95f + nZ );
      return vec;

    }

    private Vector3 createFinalNormal( float xV , float yV , float zV ){
      float eps = 0.1f  / (float)vertsWidth;
      Vector3 up    = createFinalVec(  xV , yV  , zV + eps );
      Vector3 down  = createFinalVec(  xV , yV  , zV - eps );
      Vector3 left  = createFinalVec(  xV + eps , yV , zV  );
      Vector3 right = createFinalVec(  xV - eps , yV , zV  );

      Vector3 norm = new Vector3();
      norm = Vector3.Cross( up - down , left - right );
      norm.Normalize();

      return norm;


    }

    private void createBuffers() {

      _vertBuffer = new ComputeBuffer( vertexCount ,  VERT_SIZE * sizeof(float));
      _ogBuffer = new ComputeBuffer( vertexCount ,  3 * sizeof(float));
      _transBuffer = new ComputeBuffer( 32 ,  sizeof(float));

      float[] inValues = new float[VERT_SIZE * vertexCount];
      float[] ogValues = new float[3 * vertexCount];

      int index = 0;
      int indexOG = 0;

      float a = .3f;
      float c = .8f;

      Vector3[] vecs = new [] {
        new Vector3(  1 ,  0 ,  0 ),
        new Vector3( -1 ,  0 ,  0 ),
        new Vector3(  0 ,  1 ,  0 ),
        new Vector3(  0 , -1 ,  0 ),
        new Vector3(  0 ,  0 ,  1 ),
        new Vector3(  0 ,  0 , -1 )
      };



      for (int z = 0; z < gridZ; z++) {
        for (int y = 0; y < gridY; y++) {
          for (int x = 0; x < gridX; x++) {

            int id = x + y * gridX + z * gridX * gridY; 

            int stalkID = id / 8;
            int faceID = (int)Mathf.Floor( stalkID / (vertsWidth * vertsHeight) );
           // if( (float)faceID == (float)stalkID / (vertsWidth * vertsHeight)){ print(faceID); print("WOW"); }

            stalkID = stalkID - faceID * ( vertsWidth * vertsHeight );
           

            int tipID = id % 8;

            float uvX = ((float)(stalkID % vertsWidth ) + .5f) / vertsWidth;
            float uvY = (Mathf.Floor( (float)(stalkID / vertsWidth)) + .5f) / vertsWidth;
            
           // if( uvX == 0 ){ print( uvY ); }

            float u = uvY * 2.0f * Mathf.PI;
            float v = uvX * 2.0f * Mathf.PI;

            float xV = (1.0f * uvX) - 0.5f;
            float zV = (1.0f * uvY) - 0.5f;
            float yV = ((float)tipID / 8) + .5f;

            Vector3 vec  = createFinalVec( xV , yV , zV );

            Vector3 rVec, uVec, fVec; // right up forward
            Vector3 upVec = createFinalNormal( xV , yV , zV ) + new Vector3( 0 , 4 , 0 );
            upVec.Normalize();
            if( faceID <= 3 ){
              vec = Quaternion.AngleAxis(360 * faceID / 4, Vector3.forward) * vec;

              ogValues[indexOG++] = vec.x;
              ogValues[indexOG++] = vec.y;
              ogValues[indexOG++] = vec.z;

              vec = mainObject.transform.localToWorldMatrix.MultiplyPoint( vec );

              upVec = Quaternion.AngleAxis(360 * faceID / 4, Vector3.forward) * upVec;

            }else if( faceID == 4 || faceID == 5 ){
              vec = Quaternion.AngleAxis( 360 * ((faceID-4) * 2 + 1 ) / 4  , Vector3.right ) * vec;

              ogValues[indexOG++] = vec.x;
              ogValues[indexOG++] = vec.y;
              ogValues[indexOG++] = vec.z;

              vec = mainObject.transform.localToWorldMatrix.MultiplyPoint( vec );
              upVec = Quaternion.AngleAxis( 360 * ((faceID-4) * 2 + 1 ) / 4  , Vector3.right ) * upVec;
            }



            // /xV  = uvX / 10;
            // /yV  = 1;
            // /zV  =( uvY -.5f)* 10;

           



            //pos
            // need to be slightly different to not get infinte forces
            inValues[index++] = vec.x * 1.01f;
            inValues[index++] = vec.y * 1.01f;
            inValues[index++] = vec.z * 1.01f;
           
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
            inValues[index++] = upVec.x;
            inValues[index++] = upVec.y;
            inValues[index++] = upVec.z;

          }
        }
      }

      _vertBuffer.SetData(inValues);
      _ogBuffer.SetData(ogValues);

    }
 
    //For some reason I made this method to create a material from the attached shader.
    private void createMaterial(){

      stalkMat = new Material( stalkShader );
      tipMat   = new Material( tipShader );
      baseMat  = new Material( baseShader );

    }
 
    //Remember to release buffers and destroy the material when play has been stopped.
    void ReleaseBuffer(){

      _vertBuffer.Release(); 
      _ogBuffer.Release(); 
      _transBuffer.Release(); 
      DestroyImmediate( tipMat );
      DestroyImmediate( stalkMat );
      DestroyImmediate( baseMat );

    }


    private void Dispatch() {

      
      computeShader.SetVector("_HandL", handL.transform.position);
      computeShader.SetVector("_HandR", handR.transform.position);
//      int L = handL.GetComponent<controllerInfo>().triggerDown;
//      int R = handR.GetComponent<controllerInfo>().triggerDown;

      computeShader.SetInt( "_ThreadSize" , threadSize );
      computeShader.SetInt( "_StrideSize" , strideSize );

      Vector3 val = Select3D.GetComponent<Select3D>().Value;
      computeShader.SetVector("_Interface" , val );

      Matrix4x4 m = mainObject.transform.localToWorldMatrix;

      for( int i = 0; i < 16; i++ ){
        int x = i % 4;
        int y = (int) Mathf.Floor(i / 4);
        transValues[i] = m[x,y];
      }

      m = mainObject.transform.worldToLocalMatrix;

      for( int i = 0; i < 16; i++ ){
        int x = i % 4;
        int y = (int) Mathf.Floor(i / 4);
        transValues[i+16] = m[x,y];
      }


      _transBuffer.SetData(transValues);

    
      //computeShader.SetInt( "_TriggerL", L );
      //computeShader.SetInt( "_TriggerR", R );
      /*computeShader.SetFloat( "_Drawing" , drawing );

      computeShader.SetFloat( "_DeltaTime"    , Time.deltaTime );
      computeShader.SetFloat( "_Time"         , Time.time      );
      computeShader.SetInt( "_RibbonWidth"  , ribbonWidth    );

      Texture2D audioTexture = audioObj.GetComponent<audioSourceTexture>().AudioTexture;

      computeShader.SetTexture(_kernel,"_Audio", audioTexture);
      */
      computeShader.SetBuffer(_kernel, "transBuffer" , _transBuffer);
      computeShader.SetBuffer(_kernel, "vertBuffer", _vertBuffer);
      computeShader.SetBuffer(_kernel, "ogBuffer", _ogBuffer);

      computeShader.Dispatch(_kernel, strideX , strideY , strideZ );

    }

}