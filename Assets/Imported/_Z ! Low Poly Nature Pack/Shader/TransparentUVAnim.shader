Shader "Zomoss/TransparentUVAnim" {
 Properties {
  _MainTex ("Base (RGB)", 2D) = "white" {}
  _DirectionU("horizontal scroll speed (1 sample)", float) = 0.0
  _DirectionV("vertical scroll speed (1 sample)", float) = 0.0
  _SelfIllumStrength ("_SelfIllumStrength", Range(0.0, 1.5)) = 1.0
  _TintColor ("TintColor", Color) = (1.0, 1.0, 1.0, 1.0)
 }
 SubShader {
  Tags { "RenderType"="Transparent" "IgnoreProjector" = "True" "Queue"="Transparent"}
  
  Blend SrcAlpha OneMinusSrcAlpha
  ZWrite Off
  LOD 200
  Pass
  {
   CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma fragmentoption ARB_precision_hint_fastest
    
    #include "UnityCG.cginc"
    
    uniform half _DirectionU;
    uniform half _DirectionV;
    uniform half4 _MainTex_ST;
    uniform sampler2D _MainTex;
    uniform fixed _SelfIllumStrength;
    uniform fixed4  _TintColor;
    
    struct v2f_full
    {
      half4 pos : POSITION;
      half4 color : TEXCOORD0;
      half2 uv : TEXCOORD1;
    };
    
    v2f_full vert(appdata_full v)
    {
      v2f_full o;
      o.pos = UnityObjectToClipPos(v.vertex);
      o.color = v.color;
      fixed2 uvTex = TRANSFORM_TEX(v.texcoord, _MainTex);
      uvTex.x = uvTex.x + _Time.x * _DirectionU;
      uvTex.y = uvTex.y + _Time.x * _DirectionV;
      o.uv.xy = uvTex;  
      return o;
    }
    
    fixed4 frag(v2f_full i) : COLOR
    {
      fixed4 c = tex2D(_MainTex, i.uv.xy);
      c.rgb *= i.color.rgb + c.a * _SelfIllumStrength;
      c.a = i.color.a * c.a;
      c = c * _TintColor;
      return c;
    }  
   ENDCG
  }
 }
}