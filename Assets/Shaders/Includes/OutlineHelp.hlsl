#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

SAMPLER(sampler_point_clamp);

// pts ssampled relative to starting point
static float2 sobelSamplePoints[9] = {
    float2(-1, 1), float2(0, 1), float2(1, 1), 
    float2(-1, 0), float2(0, 0), float2(1, 1),
    float2(-1, -1), float2(0, -1), float2(1, -1)
};

// wts for x-component
static float sobelXMatrix[9] = {
    1, 0, -1, 
    2, 0, -2, 
    1, 0, -1
};

//wts for y-component
static float sobelYMatrix[9] = {
    1, 2, 1, 
    0, 0, 0, 
    -1, -2, -1
};

void GetDepth_float(float2 uv, out float Depth)
{
    Depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv);
}


void GetNormal_float(float2 uv, out float3 Normal) 
{
    Normal = SAMPLE_TEXTURE2D(_NormalsBuffer, sampler_point_clamp, uv).rgb;
}

void DepthSobel_float(float2 UV, float Thickness, out float Out) {
    float2 sobel = 0;

    [unroll] for (int i = 0; i < 9; i++) {
        // float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness);
        float depth = 
        sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
    }
    Out = length(sobel);
}

#endif