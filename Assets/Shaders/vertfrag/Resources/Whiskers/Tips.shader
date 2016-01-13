Shader "Custom/Tips" {

    Properties {


    _Hand1( "Hand Position 1" , Vector ) = ( .1 , .4 , .4 )
    _Hand2( "Hand Position 2" , Vector ) = ( .1 , .4 , .4 )


    }

    SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Cull off
        Pass{

           // Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
 
            CGPROGRAM
            #pragma target 5.0
 
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 

            struct Vert {

						    float3 pos;
						    float3 vel;
						    float3 nor;
						    float2 uv;
						    float  ribbonID;
						    float  life; 
						    float3 debug;

						};

            StructuredBuffer<Vert> buf_Points;

            uniform float3 _Hand1;
            uniform float3 _Hand2;
            uniform int _RibbonWidth;
 
            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos : SV_POSITION;
                float3 nor : TEXCOORD1;
                float3 debug : TEXCOORD0;
            };

            uint getID( uint id  ){

            	return (floor( id / 6) * 8) + 7;

            }
 
            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                uint fID = getID( id );
                Vert v = buf_Points[fID];
                Vert d = buf_Points[fID-1];
                Vert d2 = buf_Points[fID-2];
                uint triID = id % 6;


                float3 worldPos = v.pos;
                float3 worldNor = normalize(v.pos - d.pos);
                float3 worldNor2 = normalize(d.pos - d2.pos);
                float3 xVec = normalize( cross( worldNor , float3( 0 , 1 , 0 )) );
                float3 yVec = normalize( cross( xVec , worldNor) );
                float size = .005;

                xVec *= size;
                yVec *= size;

                if( triID == 0  || triID == 3 ){
                	worldPos += -xVec - yVec;
                }else if( triID == 2 || triID == 4 ){
                	worldPos += xVec + yVec;

                }else if( triID == 1 ){
                	worldPos += xVec - yVec;
                	
                }else if( triID == 5 ){
                	worldPos += -xVec  + yVec;
                	
                }

                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
                o.nor = worldNor;
                o.debug =  float3( 1.0 , 1.0 , 1.0 );//nor * .5 + .5;//float3(float(fID)/32768., v.uv.x , v.uv.y);
                return o;
            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings i) : COLOR {
                float3 col = i.nor * .5 + .5;
                return float4( col , 1.);//.1 * float4(1,0.5f,0.0f,1.0) / ( i.dToPoint * i.dToPoint  * i.dToPoint );
            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}
