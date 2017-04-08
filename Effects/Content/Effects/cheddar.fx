#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


float4x4 World;
float4x4 View;
float4x4 Projection;

float scanHeight;

float3 LightDir = float3(0, 1, -1);

float4 LightColor = float4(1, 1, 1, 1);
float intensity = 0.6;

texture tex;
texture tex2;

float4 AmbientColor;

texture ModelTextureA;
sampler2D textureSamplerA = sampler_state {
	Texture = (ModelTextureA);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture ModelTextureB;
sampler2D textureSamplerB = sampler_state {
	Texture = (ModelTextureB);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};


struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
	float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
	float4 Normal : TEXCOORD1;
	float3 PiPos: TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position - input.Normal * 0.1 * scanHeight * 2, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);


	output.PiPos = mul(viewPosition, Projection);

	output.TexCoords = input.TexCoords;
	output.Normal = input.Normal;



	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	//Read from texture
	float4 colorInTexture = tex2D(textureSamplerA, input.TexCoords);
	float4 colorInTexture2 = tex2D(textureSamplerB, input.TexCoords);

	//merged textures
	//float4 newTexColor = (1 - scanHeight)*colorInTexture + scanHeight * colorInTexture2;
	//return newTexColor;

	//Light
	float4 normal = input.Normal;
	float4 diffuse = saturate(dot(-LightDir, normal));

	float4 texColorWithLightAbove = colorInTexture * 0.1 + 0.9 * (colorInTexture * LightColor * intensity * diffuse);
	// lerp(x,y,factor)

	float4 texColorWithLightUnder = colorInTexture2*0.1 + 0.9*(colorInTexture2 * LightColor * intensity * diffuse);

	//Scannline part
	float pos = (float)input.PiPos.y;

	if (pos < -4.5 + (scanHeight * 10))
		return texColorWithLightAbove;
	else if (pos > -4.5 + scanHeight * 10 && pos < -4.35 + scanHeight * 10)
		return lerp(float4(1, 0, 0, 1), texColorWithLightAbove, 0.4);
	else
		return texColorWithLightUnder;

}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};