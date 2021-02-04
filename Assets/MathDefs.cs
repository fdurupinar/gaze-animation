using System;
using System.Collections.Generic;

public static class MathDefs
{
    private static System.Random rand = new System.Random();

    public static float Percentile(List<float> sequence, float excelPercentile)
    {
        sequence.Sort();
        int N = sequence.Count;
        float n = (N - 1) * excelPercentile + 1;
        // Another method: double n = (N + 1) * excelPercentile;
        if (n == 1d) return sequence[0];
        else if (n == N) return sequence[N - 1];
        else
        {
            int k = (int)n;
            float d = n - k;
            return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
        }
    }

    public static int GetRandomNumber(int max) {
        return rand.Next(max);
    }

    public static float GetRandomNumber(float max) {
        return (float)rand.NextDouble() * max;
    }
    public static int GetRandomNumber(int min, int max) {
        return rand.Next(min, max);
    }
    public static float GetRandomNumber(float min, float max) {
        return min + (float)rand.NextDouble() * (max - min);
    }

    /// <summary>
	/// Gaussian Distribution with Box-Muller transform
	/// </summary>
	public static float GaussianDist(float mean, float std) {

        double u1 = rand.NextDouble(); //uniform(0,1) random doubles
        double u2 = rand.NextDouble();
        float randStdNormal = (float)Math.Sqrt(-2.0 * Math.Log(u1)) * (float)Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        float randNormal = mean + std * randStdNormal; //random normal(mean,stdDev^2)						
        return randNormal;

    }

}