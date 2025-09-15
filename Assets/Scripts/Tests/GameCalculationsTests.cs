using NUnit.Framework;
using UnityEngine;

public static class GameCalculations
{
    public static float MapExponential(float value, float min = 120f, float max = 210f, float exponent = 2f)
    {
        value = Mathf.Clamp(value, min, max);
        float t = (value - min) / (max - min);

        return Mathf.Pow(t, exponent);
    }

    public static Vector3 ScaleToVector3(float scale)
    {
        return new Vector3(scale, scale, scale);
    }

    public static int GetStageFromAge(float age, float min = 20f, float max = 100f, int stages = 5)
    {
        age = Mathf.Clamp(age, min, max);
        float stageSize = (max - min) / stages;

        int stage = Mathf.FloorToInt((age - min) / stageSize);
        if (stage >= stages) stage = stages - 1;

        return stage + 1;
    }
}

public class AgeMatrixData
{
    public int AgeStage;       // 1–5
    public Matrix4x4 Matrix;   // Transformation matrix

    public AgeMatrixData(int stage, Matrix4x4 matrix)
    {
        AgeStage = stage;
        Matrix = matrix;
    }
}

    public class GameCalculationsTests
    {
        [Test]
        public void MapExponential_ClampsAndScalesCorrectly()
        {
            float result = GameCalculations.MapExponential(150f, 120f, 210f, 2f);

            // 150 is between 120 and 210, expect value between 0 and 1
            Assert.That(result, Is.GreaterThanOrEqualTo(0f).And.LessThanOrEqualTo(1f));
        }

        [Test]
        public void MapExponential_BelowMin_ClampsToMin()
        {
            float result = GameCalculations.MapExponential(50f, 120f, 210f, 2f);

            // 50 < 120, so it clamps to 120 → t = 0 → pow(0, exp) = 0
            Assert.AreEqual(0f, result);
        }

        [Test]
        public void ScaleToVector3_ReturnsUniformScale()
        {
            float scale = 2f;
            Vector3 vec = GameCalculations.ScaleToVector3(scale);

            Assert.AreEqual(new Vector3(2f, 2f, 2f), vec);
        }

        [Test]
        public void GetStageFromAge_ReturnsCorrectStage()
        {
            int stage = GameCalculations.GetStageFromAge(60f, 20f, 100f, 5);

            // 20–100 split into 5 stages → each stage = 16 units wide
            // 60 is in the middle range → should be stage 3 or 4 depending on flooring
            Assert.AreEqual(3, stage);
        }

        [Test]
        public void GetStageFromAge_ClampsAboveMax()
        {
            int stage = GameCalculations.GetStageFromAge(999f, 20f, 100f, 5);

            // Should clamp to last stage
            Assert.AreEqual(5, stage);
        }
    }
