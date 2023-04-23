// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ElectricPulse"
{
	Properties
	{
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_Speed1("Speed1", Vector) = (0,0.5,0,0)
		_Speed2("Speed2", Vector) = (0,0.5,0,0)
		[HDR]_Color("Color", Color) = (2,1.960784,0,0)
		_Power("Power", Range( 0 , 10)) = 12
		_Width("Width", Range( 1 , 10)) = 5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _NoiseTex;
		uniform float2 _Speed1;
		uniform float2 _Speed2;
		uniform float _Power;
		uniform float _Width;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner10 = ( 1.0 * _Time.y * _Speed1 + float2( 0,0 ));
			float2 uv_TexCoord8 = i.uv_texcoord + panner10;
			float2 panner14 = ( 1.0 * _Time.y * _Speed2 + float2( 0,0 ));
			float2 uv_TexCoord12 = i.uv_texcoord + panner14;
			float4 temp_cast_0 = (_Power).xxxx;
			float4 temp_cast_1 = (-10.0).xxxx;
			float4 temp_cast_2 = (10.0).xxxx;
			float2 appendResult10_g1 = (float2(_Width , _Width));
			float2 temp_output_11_0_g1 = ( abs( ((temp_cast_1 + (pow( ( tex2D( _NoiseTex, uv_TexCoord8 ) + tex2D( _NoiseTex, uv_TexCoord12 ) ) , temp_cast_0 ) - float4( 0,0,0,0 )) * (temp_cast_2 - temp_cast_1) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 ))).rg*2.0 + -1.0) ) - appendResult10_g1 );
			float2 break16_g1 = ( 1.0 - ( temp_output_11_0_g1 / fwidth( temp_output_11_0_g1 ) ) );
			o.Emission = ( _Color * saturate( min( break16_g1.x , break16_g1.y ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18909
206;141;1947;1180;908.569;480.41;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;40;-1953.885,-494.814;Inherit;False;1650.749;754.4885;Comment;12;37;18;20;17;13;14;12;8;6;10;11;39;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;39;-1903.885,7.200935;Inherit;False;Property;_Speed2;Speed2;2;0;Create;True;0;0;0;False;0;False;0,0.5;0.1,-0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;11;-1900.463,-163.6473;Inherit;False;Property;_Speed1;Speed1;1;0;Create;True;0;0;0;False;0;False;0,0.5;0.2,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;10;-1635.127,-166.685;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;14;-1622.755,-2.166967;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1390.833,-206.5093;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1387.46,-37.09117;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;37;-1450.883,-444.814;Inherit;True;Property;_NoiseTex;NoiseTex;0;0;Create;True;0;0;0;False;0;False;None;e28dc97a9541e3642a48c0e3886688c5;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;13;-1115.943,-51.49714;Inherit;True;Property;_TextureSample2;Texture Sample 2;1;0;Create;True;0;0;0;False;0;False;-1;e28dc97a9541e3642a48c0e3886688c5;e28dc97a9541e3642a48c0e3886688c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-1114.333,-241.3094;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;e28dc97a9541e3642a48c0e3886688c5;e28dc97a9541e3642a48c0e3886688c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-723.5762,-109.4677;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;41;-886.3314,300.1423;Inherit;False;1060.547;558.9397;Comment;7;21;22;27;24;34;28;33;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-958.2925,144.6745;Inherit;False;Property;_Power;Power;4;0;Create;True;0;0;0;False;0;False;12;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-836.3314,624.2706;Inherit;False;Constant;_Float3;Float 3;3;0;Create;True;0;0;0;False;0;False;-10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;18;-564.1367,-37.04322;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-832.3314,696.2706;Inherit;False;Constant;_Float4;Float 4;3;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;21;-632.3579,538.8589;Inherit;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-569.8551,744.0815;Inherit;False;Property;_Width;Width;5;0;Create;True;0;0;0;False;0;False;5;5;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;22;-369.6297,530.9481;Inherit;True;Rectangle;-1;;1;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;0.5;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;33;-341.7842,350.1423;Inherit;False;Property;_Color;Color;3;1;[HDR];Create;True;0;0;0;False;0;False;2,1.960784,0,0;0.475316,2.670157,2.670157,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-60.78426,433.1423;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;52;379.1474,37.012;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;ElectricPulse;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;16;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;4;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;2;11;0
WireConnection;14;2;39;0
WireConnection;8;1;10;0
WireConnection;12;1;14;0
WireConnection;13;0;37;0
WireConnection;13;1;12;0
WireConnection;6;0;37;0
WireConnection;6;1;8;0
WireConnection;17;0;6;0
WireConnection;17;1;13;0
WireConnection;18;0;17;0
WireConnection;18;1;20;0
WireConnection;21;0;18;0
WireConnection;21;3;27;0
WireConnection;21;4;28;0
WireConnection;22;1;21;0
WireConnection;22;2;24;0
WireConnection;22;3;24;0
WireConnection;34;0;33;0
WireConnection;34;1;22;0
WireConnection;52;2;34;0
ASEEND*/
//CHKSM=3CE6F968E04A49B9FF5B577E713B098D70312401