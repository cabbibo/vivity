Shader "Custom/seaFloor" {

    Properties {


    _Hand1( "Hand Position 1" , Vector ) = ( .1 , .4 , .4 )
    _Hand2( "Hand Position 2" , Vector ) = ( .1 , .4 , .4 )


    }

    SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Cull Back
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
						struct Pos{
							float3 pos;
						};

            StructuredBuffer<Vert> buf_Points;
            StructuredBuffer<Pos> og_Points;

            uniform float3 _Hand1;
            uniform float3 _Hand2;
            uniform int _RibbonWidth;
            uniform sampler2D _Audio;
            uniform float4x4 _World;
 
            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos : SV_POSITION;
                float3 debug : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            uint3 getID( uint id  ){

            	uint base = floor( id / 6 );
            	uint tri  = id % 6;
            	uint row = floor( base / _RibbonWidth );
            	uint col = base % _RibbonWidth;

            	uint rDoID = row * _RibbonWidth;
            	uint rUpID = (row + 1) * _RibbonWidth;
            	uint cDoID = col;
            	uint cUpID = col + 1;

                  uint fID = 0;
                  uint tri1 = 0;
                  uint tri2 = 0;


            	if( tri == 0 ){
            		fID = rDoID + cDoID;
                    tri1 = rUpID + cDoID;
                    tri2 = rUpID + cUpID;
            	}else if( tri == 1 ){
            		fID = rUpID + cDoID;
                    tri1 = rUpID + cUpID;
                    tri2 = rDoID + cDoID;
            	}else if( tri == 2 ){
            		fID = rUpID + cUpID;
                    tri1 = rDoID + cDoID;
                    tri2 = rUpID + cDoID;
            	}else if( tri == 3 ){
            		fID = rDoID + cDoID;
                    tri1 = rUpID + cUpID;
                    tri2 = rDoID + cUpID;
            	}else if( tri == 4 ){
            		fID = rUpID + cUpID;
                    tri1 = rDoID + cUpID;
                    tri2 = rDoID + cDoID;
            	}else if( tri == 5 ){
            		fID = rDoID + cUpID;
                    tri1 = rDoID + cDoID;
                    tri2 = rUpID + cUpID;
            	}else{
            		fID = 0;
            	}

                //if( fID >=32768 ){ fID -= 32768; }
                //if( tri1 >=32768 ){ tri1 -= 32768; }
                //if( tri2 >=32768 ){ tri2 -= 32768; }
                return uint3( fID , tri1 , tri2 );

            }
 
            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                uint3 fID = getID( id );
                Vert v = buf_Points[fID.x * 8];
                Pos og = og_Points[fID.x * 8];

                float3 ogPos = mul(_World , float4( og.pos , 1.));
                float3 tri1 = buf_Points[fID.y * 8 ].pos;
                float3 tri2 = buf_Points[fID.z * 8].pos;

                float3 nor = normalize(cross( normalize(v.pos - tri1) , normalize(v.pos - tri2)));
                float3 worldPos = v.pos;
                o.uv = v.uv;

                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
                
                o.debug =  ogPos - v.pos; //nor * .5 + .5;//float3(float(fID)/32768., v.uv.x , v.uv.y);
                return o;
            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings i) : COLOR {

                float r = length(i.uv - float2( .5 , .5 ));

                float3 col = tex2D( _Audio , float2( r / 2.0 , 0.0 )).xyz;

                return float4( (normalize(i.debug)*.5 + .5)  * 20.  * length( i.debug) , 1.0 );//.1 * float4(1,0.5f,0.0f,1.0) / ( i.dToPoint * i.dToPoint  * i.dToPoint );
            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}
