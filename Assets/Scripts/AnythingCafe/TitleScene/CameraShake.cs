using System;
using UnityEngine;

/// <summary>
/// 给相机添加摇晃效果
/// </summary>
public class CameraShake : MonoBehaviour
{
    public float Frequency = 25f;
    public Vector3 MaximumTranslationShake = Vector3.one * 0.5f;

    private float _seed;

    private void OnEnable()
    {
        _seed = UnityEngine.Random.value;
    }

    private void Update()
    {
        var timeFrequency = Time.time * Frequency;

        transform.localPosition = new Vector3(
            (Mathf.PerlinNoise(_seed, timeFrequency) * 2 - 1) * MaximumTranslationShake.x,
            (Mathf.PerlinNoise(_seed + 1, timeFrequency) * 2 - 1) * MaximumTranslationShake.y,
            transform.localPosition.z
        );
    }


    #region 柏林噪声

    // TODO:废弃自定义柏林噪声

    [Obsolete]
    private float PerlinNoise(float xIn, float yIn)
    {
        var x = Mathf.FloorToInt(xIn) & 255;
        var y = Mathf.FloorToInt(yIn) & 255;

        xIn -= Mathf.Floor(xIn);
        yIn -= Mathf.Floor(yIn);

        var u = Fade(xIn);
        var v = Fade(yIn);

        var a = (P[x] + y) & 255;
        var b = (P[x + 1] + y) & 255;

        return Mathf.Lerp(
            v,
            Mathf.Lerp(u, Grad(P[a], xIn, yIn), Grad(P[b], xIn - 1, yIn)),
            Mathf.Lerp(u, Grad(P[a + 1], xIn, yIn - 1), Grad(P[b + 1], xIn - 1, yIn - 1))
            );
    }

    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Grad(int hash, float x, float y)
    {
        var h = hash & 15;
        var u = h < 8 ? x : y;
        var v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    private static readonly int[] P = new int[512];
    private static readonly int[] Permutation = { 151,160,137,91,90,15, // Hash lookup table as defined by Ken Perlin.  This is a randomly
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
        8,99,37,240,21,10,23,190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,
        32,57,177,33,88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,
        166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,102,
        143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,135,130,116,
        188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,5,202,38,147,118,126,
        255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,223,183,170,213,119,248,152, 2,
        44,154,163, 70,221,153,101,155,167, 43,172,9,129,22,39,253,19,98,108,110,79,113,224,
        232,178,185, 112,104,218,246,97,228,251,34,242,193,238,210,144,12,191,179,162,241, 81,
        51,145,235,249,14,239,107,49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,
        45,127, 4,150,254,138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,
        180
    };

    static CameraShake()
    {
        for (var i = 0; i < 256; i++)
            P[256 + i] = P[i] = Permutation[i];
    }

    #endregion
}
