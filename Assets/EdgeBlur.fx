sampler tex0 : register(s0);

int type;
float2 resolution;
float4 outer;
float4 inner;

float CheckPos(float2 pos)
{
    float2 center = (inner.xy + inner.zw) / 2;
    float x, y;
    
    if (pos.x < center.x)
        x = lerp(outer.x, inner.x, pos.x);
    else
        x = lerp(outer.z, inner.z, pos.x);
    x = clamp(x, 0, 1);
    
    if (pos.y < center.y)
        y = lerp(outer.y, inner.y, pos.y);
    else
        y = lerp(outer.w, inner.w, pos.y);
    y = clamp(y, 0, 1);
    
    return min(x, y);
}

float4 EdgeBlur(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(tex0, coords);
    float2 pos = coords * resolution;
    return c * CheckPos(pos);
}

technique T
{
    pass EdgeBlur
    {
        PixelShader = compile ps_3_0 EdgeBlur();
    }
}