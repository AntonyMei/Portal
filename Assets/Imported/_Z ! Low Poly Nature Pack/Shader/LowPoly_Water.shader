Shader "Zomoss/LowPoly_Water"
{
	Properties
	{
		_WaterColor("Water Color", Color) = (1,1,1,0)
		_WaterNormal("Water Normal", 2D) = "bump" {}
		_WaterSmoothness("Water Smoothness", Range( 0 , 5)) = 2
		_WaterSpeed("Water Speed", Range( 0 , 1)) = 0
		_WaterOpacity("Water Opacity", Range( 0 , 1)) = 0
		_FoamColor("Foam Color", Color) = (0,0,0,0)
		_FoamDepth("Foam Depth", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular alpha:fade keepalpha 
		struct Input
		{
			half2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D _WaterNormal;
		uniform half _WaterSpeed;
		uniform float4 _WaterNormal_ST;
		uniform half4 _FoamColor;
		uniform half4 _WaterColor;
		uniform sampler2D _CameraDepthTexture;
		uniform half _FoamDepth;
		uniform sampler2D _Foam_Texture;
		uniform float4 _Foam_Texture_ST;
		uniform half _WaterSmoothness;
		uniform half _WaterOpacity;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			half2 temp_cast_0 = (_WaterSpeed).xx;
			float2 uv_WaterNormal = i.uv_texcoord * _WaterNormal_ST.xy + _WaterNormal_ST.zw;
			float2 panner19 = ( 1.0 * _Time.y * temp_cast_0 + uv_WaterNormal);
			o.Normal = UnpackScaleNormal( tex2D( _WaterNormal, panner19 ) ,1 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float eyeDepth1 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float temp_output_89_0 = abs( ( eyeDepth1 - ase_screenPos.w ) );
			float4 lerpResult13 = lerp( _FoamColor , _WaterColor , saturate( pow( ( temp_output_89_0 + 0.5 ) , 1 ) ));
			float2 uv_Foam_Texture = i.uv_texcoord * _Foam_Texture_ST.xy + _Foam_Texture_ST.zw;
			float2 panner116 = ( 1.0 * _Time.y * float2( -0.01,0.01 ) + uv_Foam_Texture);
			float temp_output_114_0 = ( saturate( pow( ( temp_output_89_0 + _FoamDepth ) , -3 ) ) * tex2D( _Foam_Texture, panner116 ).r );
			float4 lerpResult117 = lerp( lerpResult13 , half4(1,1,1,0) , temp_output_114_0);
			o.Albedo = lerpResult117.rgb;
			o.Specular = 0;
			float lerpResult133 = lerp( _WaterSmoothness , 0 , temp_output_114_0);
			o.Smoothness = ( lerpResult133 * 0.5 );
			o.Alpha = _WaterOpacity;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}