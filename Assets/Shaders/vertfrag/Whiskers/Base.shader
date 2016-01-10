Shader "Custom/whiskersBase" {

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
            uniform int _SideWidth;
 
            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos : SV_POSITION;
                float3 debug : TEXCOORD0;
            };

            uint3 getID( uint id  ){
            	

            	uint base = floor( id / 6 );
            	uint faceID = floor( base  / ( _SideWidth * _SideWidth ));

            	uint tri  = id % 6;
            	uint row = floor( base / _SideWidth );
            	uint col = base % _SideWidth;

            	uint rDoID = row * _SideWidth;
            	uint rUpID = (row + 1) * _SideWidth;
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

             
                return uint3( fID , tri1 , tri2 );

            }
 
 						uint addColumn( uint id ){
 							return id + 8;
 						}
 						uint addRow( uint id ){

 							return id + _SideWidth * 8;

 						}

            uint getVertID( uint face , uint row , uint col ){
              uint id = 0;
              id += face * _SideWidth * _SideWidth;
              id += col;
              id += row * _SideWidth;
              return id * 8;
            }
            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                uint bID = (floor( id / 6) * 8);
                uint tri = id % 6;
                uint faceID = floor( (bID/8) / (_SideWidth * _SideWidth));
                uint idInFace = (bID/8) - (faceID * _SideWidth * _SideWidth  );
                uint rowID = floor( idInFace / _SideWidth );
                uint colID = idInFace % _SideWidth;


                //uint rowID = 

                

                //uint bPerFace = _SideWidth * _SideWidth * 

                //uint v1 = bID;
                //uint v2 = addColumn( bID );
                //uint v3 = addRow( bID );
                //uint v4 = addColumn( addRow(bID));

                uint v1 = getVertID( faceID , rowID , colID );
                uint v2 = getVertID( faceID , rowID , colID+1 );
                uint v3 = getVertID( faceID , rowID+1 , colID );
                uint v4 = getVertID( faceID , rowID+1 , colID+1 );

                if( colID == _SideWidth - 1 ){
                	uint faceUp;
	                if( faceID <= 3 ){
	                	if( faceID == 0 ){ faceUp = 3; }
                	  if( faceID == 1 ){ faceUp = 0; }
                	  if( faceID == 2 ){ faceUp = 1; }
                	  if( faceID == 3 ){ faceUp = 2; }
                    v2 = getVertID( faceUp , rowID     , 0 );
                    v4 = getVertID( faceUp , rowID + 1 , 0 );
	                }else if( faceID == 4 ){
	                	faceUp = 3;
                    v2 = getVertID( 3 , colID , rowID );
                    v4 = getVertID( 3 , colID , rowID + 1);
	                }else if( faceID == 5 ){
                    v1 = getVertID( 5 , rowID , 0 );
                    v3 = getVertID( 5 , rowID + 1 , 0 );
                    v2 = getVertID( 1 , 0 , rowID  );
                    v4 = getVertID( 1 , 0 , rowID +1 );
	                }
	              }

                if( rowID == _SideWidth - 1 ){

                    uint faceUp;
                    if( faceID == 0 ){ 
                      faceUp = 4; 
                      v3 = getVertID( faceUp , 0 , colID );
                      v4 = getVertID( faceUp , 0 , colID +1);
                      if( colID == _SideWidth - 1){
                        faceUp = 3;
                        v4 = getVertID( faceUp , _SideWidth -1 , 0 );
                      }
                    }else if( faceID == 1 ){
                      faceUp = 4;
                      v3 = getVertID( faceUp , _SideWidth - colID -1, 0 );
                      v4 = getVertID( faceUp , _SideWidth - colID -2, 0 );
                      if( colID == _SideWidth - 1){//discard
                       v4 = v3;
                      }
                    }else if( faceID == 2 ){
                      faceUp = 5;
                      v1 = getVertID( faceID , 0 , colID );
                      v2 = getVertID( faceID , 0 , colID+1 );
                      v3 = getVertID( faceUp , 0, _SideWidth - colID -1 );
                      v4 = getVertID( faceUp , 0, _SideWidth - colID -2 );
                      if( colID == _SideWidth - 1){//discard
                       v4 = v3;
                      }
                    }else if( faceID == 3 ){
                      faceUp = 5;
                      v1 = getVertID( faceID , 0 , colID );
                      v2 = getVertID( faceID , 0 , colID+1 );
                      v3 = getVertID( faceUp ,_SideWidth - 1- colID, _SideWidth - 1 );
                      v4 = getVertID( faceUp , _SideWidth - 1 -colID-1, _SideWidth - 1 );
                      if( colID == _SideWidth - 1){//discard
                       v4 = v3;
                      }
                    }else if( faceID == 4 ){
                      faceUp = 2;
                      v3 = getVertID( faceUp , _SideWidth - 1 , _SideWidth - 1- colID     );
                      v4 = getVertID( faceUp , _SideWidth - 1 , _SideWidth - 1 -colID-1  );
                      if( colID == _SideWidth - 1){//discard
                       v4 = v3;
                      }
                      
                    }else if( faceID == 5 ){
                      faceUp = 0;
                      v3 = getVertID( faceUp , 0 , colID     );
                      v4 = getVertID( faceUp , 0 , colID +1  );
                      if( colID == _SideWidth - 1){//discard
                       v4 = v3;
                      }
                      
                    }


                }


                uint fID;
                uint t1;
                uint t2;

                if( tri == 0 ){
                  fID = v1;
                  t1 =v3;
                  t2 =v4;
                }else if( tri == 1 ){
                  fID = v3;
                  t1 = v4;
                  t2 = v1;
                }else if( tri == 2 ){
                  fID = v4;
                  t1 = v1;
                  t2 = v3;
                }else if( tri == 3 ){
                  fID = v1;
                  t1 = v4;
                  t2 = v2;
                }else if( tri == 4 ){
                  fID = v4;
                  t1 = v2;
                  t2 = v1;
                }else if( tri == 5 ){
                  fID  = v2;
                  t1 = v1;
                  t2 = v4;
                }else{
                  fID = 0;
                }



                //if( triID == 0 ){ bID + 8;}
                //if( triID == 1 ){ bID - 8;}
                //uint3 fID = getID( bID );

                Vert v = buf_Points[fID];
                float3 tri1 = buf_Points[t1].pos;
                float3 tri2 = buf_Points[t2].pos;

                float3 nor = normalize(cross( normalize(v.pos - tri1) , normalize(v.pos - tri2)));
                float3 worldPos = v.pos;

               // if(  rowID == _SideWidth - 1  || colID == _SideWidth - 1 ){ worldPos = 0; }
                if(faceID == 2 && colID != _SideWidth - 1 ){ worldPos = v.pos; }
                //if( tri < 3){ worldPos = 0.;}

                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
                
                if( faceID == 0 ){
                  o.debug = float3( 1. , 0. , 0. );
                }else if( faceID == 1 ){
                  o.debug = float3( 1. , 1. , 0. );
                }else if( faceID == 2 ){
                  o.debug = float3( 1. , 0. , 1. );
                }else if( faceID == 3 ){
                  o.debug = float3( 1. , 1. , 1. );
                }else if( faceID == 4 ){
                  o.debug = float3( 0. , 1. , 0. );
                }else if( faceID == 5 ){
                  o.debug = float3( 0. , 0. , 1. );
                }

                //o.debug = float3( 0,float(colID)/_SideWidth,float(rowID)/_SideWidth); //v.uv.x , v.uv.y);
                o.debug = nor * .5 + .5;

                return o;
            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings i) : COLOR {
                return float4(i.debug , 1.);//.1 * float4(1,0.5f,0.0f,1.0) / ( i.dToPoint * i.dToPoint  * i.dToPoint );
            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}