
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


//------- Technique: Colored --------

VertexToPixel ColoredVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float4 inColor: COLOR, float4 inPaint: COLOR1)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection) ;
	Output.Position3D = mul(inPos, xWorld) ;
	Output.TextureCoords =inPos;
	Output.Color = inColor;
	
	Output.Paint = inPaint;
	float3 Normal = normalize(mul(normalize(inNormal), xWorld));	
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = saturate(dot(Normal, -xLightDirection));
    
	
	return Output;    
}

PixelToFrame ColoredPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
    
	Output.Color = PSIn.Paint;



	//float distanceFactor = (sqrt(pow(xCamPos.x-PSIn.Position3D.x,2) + pow(xCamPos.y-PSIn.Position3D.y,2) + pow(xCamPos.z-PSIn.Position3D.z,2))-1000)/400.0;
	float dist = distance(PSIn.Position3D, xCamPos);
    Output.Color = dist/200.0;



	Output.Color.a=xOpacity;
	return Output;
}

technique Colored
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 ColoredVS();
		PixelShader  = compile ps_2_0 ColoredPS();
	}
}

//---
