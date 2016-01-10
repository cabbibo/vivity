Shader "Custom/AudioShader" {
	Properties {

		_MainTex ("Albedo (RGB)", 2D) = "black" {}
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
      };
       
            
    

      VertexOut vert(VertexIn v) {
        
        VertexOut o;

        o.normal = v.normal;
        
        o.uv = v.texcoord;

        float3 pos = v.position;
        float2 lu = float2(abs(v.normal.z) , 0. );

        fixed4 t = tex2Dlod( _MainTex , float4( lu , 0. , 0.) );
  
        // Getting the position for actual position
        o.pos = mul( UNITY_MATRIX_MVP , v.position + 4. * float4((v.normal * t.xyz),0.) );
     
        return o;

      }


     // Fragment Shader
      fixed4 frag(VertexOut i) : COLOR {

      	float3 col = i.normal * .5 + .5;
      	fixed4 color;

        color = fixed4( col , 1. );
        return color;

     	}

      ENDCG
    }
  }
  FallBack "Diffuse"
}
