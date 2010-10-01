float4x4 worldViewProjMatrix;  // Object Space -> Screen Space
float4x4 worldMatrix; // Object Space -> World Space
float4x4 worldViewMatrix; // Object Space -> Camera Space
float4x4 worldITMatrix; // Inverse Transpose of the World Matrix
float4x4 viewITMatrix; // Inverse Transpose of the View Matrix
float4 ModelBBoxOffSet;
float4 ModelBBoxScale;
bool isSkinning;
float4 PointLightColors[2];
float4 PointLightParams[2];
float4 PointLightPositions[2];
float4 uvofs0 = { 0, 0, 0, 0 };
float3 fogParam = { 0, 100, 1 };
float4x3 jointMatrices[48];
float4 EnableShadowFlag;

struct VS_IN {
	float3 Position : POSITION;
	float4 Color : COLOR0;
	float3 Normal : NORMAL;
	float3 Tangent : TEXCOORD5;
	float2 UV0 : TEXCOORD0;
	float4 BoneWeights : TEXCOORD6;
	float4 BoneIndices : TEXCOORD7;
};

struct VS_OUTPUT
{
    float4 Position : POSITION;
	float4 LightColor : TEXCOORD0;
	float4 WorldPos : TEXCOORD1;
	float4 Pos : TEXCOORD2;
	float3 Normal : TEXCOORD3;
	float4 Tangent : COLOR0;
	float4 UV0 : TEXCOORD4;
	float4 EyeVector : TEXCOORD6;
	float3 Color : TEXCOORD7;
};

VS_OUTPUT vs_main( in VS_IN In )
{
	VS_OUTPUT Out;
	Out.Color = In.Color.xyz;

	float4 objPos;
	float3 normal = In.Normal * 2 - 1;
	float3 tangent = In.Tangent * 2 - 1;
	objPos.xyz = In.Position * ModelBBoxScale.xyz + ModelBBoxOffSet.xyz; // Scale and offset the object position using the bounding box info
	objPos.w = 1;
	if (isSkinning) {
		int4 indices = D3DCOLORtoUBYTE4(In.BoneIndices);
		float4 skinnedPos = 0;
		float3 skinnedNormal = 0;
		float3 skinnedTangent = 0;
		
		for (uint b = 0; b < 4; b++) {
			int index = indices[b];
			float weight = In.BoneWeights[b];
			skinnedPos.xyz += mul( objPos, jointMatrices[index] ) * weight;
			skinnedNormal += mul( normal, (float3x3)jointMatrices[index] ) * weight;
			skinnedTangent += mul( tangent, (float3x3)jointMatrices[index] ) * weight;
		}

		objPos.xyz = skinnedPos.xyz;
		normal = skinnedNormal;
		tangent = skinnedTangent;
	}
	float4 worldPos = mul( objPos, worldMatrix );
	Out.WorldPos = worldPos;

	float4 viewPos = mul( objPos, worldViewMatrix );

	// Calculate fog using viewPos
	float fogDist = -viewPos.w - fogParam.x;
	float fogRange = 1 / (fogParam.y - fogParam.x);
	Out.EyeVector.w = pow(2,log2(saturate(fogDist * fogRange)) * fogParam.z);

	float4 screenPos = mul( objPos, worldViewProjMatrix );
	Out.Position = screenPos;
	Out.Pos = screenPos;
	
	normal = mul(normal, (float3x3)worldITMatrix);
	Out.Normal = normal;
	normal = normalize( normal ); // Normalize the normal
	
	tangent = mul(tangent, (float3x3)worldITMatrix);
	Out.Tangent = float4((tangent + 1) / 2, 1);

	float2 uv = uvofs0 + In.UV0;
	Out.UV0.x = uv.x;
	Out.UV0.y = -uv.y + 1;
	Out.UV0.zw = 0;

	//View Vector = WorldPos - CameraPos (world-space)
	float4 cameraPosition = float4(viewITMatrix[0][3], viewITMatrix[1][3], viewITMatrix[2][3], viewITMatrix[3][3]);
	float4 viewDir = worldPos - cameraPosition;
	Out.EyeVector.xyz = normalize(viewDir.xyz);

	//float r1y = max( 1, EnableShadowFlag.y );					// mov r1.y, c7.y
																// max r1.y, -r1.y, c177.y
	//float3 r5 = r1y * PointLightColors[0].xyz;				// mul r5.xyz, r1.y, c173
	//float3 lightColor0 = PointLightColors[0].xyz;				// mov r6.xyz, c173
	//float3 r1 = lightColor0 * -r1y + PointLightColors[1].xyz;	// mad r1.yzw, r6.xxyz, -r1.y, c174.xxyz
	float3 totalColor = 0;										// mov r6.xyz, -c7.w
	//float r4w = 0;											// mov r4.w, -c7.w
	// Apply point lights
	for (uint i = 0; i < 2; i++)
	{
		float3 lightDir = worldPos.xyz - PointLightPositions[i].xyz; // Light Direction
		float lightDistance = length(lightDir);
		float logLightDistance = pow(2, log2(lightDistance) * PointLightParams[i].z);
		float light_var1 = max(PointLightParams[i].w - lightDistance, 0.001);
		float light_var2 = 1 / max(logLightDistance * PointLightParams[i].w, 0.001);
		float attenuation = light_var1 * light_var2;

		lightDir = normalize(lightDir);
		float nDotL = saturate(dot(normal, -lightDir));

		float3 thisColor = PointLightColors[i];
		totalColor = totalColor + thisColor * nDotL * attenuation;
		//r4w = r4w + 1;
	}
	float4 color;
	color.xyz = totalColor + 1;
	color.w = 1;
	Out.LightColor = color * In.Color;
    return Out;
}