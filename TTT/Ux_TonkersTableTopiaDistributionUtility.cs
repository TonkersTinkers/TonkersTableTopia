using System;
using UnityEngine;

internal static class Ux_TonkersTableTopiaDistributionUtility
{
    private static float[] _fixedScratch = Array.Empty<float>();
    private static float[] _percentageScratch = Array.Empty<float>();
    private static int[] _flexScratch = Array.Empty<int>();

    public static void DistributeInto(int count, Func<int, float> getSpec, float spacing, float inner, ref float[] result)
    {
        if (count <= 0)
        {
            result = Array.Empty<float>();
            return;
        }

        if (result == null || result.Length != count)
        {
            result = new float[count];
        }

        DistributeCore(count, getSpec, spacing, inner, result);
    }

    public static float[] Distribute(int count, Func<int, float> getSpec, float spacing, float inner)
    {
        if (count <= 0)
        {
            return Array.Empty<float>();
        }

        float[] result = new float[count];
        DistributeCore(count, getSpec, spacing, inner, result);
        return result;
    }

    public static float[] ComputeNormalizedPercentages(int count, Func<int, float> getResolvedSpec, float spacing, float inner)
    {
        if (count <= 0)
        {
            return Array.Empty<float>();
        }

        float[] pixels = Distribute(count, getResolvedSpec, spacing, inner);
        float available = Mathf.Max(1f, inner - spacing * Mathf.Max(0, count - 1));
        float[] percentages = new float[count];
        float sum = 0f;

        for (int i = 0; i < count; i++)
        {
            percentages[i] = Mathf.Clamp01(pixels[i] / available);
            sum += percentages[i];
        }

        if (sum > 1.0001f)
        {
            float inverse = 1f / sum;
            for (int i = 0; i < count; i++)
            {
                percentages[i] *= inverse;
            }
        }

        return percentages;
    }

    public static void NormalizeNegativePercentageSpecs(int count, Func<int, float> getSpec, Action<int, float> setSpec)
    {
        if (count <= 0 || getSpec == null || setSpec == null)
        {
            return;
        }

        float sum = 0f;
        for (int i = 0; i < count; i++)
        {
            float value = getSpec(i);
            if (value < 0f)
            {
                sum += -value;
            }
        }

        if (sum <= 1.0001f || sum <= 0f)
        {
            return;
        }

        float inverse = 1f / sum;
        for (int i = 0; i < count; i++)
        {
            float value = getSpec(i);
            if (value < 0f)
            {
                setSpec(i, -Mathf.Clamp01((-value) * inverse));
            }
        }
    }

    public static void RebalancePair(ref float leftPct, ref float rightPct, float deltaPct, float floor)
    {
        float sum = Mathf.Clamp01(leftPct + rightPct);
        float minLeft = floor;
        float maxLeft = Mathf.Max(floor, sum - floor);
        float newLeft = Mathf.Clamp(leftPct + deltaPct, minLeft, maxLeft);
        float newRight = sum - newLeft;

        leftPct = newLeft;
        rightPct = newRight;
    }

    private static void DistributeCore(int count, Func<int, float> getSpec, float spacing, float inner, float[] result)
    {
        EnsureScratchCapacity(count);

        for (int i = 0; i < count; i++)
        {
            _fixedScratch[i] = 0f;
            _percentageScratch[i] = 0f;
            _flexScratch[i] = 0;
            result[i] = 0f;
        }

        float fixedSum = 0f;
        float percentageSum = 0f;
        int flexCount = 0;

        for (int i = 0; i < count; i++)
        {
            float value = getSpec != null ? getSpec(i) : 0f;

            if (value > 0f)
            {
                _fixedScratch[i] = value;
                fixedSum += value;
                continue;
            }

            if (value < 0f)
            {
                float percent = Mathf.Clamp01(-value);
                _percentageScratch[i] = percent;
                percentageSum += percent;
                continue;
            }

            _flexScratch[i] = 1;
            flexCount++;
        }

        float remaining = Mathf.Max(0f, inner - spacing * Mathf.Max(0, count - 1) - fixedSum);
        float usedByPercentages = 0f;

        if (percentageSum > 0f)
        {
            float normalize = percentageSum > 1f ? 1f / percentageSum : 1f;
            for (int i = 0; i < count; i++)
            {
                if (_percentageScratch[i] <= 0f)
                {
                    continue;
                }

                float allocation = remaining * (_percentageScratch[i] * normalize);
                result[i] = _fixedScratch[i] + allocation;
                usedByPercentages += allocation;
            }
        }

        float residual = Mathf.Max(0f, remaining - usedByPercentages);

        if (flexCount > 0)
        {
            float share = residual / flexCount;
            for (int i = 0; i < count; i++)
            {
                if (result[i] > 0f)
                {
                    continue;
                }

                result[i] = _fixedScratch[i] + share;
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                if (result[i] > 0f)
                {
                    continue;
                }

                result[i] = _fixedScratch[i];
            }
        }

        for (int i = 0; i < count; i++)
        {
            if (result[i] < 0f)
            {
                result[i] = 0f;
            }
        }
    }

    private static void EnsureScratchCapacity(int count)
    {
        if (_fixedScratch.Length < count)
        {
            Array.Resize(ref _fixedScratch, count);
        }

        if (_percentageScratch.Length < count)
        {
            Array.Resize(ref _percentageScratch, count);
        }

        if (_flexScratch.Length < count)
        {
            Array.Resize(ref _flexScratch, count);
        }
    }
}