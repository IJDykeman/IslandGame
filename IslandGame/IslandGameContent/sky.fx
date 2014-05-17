float4x4 World;
float4x4 View;
float4x4 Projection;


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

VertexToPixel VertexShaderFunction( float4 inPos : POSITION, float4 inColor : COLOR0)
{
    VertexToPixel output = (VertexToPixel)0;
 
    float4 worldPosition = mul(inPos, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Position3D = mul(inPos, World);
    return output;
}

PixelToFrame PixelShaderFunction(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;    
	float4 horizonColor = float4(.392,.584,.733,1);
	float4 highColor = float4(.019,.243,.549,1)*1.7;
	float y = PSIn.Position3D.y;
	y = clamp(y,0,10000000);
	float heightFactor = pow(y/500.0,.6);
     Output.Color = highColor*heightFactor + horizonColor*(1.0-heightFactor);
	 Output.Color.a = 1;
     return Output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
