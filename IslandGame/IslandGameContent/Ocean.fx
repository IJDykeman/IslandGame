float4x4 xWorld;
float4x4 View;
float4x4 Projection;
float3 CameraLoc;
float xAmbient;

// TODO: add effect parameters here.

struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
	float3 Position3D    : TEXCOORD2;
};


struct PixelToFrame
{
    float4 Color        : COLOR0;
};

VertexToPixel OceanVertexShaderFunction( float4 inPos : POSITION, float4 inColor : COLOR0)
{
    VertexToPixel output = (VertexToPixel)0;
 
    float4 worldPosition = mul(inPos, xWorld);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Position3D = mul(inPos, xWorld);
    return output;
}

PixelToFrame OceanPixelShaderFunction(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;    

     Output.Color = float4(0,.2588,.4274,1)*xAmbient;
	 Output.Color.a = 1;
     return Output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 OceanVertexShaderFunction();
        PixelShader = compile ps_2_0 OceanPixelShaderFunction();
    }
}
