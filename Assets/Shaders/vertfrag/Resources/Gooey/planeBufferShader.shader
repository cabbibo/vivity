Shader "DX11/PlaneBufferShader"
{

    Properties {


    _Hand1( "Hand Position 1" , Vector ) = ( .1 , .4 , .4 )
    _Hand2( "Hand Position 2" , Vector ) = ( .1 , .4 , .4 )


    }

    SubShader{
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
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
            };

            StructuredBuffer<Vert> buf_Points;




            uniform float3 _Hand1;
            uniform float3 _Hand2;
      
 
            //A simple input struct for our pixel shader step containing a position.
            struct ps_input {
                float4 pos : SV_POSITION;
                float dToPoint : TEXCOORD0;
            };
 
            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            ps_input vert (uint id : SV_VertexID){

                ps_input o;
                float3 worldPos = buf_Points[id].pos;
                float3 d1 = worldPos - _Hand1;
                float3 d2 = worldPos - _Hand2;

                o.dToPoint = min( length( d1 ) , length( d2 ) );
                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
                return o;
            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (ps_input i) : COLOR {
                return float4( 1. , 1. , 1. , 1.);//.1 * float4(1,0.5f,0.0f,1.0) / ( i.dToPoint * i.dToPoint  * i.dToPoint );
            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
}