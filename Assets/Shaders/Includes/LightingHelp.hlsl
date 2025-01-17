void GetMainLight_float(float3 WorldPos, out float3 Color, out float3 Direction, out float DistanceAtten, out float ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(float3(0.5, 0.5, 0));
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
        float4 clipPos = TransformWorldToClip(WorldPos);
        float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif

    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void ComputeAdditionalLighting_float(float3 WorldPosition, float3 WorldNormal,
    float2 Thresholds, float3 RampedDiffuseValues,
    out float3 Color, out float Diffuse)
{
    Color = float3(0, 0, 0);
    Diffuse = 0;

#ifndef SHADERGRAPH_PREVIEW

    int pixelLightCount = GetAdditionalLightsCount();
    
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, WorldPosition);
        float4 tmp = unity_LightIndices[i / 4];
        uint light_i = tmp[i % 4];

        half shadowAtten = light.shadowAttenuation * AdditionalLightRealtimeShadow(light_i, WorldPosition, light.direction);
        
        half NdotL = saturate(dot(WorldNormal, light.direction));
        half distanceAtten = light.distanceAttenuation;

        half thisDiffuse = distanceAtten * shadowAtten * NdotL;
        
        half rampedDiffuse = 0;
        
        if (thisDiffuse < Thresholds.x)
        {
            rampedDiffuse = RampedDiffuseValues.x;
        }
        else if (thisDiffuse < Thresholds.y)
        {
            rampedDiffuse = RampedDiffuseValues.y;
        }
        else
        {
            rampedDiffuse = RampedDiffuseValues.z;
        }
        
        
        if (shadowAtten * NdotL == 0)
        {
            rampedDiffuse = 0;

        }
        
        if (light.distanceAttenuation <= 0)
        {
            rampedDiffuse = 0.0;
        }

        Color += max(rampedDiffuse, 0) * light.color.rgb;
        Diffuse += rampedDiffuse;
    }
#endif
}

void ChooseColor_float(float3 Highlight, float3 Midtone, float3 Shadow, float DiffuseM, float DiffuseHS, float Min_Threshold, float Max_Threshold, out float3 OUT)
{
    if (DiffuseM > Min_Threshold && DiffuseM < Max_Threshold) {
        OUT = Midtone;
    } else if (DiffuseHS < Min_Threshold) {
        OUT = Shadow;
    } else if (DiffuseHS > Max_Threshold) {
        OUT = Highlight;
    } else {
        OUT = Midtone;
    }
}

void ChooseColor1_float(float3 Highlight, float3 Midtone, float3 Shadow, float DiffuseM, float DiffuseT1, float DiffuseT2, float Min_Threshold, float Max_Threshold, out float3 OUT)
{
    float3 col;
    float a = 1.025;
    float b = 1.05;
    float c = 1.1;
    float d = 1.15;

    float3 MinMidtone = 0.5 * Shadow + 0.5 * Midtone;
    Shadow = DiffuseT1 * Shadow + (1 - DiffuseT1) * MinMidtone;

    float t = (DiffuseM - Min_Threshold) / (Max_Threshold - Min_Threshold);
    float3 MaxMidtone = 0.4 * Midtone + 0.6 * Highlight;
    Midtone = (1 - t) * Shadow + t * MaxMidtone;

    if (DiffuseM < Min_Threshold) {
        col = Shadow;      
        
        /*if (DiffuseT1 <= Min_Threshold * 0.25) {
            OUT = float3(col[0] / d, col[1] / c, col[2] / a);
        } else if (DiffuseT1 < Min_Threshold * 0.5) {
            OUT = float3(col[0] / c, col[1] / d, col[2] / b);
        } else if (DiffuseT1 < Min_Threshold * 0.75) {
            OUT = float3(col[0] / c, col[1] / c, col[2] / b);
        } else {
            OUT = float3(col[0] / b, col[1] / b, col[2] / 1.01);
        }*/

        if (DiffuseT2 <= Min_Threshold) {
            OUT = Shadow;
        } else {
            OUT = Midtone;
        }
    } else if (DiffuseM >= Max_Threshold) {
        if (DiffuseT2 >= Max_Threshold) {
            OUT = Highlight;
        } else {
            OUT = Midtone;
        }
    } else {
        OUT = Midtone;
    }

    
}