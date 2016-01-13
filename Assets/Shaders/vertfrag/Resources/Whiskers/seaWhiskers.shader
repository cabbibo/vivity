Shader "Custom/seaWhiskers" {

    Properties {


    _Hand1( "Hand Position 1" , Vector ) = ( .1 , .4 , .4 )
    _Hand2( "Hand Position 2" , Vector ) = ( .1 , .4 , .4 )


    }

    SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Cull off
        Pass{

            //Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
 
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

						struct Pos{
							float3 pos;
						};


            StructuredBuffer<Vert> buf_Points;
            StructuredBuffer<Pos> og_Points;

            uniform float3 _Hand1;
            uniform float3 _Hand2;
            uniform int _RibbonWidth;
            uniform float4x4 _World;
 
            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos : SV_POSITION;
                float3 nor : TEXCOORD1;
                float3 debug : TEXCOORD0;
            };



            uint weirdGeogetID( uint id  ){

                int base = floor(id /7);
                int stalkVal = id - base*2;

                return base * 8 + floor(stalkVal/ 2 ) + stalkVal % 2;

            }

            uint getID( uint id  ){

                int base = floor(id /14);
                int stalkVal = id - base * 14;

                return base * 8 + floor(stalkVal/ 2 ) + stalkVal % 2;

                //return id;

            }
 
            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                uint fID = getID( id );


                Vert v = buf_Points[fID];
                Vert u = buf_Points[fID+1];
                Vert d = buf_Points[fID-1];

                Pos og = og_Points[fID];

                float3 ogPos = mul(_World , float4( og.pos , 1.));

                float3 worldPos = v.pos;
       

                uint wID = fID % 8;
                float map = (float(wID) ) / 8;

                float3 norm;

                if( wID == 0 ){
                    norm = normalize(u.pos - v.pos);
                }else if( wID == 7 ){
                    norm = normalize(v.pos - d.pos);
                }else{

                    norm = normalize( (v.pos - d.pos) + ( u.pos - v.pos ) );
                }


                o.nor = norm;

                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
                o.debug =  float3(v.debug.x , v.debug.x , v.debug.x );//float3( map , id% 2 , 1.0 );//float3(float(fID)/32768., v.uv.x , v.uv.y);
                o.debug = float3( map , id% 2 , 1.0 );//float3(float(fID)/32768., v.uv.x , v.uv.y);
                o.debug = ogPos - v.pos;
                return o;
            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings i) : COLOR {
                float3 col = i.nor * .5 + .5;
                return float4( col  * length( i.debug ) * 1., 1.);//.1 * float4(1,0.5f,0.0f,1.0) / ( i.dToPoint * i.dToPoint  * i.dToPoint );
            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}
