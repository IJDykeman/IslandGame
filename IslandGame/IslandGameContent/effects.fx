
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
float2 ShadowMapPixelSize;
float2 ShadowMapSize;

float3 xLightDirection;
float xAmbient;
float xOpacity;
bool xEnableLighting;
bool xShowNormals;
float3 xCamPos;
float3 xCamUp;
float xPointSpriteSize;
float4 xTint;

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




VertexToPixel InstancedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float4 inColor: COLOR, float4 inPaint: COLOR1, float4x4 instanceTransform : TEXCOORD1, float2 atlasCoord : TEXCOORD5)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (transpose(instanceTransform), preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection) ;
	Output.Position3D = mul(inPos, transpose(instanceTransform) );
	Output.TextureCoords =inPos;
	Output.Color = inColor;
	
	Output.Paint = inPaint;
	float3 Normal = normalize(mul(normalize(inNormal), transpose(instanceTransform)));	
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





	float4 lightingPosition = mul(PSIn.Position3D, mul (xShadowView, xShadowProjection));
        //float4 lightingPosition = mul(input.WorldPos, LightViewProj);
    
    // Find the position in the shadow map for this pixel
   float2 ShadowTexCoord = 0.5 * lightingPosition.xy / 
                            lightingPosition.w + float2( 0.5, 0.5 );
    ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

    // Get the current depth stored in the shadow map
    /*float shadowdepth = tex2D(TextureSampler, ShadowTexCoord).r;    
    
    // Calculate the current pixel depth
    // The bias is used to prevent folating point errors that occur when
    // the pixel of the occluder is being drawn
    float ourdepth = (lightingPosition.z / lightingPosition.w) - 0.005f;
    




    if (shadowdepth < ourdepth && ShadowTexCoord.y>=0 && ShadowTexCoord.y<=1 
	&& ShadowTexCoord.x>=0 && ShadowTexCoord.x<=1 && shadowdepth<1)
    {
        // Shadow the pixel by lowering the intensity
        Output.Color *= float4(0.6,0.6,0.7,0);

    };*/
	//Output.Color *= xTint;
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

technique Instanced
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 InstancedVS();
		PixelShader  = compile ps_2_0 ColoredPS();
	}
}

