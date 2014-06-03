
struct VertexToPixel
{
    float4 Position   	: POSITION;  
	float4 Position3D   : TEXCOORD2;    
    float4 Color		: COLOR0;
	float4 Paint		: COLOR1;
    float LightingFactor: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};





//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;


float3 xLightDirection;
float xAmbient;
float xOpacity;
bool xEnableLighting;
bool xShowNormals;
float3 xCamPos;
float3 xCamUp;
float xPointSpriteSize;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = POINT; minfilter = POINT; mipfilter=POINT; AddressU = mirror; AddressV = mirror;};
//change LINEAR to POINT for blocky textures


struct CreateShadowMap_VSOut
{
    float4 Position : POSITION;
    float Depth     : TEXCOORD0;
};
CreateShadowMap_VSOut ColoredVS(float4 Position: POSITION)
{	
    CreateShadowMap_VSOut Out;
    Out.Position = mul(Position, mul(xWorld, mul(xView,xProjection))); 
    Out.Depth = Out.Position.z / Out.Position.w;    
    return Out;
}



CreateShadowMap_VSOut InstancedColoredVS(float4 Position: POSITION, float4x4 instanceTransform : TEXCOORD1, float2 atlasCoord : TEXCOORD5)
{	
    CreateShadowMap_VSOut Out;
    Out.Position = mul(Position, transpose(instanceTransform)); 
    Out.Depth = Out.Position.z / Out.Position.w;    
    return Out;
}

float4 ColoredPS(CreateShadowMap_VSOut input) : COLOR
{
    return float4(input.Depth, 0, 0, 0);
}

technique Colored
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 ColoredVS();
		PixelShader  = compile ps_2_0 ColoredPS();
	}
}

technique Instanced
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 InstancedColoredVS();
		PixelShader  = compile ps_2_0 ColoredPS();
	}
}

//---
