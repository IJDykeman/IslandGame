
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

float4x4 xShadowView;
float4x4 xShadowProjection;
float4x4 xShadowWorld;
float3 xLightPos;

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



	Output.Color.rgb *=  (PSIn.LightingFactor  +xAmbient)*saturate(   ((-1.2)*(pow(PSIn.Color.r*4,1)/6) + 1) );
	//Output.Color.rgb *= tex2D(TextureSampler, float2(PSIn.Position.x+PSIn.TextureCoords.y,PSIn.TextureCoords.y));

	float4 fogColor = float4(.392,.573,.722,1)*xAmbient;
	fogColor.a=1;
	float distanceFactor = (sqrt(pow(xCamPos.x-PSIn.Position3D.x,2) + pow(xCamPos.y-PSIn.Position3D.y,2) + pow(xCamPos.z-PSIn.Position3D.z,2))-1000)/400.0;
	distanceFactor = clamp(distanceFactor,0,1);
    Output.Color = fogColor*distanceFactor + Output.Color*(1.0-distanceFactor);

	/*
	float3 clampedPosition = float3((float)((int)(PSIn.Position3D.x*10.0+.04)),(float)((int)(PSIn.Position3D.y*10.0+.04)),
		(float)((int)(PSIn.Position3D.z*10.0+.04)));
	float hash = clampedPosition.x*23.5+clampedPosition.y*222.4+clampedPosition.z*43.6;
	hash = hash%1.0;
	Output.Color *= hash;*/



	float4 lightingPosition = mul(PSIn.Position3D, mul (xShadowView, xShadowProjection));
        //float4 lightingPosition = mul(input.WorldPos, LightViewProj);
    
    // Find the position in the shadow map for this pixel
    float2 ShadowTexCoord = 0.5 * lightingPosition.xy / 
                            lightingPosition.w + float2( 0.5, 0.5 );
    ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

    // Get the current depth stored in the shadow map
    float shadowdepth = tex2D(TextureSampler, ShadowTexCoord).r;    
    
    // Calculate the current pixel depth
    // The bias is used to prevent folating point errors that occur when
    // the pixel of the occluder is being drawn
    float ourdepth = distance(xLightPos,PSIn.Position3D)/300 - .01;
    
    // Check to see if this pixel is in front or behind the value in the shadow map
    if (shadowdepth < ourdepth)
    {
        // Shadow the pixel by lowering the intensity
        Output.Color *= float4(0.5,0.5,0.5,0);
    };
    //Output.Color = float4( shadowdepth-ourdepth,shadowdepth-ourdepth,shadowdepth-ourdepth,shadowdepth-ourdepth);

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

