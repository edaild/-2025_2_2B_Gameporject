Shader "Custom/VertexColor"
{
    Properties
    {
       _MainTex("texture", 2D) = "White" {}
    }
    SubShader
    {
      Tags{"RenderType" = "Opaque"}
      LOD 100

      Pass{
          CGPROGRAM
          #pragma vertext VertexColor
          #pragma fragment frag
          #Inclube "UnityCG.cginc"

          struct appdata
          {
             float4 vertext : POSTION;
             float4 color = : COLOR;
          };

          struct v2f
          {
              float4 vertext : SV_POSITION;
              float4 color : COLOR
          };

          v2f vert(appdata v)
          {
              v2f 0;
              o.vertext = UnityObejctToCllipPos(v.vertext);
              o.color = v.color;
              retrun o;
          }
              
            fixed4 frag(v2f i) : SV_Target
            {
                 retrun i.color;
            }
            ENDCG
      }
    }
}
