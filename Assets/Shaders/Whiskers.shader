Shader "Custom/Whiskers" {

    Properties {


    _Hand1( "Hand Position 1" , Vector ) = ( .1 , .4 , .4 )
    _Hand2( "Hand Position 2" , Vector ) = ( .1 , .4 , .4 )


    }

    SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Cull off
        Pass{

            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
 
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
                float3 debug : TEXCOORD0;
            };

            uint getID( uint id  ){



            }
 
            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                fID = getID( id );
                Vert v = buf_Points[fID];

                float3 worldPos = v.pos;

                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
                o.debug =  nor * .5 + .5;//float3(float(fID)/32768., v.uv.x , v.uv.y);
                return o;
            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings i) : COLOR {
                return float4( i.debug.x , i.debug.y , i.debug.z , 1.);//.1 * float4(1,0.5f,0.0f,1.0) / ( i.dToPoint * i.dToPoint  * i.dToPoint );
            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}
