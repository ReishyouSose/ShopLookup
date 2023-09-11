sampler tex : register(s0);

float4 hitbox;
float4 inner;

bool Contains(float4 rec, float2 coords)
{
    if (rec.x <= coords.x && coords.x <= rec.x + rec.z && rec.y <= coords.y)
    {
        return coords.y <= rec.y + rec.w;
    }

    return false;
}

float4 InvisibleBlur(float2 coords :TEXCOORD0) : COLOR0
{
    if (Contains(inner, coords))
    {
        return tex2D(tex, coords);
    }
    if (!Contains(hitbox, coords))
    {
        return float4(0, 0, 0, 0);
    }
    float opacity = 1;
    if (coords.x < inner.x)  
        opacity *= lerp(hitbox.x, inner.x, coords.x);
    else if (coords.x > inner.x + inner.z)
        opacity *= lerp(inner.x + inner.z, hitbox.x + hitbox.z, coords.x);
    if (coords.y < inner.y)  
        opacity *= lerp(hitbox.y, inner.y, coords.y);
    else if (coords.x > inner.y + inner.w)
        opacity *= lerp(inner.y + inner.w, hitbox.y + hitbox.w, coords.y);
    return tex2D(tex, coords) * opacity;
}

technique T
{
    pass InvisibleBlur
    {
        PixelShader = compile ps_3_0 InvisibleBlur();
    }
}