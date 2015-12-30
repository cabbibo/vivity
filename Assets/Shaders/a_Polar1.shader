Shader "Custom/a_Polar1" {
	Properties {

		_MainTex ("Albedo (RGB)", 2D) = "black" {}
	}
	  SubShader {
    //Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

    Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
    LOD 200
    Cull Off

    Pass {
      //Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
		

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      // Use shader model 3.0 target, to get nicer looking lighting
      #pragma target 3.0

      #include "UnityCG.cginc"

      
 
      


      uniform sampler2D _MainTex;


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
          float4 col    : TEXCOORD1;
      };
       


      float3 cart2Pol( float3 p ){

      	float radius = sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
				float theta = atan2(p.y, p.x);
				float phi = acos(p.z / radius);

				return float3( radius , theta , phi );
      }

      float3 pol2Cart( float3 p ){

				float x = cos(p.y) * cos(p.z) * p.x;
				float y = sin(p.y) * cos(p.z) * p.x;
				float z = sin(p.z) * p.x;

				return float3( x , y , z );

      }
            
    

      VertexOut vert(VertexIn v) {
        
        VertexOut o;

        o.normal = normalize( mul( UNITY_MATRIX_IT_MV, float4( v.normal.xyz , 0.) ).xyz );
        
        o.uv = v.texcoord;

        float3 pos = v.position;

        float3 pol = cart2Pol( pos );

        float luVal = length( pos)/10.;
        fixed4 t = tex2Dlod( _MainTex , float4( luVal , 0. , 0. , 0. ));

        o.col = t;
       // t = pow( t , .4 );
        t = t * float4( luVal  + 2. , luVal  + 2. , luVal  + 2. , luVal  + 2.);

        float3 p = pol2Cart( float3( 20. * t.y , t.x  * 2.  , t.z * 2. ) );
  
  			float3 dir = float3( sin( pol.y ) , cos( pol.z ) , 0. );
        // Getting the position for actual position
        o.pos = mul( UNITY_MATRIX_MVP , v.position + float4(  1. * v.normal * t.xyz * p , 1. ));
     
        return o;

      }


     // Fragment Shader
      fixed4 frag(VertexOut i) : COLOR {

      	float3 col = i.normal * .5 + .5;
      	fixed4 color;

        color = fixed4( col * i.col.xyz * 200. * .5 + col * .5, 1. );
        return color;

     	}

      ENDCG
    }
  }
  FallBack "Diffuse"
}
