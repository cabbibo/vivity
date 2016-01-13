﻿ Shader "Custom/RaytraceFloor" {
  Properties {
  

    _NumberSteps( "Number Steps", Int ) = 20
    _MaxTraceDistance( "Max Trace Distance" , Float ) = 10.0
    _IntersectionPrecision( "Intersection Precision" , Float ) = 0.0001



  }
  
  SubShader {
    //Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

    Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
    LOD 200

    Pass {
      //Blend SrcAlpha OneMinusSrcAlpha // Alpha blending


      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      // Use shader model 3.0 target, to get nicer looking lighting
      #pragma target 3.0

      #include "UnityCG.cginc"
      
 
      


      uniform int _NumberSteps;
      uniform float  _IntersectionPrecision;
      uniform float _MaxTraceDistance;

      uniform float3 _Hand1;
      uniform float3 _Hand2;
      


      struct VertexIn
      {
         float4 position  : POSITION; 
         float3 normal    : NORMAL; 
         float4 texcoord  : TEXCOORD0; 
         float4 tangent   : TANGENT;
      };

      struct VertexOut {
          float4 pos    : POSITION; 
          float3 normal : NORMAL; 
          float4 uv     : TEXCOORD0; 
          float3 ro     : TEXCOORD2;

          //float3 rd     : TEXCOORD3;
          float3 camPos : TEXCOORD4;
      };
        

      float sdBox( float3 p, float3 b ){

        float3 d = abs(p) - b;

        return min(max(d.x,max(d.y,d.z)),0.0) +
               length(max(d,0.0));

      }

      float sdSphere( float3 p, float s ){
        return length(p)-s;
      }

      float sdCapsule( float3 p, float3 a, float3 b, float r )
      {
          float3 pa = p - a, ba = b - a;
          float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
          return length( pa - ba*h ) - r;
      }

      float2 smoothU( float2 d1, float2 d2, float k)
      {
          float a = d1.x;
          float b = d2.x;
          float h = clamp(0.5+0.5*(b-a)/k, 0.0, 1.0);
          return float2( lerp(b, a, h) - k*h*(1.0-h), lerp(d2.y, d1.y, pow(h, 2.0)));
      }

      
      float3 modit(float3 x, float3 m) {
			    float3 r = x%m;
			    return r<0 ? r+m : r;
			}
      float2 map( in float3 pos ){
        
        float2 res;
        float2 lineF;
        float2 sphere;

			res = float2( sdSphere( pos + float3( 0 , -.2 , 0.), .4 ) , 0.6 );
        float3 modVal = float3( .4 , .4 , .4 );
        pos -= float3( 0 , .2 , 0.);
//
        //pos += .1 * float3( sin( pos.x * 10. ) , sin( pos.y * 10. ) , sin( pos.z * 10. ));
        //pos += .1 * float3( sin( pos.x * 10. ) , sin( pos.y * 10. ) , sin( pos.z * 10. ));
        //pos += .1 * float3( sin( pos.x * 10. ) , sin( pos.y * 10. ) , sin( pos.z * 10. ));
        int3 test;
        float2 res2 = float2( sdSphere( modit(pos , modVal) - modVal / 2. , .1 ) , 0.6 );
       	res = smoothU( res , res2 , 0.1 );
    		//res = float2( length( pos - float3( 0., -.8 ,0) ) - 1., 0.1 );
    		//res = smoothU( res , float2( length( pos - float3( .3 , .2 , -.2) ) - .1, 0.1 ) , .05 );
    		//res = smoothU( res , float2( length( pos - float3( -.4 , .2 , .4) ) - .1, 0.1 ) , .05 );
    		//res = smoothU( res , float2( length( pos - float3( 0.3 , .2 , -.3) ) - .1, 0.1 ) , .05 );

  	    return res; 
  	 
  	  }

      float3 calcNormal( in float3 pos ){

      	float3 eps = float3( 0.001, 0.0, 0.0 );
      	float3 nor = float3(
      	    map(pos+eps.xyy).x - map(pos-eps.xyy).x,
      	    map(pos+eps.yxy).x - map(pos-eps.yxy).x,
      	    map(pos+eps.yyx).x - map(pos-eps.yyx).x );
      	return normalize(nor);

      }
              
         

      float2 calcIntersection( in float3 ro , in float3 rd ){     
            
               
        float h =  _IntersectionPrecision * 2;
        float t = 0.0;
        float res = -1.0;
        float id = -1.0;
        
        for( int i=0; i< _NumberSteps; i++ ){
            
            if( h < _IntersectionPrecision || t > _MaxTraceDistance ) break;
    
            float3 pos = ro + rd*t;
            float2 m = map( pos );
            
            h = m.x;
            t += h;
            id = m.y;
            
        }
    
    
        if( t <  _MaxTraceDistance ){ res = t; }
        if( t >  _MaxTraceDistance ){ id = -1.0; }
        
        return float2( res , id );
          
      
      }
            
    

      VertexOut vert(VertexIn v) {
        
        VertexOut o;

        o.normal = v.normal;
        
        o.uv = v.texcoord;
  
        // Getting the position for actual position
        o.pos = mul( UNITY_MATRIX_MVP , v.position );
     
        float3 mPos = mul( _Object2World , v.position );

        o.ro = v.position;
        o.camPos = mul( _World2Object , float4( _WorldSpaceCameraPos  , 1. )); 

        return o;

      }


     // Fragment Shader
      fixed4 frag(VertexOut i) : COLOR {

        float3 ro = i.ro;
        float3 rd = normalize(ro - i.camPos);

        float3 col = float3( 0.0 , 0.0 , 0.0 );
    		float2 res = calcIntersection( ro , rd );
    		
    		col= float3( 0. , 0. , 0. );

    		if( res.y > -0.5 ){

    			float3 pos = ro + rd * res.x;
    			float3 norm = calcNormal( pos );
    			col = norm * .5 + .5;
    			//col = float3( 1. , 0. , 0. );
    			
    		}
     
    		//col = float3( 1. , 1. , 1. );

            fixed4 color;
            color = fixed4( col , 1. );
            return color;
      }

      ENDCG
    }
  }
  FallBack "Diffuse"
}