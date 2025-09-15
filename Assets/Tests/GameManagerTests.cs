using NUnit.Framework;
using UnityEngine;

public class GameManagerLogicTests
{
    private GameManagerLogic logic;

    [SetUp]
    public void Setup()
    {
        logic = new GameManagerLogic();

        FilterSettings.AgeFilter = 0;
        FilterSettings.WeightFilter = 0;
        FilterSettings.HeightFilter = 0;
        FilterSettings.AlchoholIntakeFilter = 0;
        FilterSettings.GenderFilter = Genders.None;
        FilterSettings.MoodFilter = Moods.None;
    }

    private User CreateUser(
        int age = 25,
        float weight = 70,
        float height = 180,
        float alcohol = 1,
        string gender = "Male",
        float depression = 0f,
        float burnout = 0f)
    {
        return new User
        {
            age = age,
            weight = weight,
            height = height,
            alchohol_intake = alcohol,
            gender = gender ?? "Male",
            depression = depression,
            burnout = burnout
        };
    }

    [Test]
    public void PassesFilters_ReturnsTrue_WhenNoFilters()
    {
        var user = CreateUser();
        Assert.IsTrue(logic.PassesFilters(user));
    }

    [Test]
    public void PassesFilters_Fails_AgeFilter()
    {
        FilterSettings.AgeFilter = 30;
        var user = CreateUser(age: 25);
        Assert.IsFalse(logic.PassesFilters(user));
    }

    [Test]
    public void PassesFilters_Fails_WeightFilter()
    {
        FilterSettings.WeightFilter = 75;
        var user = CreateUser(weight: 70);
        Assert.IsFalse(logic.PassesFilters(user));
    }

    [Test]
    public void PassesFilters_Fails_HeightFilter()
    {
        FilterSettings.HeightFilter = 185;
        var user = CreateUser(height: 180);
        Assert.IsFalse(logic.PassesFilters(user));
    }

    [Test]
    public void PassesFilters_Fails_AlcoholFilter()
    {
        FilterSettings.AlchoholIntakeFilter = 5;
        var user = CreateUser(alcohol: 3);
        Assert.IsFalse(logic.PassesFilters(user));
    }

    [Test]
    public void PassesFilters_Fails_GenderFilter()
    {
        FilterSettings.GenderFilter = Genders.Female;
        var user = CreateUser(gender: "Male");
        Assert.IsFalse(logic.PassesFilters(user));
    }

    [Test]
    public void PassesFilters_Fails_MoodFilter_Depression()
    {
        FilterSettings.MoodFilter = Moods.Depression;
        var user = CreateUser(depression: 0f);
        Assert.IsFalse(logic.PassesFilters(user));
    }

    [Test]
    public void PassesFilters_Passes_MoodFilter_Depression()
    {
        FilterSettings.MoodFilter = Moods.Depression;
        var user = CreateUser(depression: 1f);
        Assert.IsTrue(logic.PassesFilters(user));
    }

    [Test]
    public void FiltersChanged_ReturnsTrue_WhenFiltersChanged()
    {
        logic.UpdateFilterTracking();
        FilterSettings.AgeFilter = 30;
        Assert.IsTrue(logic.FiltersChanged());
    }

    [Test]
    public void FiltersChanged_ReturnsFalse_WhenFiltersUnchanged()
    {
        FilterSettings.AgeFilter = 25;
        logic.UpdateFilterTracking();
        Assert.IsFalse(logic.FiltersChanged());
    }

    [Test]
    public void InitializeGame_PopulatesStageUsersAndMatrices()
    {
        User[] testUsers = new User[10];
        for (int i = 0; i < 10; i++)
            testUsers[i] = CreateUser(age: 20 + i, depression: (i == 0 ? 1f : 0f));
        logic.users = testUsers;

        Vector3[,] gridPositions = new Vector3[2,5];
        for (int z = 0; z < 2; z++)
            for (int x = 0; x < 5; x++)
                gridPositions[z,x] = new Vector3(x,0,z);

        logic.InitializeGame(gridPositions);

        Assert.IsNotNull(logic.stageUsers);
        Assert.IsNotNull(logic.stageMatrices);
        Assert.AreEqual(7, logic.stageUsers.Length);
        Assert.AreEqual(7, logic.stageMatrices.Length);

        int totalUsers = 0;
        foreach(var list in logic.stageUsers) totalUsers += list.Count;
        int totalMatrices = 0;
        foreach(var list in logic.stageMatrices) totalMatrices += list.Count;

        Assert.Greater(totalUsers, 0);
        Assert.AreEqual(totalUsers, totalMatrices, "Each user should have a corresponding TRS matrix");
    }

    [Test]
    public void ApplyFilters_RemovesUsersNotPassingFilters()
    {
        logic.users = new User[]
        {
            CreateUser(age: 20),
            CreateUser(age: 40),
        };

        Vector3[,] gridPositions = new Vector3[1,2];
        gridPositions[0,0] = Vector3.zero;
        gridPositions[0,1] = Vector3.one;

        logic.InitializeGame(gridPositions);

        FilterSettings.AgeFilter = 30;
        logic.ApplyFilters();

        foreach (var list in logic.stageUsers)
        {
            foreach (var user in list)
            {
                Assert.GreaterOrEqual(user.age, 30);
            }
        }
    }
}
