using NUnit.Framework;
using UnityEngine;

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