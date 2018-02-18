// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33117,y:32690,varname:node_3138,prsc:2|emission-8329-OUT;n:type:ShaderForge.SFN_Tex2d,id:9073,x:32643,y:32632,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9073,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8a8f79905213d194fb47001eb60171df,ntxv:0,isnm:False|UVIN-4656-UVOUT;n:type:ShaderForge.SFN_Time,id:7569,x:32103,y:32842,varname:node_7569,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:6505,x:32190,y:32661,varname:node_6505,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Rotator,id:4656,x:32434,y:32755,varname:node_4656,prsc:2|UVIN-6505-UVOUT,ANG-5861-OUT,SPD-7569-T;n:type:ShaderForge.SFN_ValueProperty,id:5861,x:32204,y:33043,ptovrint:False,ptlb:rotationSpeed,ptin:_rotationSpeed,varname:node_5861,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:8329,x:32948,y:32711,varname:node_8329,prsc:2|A-9073-RGB,B-2913-OUT,T-7114-OUT;n:type:ShaderForge.SFN_Tex2d,id:1645,x:32643,y:32859,ptovrint:False,ptlb:SecondTex,ptin:_SecondTex,varname:node_1645,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8a8f79905213d194fb47001eb60171df,ntxv:0,isnm:False|UVIN-3881-UVOUT;n:type:ShaderForge.SFN_Color,id:9215,x:32615,y:33126,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_9215,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:2913,x:32856,y:32940,varname:node_2913,prsc:2|A-1645-RGB,B-9215-RGB;n:type:ShaderForge.SFN_Slider,id:7114,x:32807,y:32580,ptovrint:False,ptlb:node_7114,ptin:_node_7114,varname:node_7114,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3418804,max:1;n:type:ShaderForge.SFN_Rotator,id:3881,x:32435,y:32925,varname:node_3881,prsc:2|UVIN-6505-UVOUT,SPD-9010-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9010,x:32344,y:33133,ptovrint:False,ptlb:node_9010,ptin:_node_9010,varname:node_9010,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-1;proporder:9073-5861-1645-9215-7114-9010;pass:END;sub:END;*/

Shader "Shader Forge/TeleportShader" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _rotationSpeed ("rotationSpeed", Float ) = 1
        _SecondTex ("SecondTex", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _node_7114 ("node_7114", Range(0, 1)) = 0.3418804
        _node_9010 ("node_9010", Float ) = -1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _rotationSpeed;
            uniform sampler2D _SecondTex; uniform float4 _SecondTex_ST;
            uniform float4 _Color;
            uniform float _node_7114;
            uniform float _node_9010;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_7569 = _Time + _TimeEditor;
                float node_4656_ang = _rotationSpeed;
                float node_4656_spd = node_7569.g;
                float node_4656_cos = cos(node_4656_spd*node_4656_ang);
                float node_4656_sin = sin(node_4656_spd*node_4656_ang);
                float2 node_4656_piv = float2(0.5,0.5);
                float2 node_4656 = (mul(i.uv0-node_4656_piv,float2x2( node_4656_cos, -node_4656_sin, node_4656_sin, node_4656_cos))+node_4656_piv);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4656, _MainTex));
                float4 node_3831 = _Time + _TimeEditor;
                float node_3881_ang = node_3831.g;
                float node_3881_spd = _node_9010;
                float node_3881_cos = cos(node_3881_spd*node_3881_ang);
                float node_3881_sin = sin(node_3881_spd*node_3881_ang);
                float2 node_3881_piv = float2(0.5,0.5);
                float2 node_3881 = (mul(i.uv0-node_3881_piv,float2x2( node_3881_cos, -node_3881_sin, node_3881_sin, node_3881_cos))+node_3881_piv);
                float4 _SecondTex_var = tex2D(_SecondTex,TRANSFORM_TEX(node_3881, _SecondTex));
                float3 node_2913 = (_SecondTex_var.rgb*_Color.rgb);
                float3 emissive = lerp(_MainTex_var.rgb,node_2913,_node_7114);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
