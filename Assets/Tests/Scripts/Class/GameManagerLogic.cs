using System.Collections.Generic;
using UnityEngine;

// ---------------------------
// Pure logic class for testing
// ---------------------------
public class GameManagerLogic
{
    public User[] users;
    public List<User>[] stageUsers;
    public List<Matrix4x4>[] stageMatrices;

    // Filter tracking values
    public float lastAgeFilter, lastWeightFilter, lastHeightFilter, lastAlcoholFilter;
    public Genders lastGenderFilter;
    public Moods lastMoodFilter;

    public GameManagerLogic() 
    {
        stageUsers = new List<User>[7];
        stageMatrices = new List<Matrix4x4>[7];
        for (int i = 0; i < 7; i++)
        {
            stageUsers[i] = new List<User>();
            stageMatrices[i] = new List<Matrix4x4>();
        }
    }

    public bool PassesFilters(User user)
    {
        if (FilterSettings.AgeFilter > 0 && user.age < FilterSettings.AgeFilter)
            return false;
        if (FilterSettings.WeightFilter > 0 && user.weight < FilterSettings.WeightFilter)
            return false;
        if (FilterSettings.HeightFilter > 0 && user.height < FilterSettings.HeightFilter)
            return false;
        if (FilterSettings.AlchoholIntakeFilter > 0 && user.alchohol_intake < FilterSettings.AlchoholIntakeFilter)
            return false;
        if (FilterSettings.GenderFilter != Genders.None && user.gender.ToUpper() != FilterSettings.GenderFilter.ToString().ToUpper())
            return false;
        if (FilterSettings.MoodFilter != Moods.None)
        {
            if (FilterSettings.MoodFilter == Moods.Depression && user.depression <= 0f)
                return false;
            if (FilterSettings.MoodFilter == Moods.Burnout && user.burnout <= 0f)
                return false;
        }
        return true;
    }

    public bool FiltersChanged()
    {
        return FilterSettings.AgeFilter != lastAgeFilter ||
               FilterSettings.WeightFilter != lastWeightFilter ||
               FilterSettings.HeightFilter != lastHeightFilter ||
               FilterSettings.AlchoholIntakeFilter != lastAlcoholFilter ||
               FilterSettings.GenderFilter != lastGenderFilter ||
               FilterSettings.MoodFilter != lastMoodFilter;
    }

    public void UpdateFilterTracking()
    {
        lastAgeFilter = FilterSettings.AgeFilter;
        lastWeightFilter = FilterSettings.WeightFilter;
        lastHeightFilter = FilterSettings.HeightFilter;
        lastAlcoholFilter = FilterSettings.AlchoholIntakeFilter;
        lastGenderFilter = FilterSettings.GenderFilter;
        lastMoodFilter = FilterSettings.MoodFilter;
    }

    public void InitializeGame(Vector3[,] gridPositions)
    {
        for (int z = 0; z < gridPositions.GetLength(0); z++)
        {
            for (int x = 0; x < gridPositions.GetLength(1); x++)
            {
                int i = z * gridPositions.GetLength(1) + x;
                if (i >= users.Length) continue;
                User user = users[i];

                if (!PassesFilters(user))
                    continue;

                Vector3 pos = gridPositions[z, x] + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                float scale = GameCalculations.MapExponential(user.height);
                Vector3 size = GameCalculations.ScaleToVector3(scale);
                Matrix4x4 trs = Matrix4x4.TRS(pos, rot, size);

                int stage = GameCalculations.GetStageFromAge(user.age) - 1;
                if (user.depression > 0f) stage = 5;
                else if (user.burnout > 0f) stage = 6;

                stageMatrices[stage].Add(trs);
                stageUsers[stage].Add(user);
            }
        }
    }

    public void ApplyFilters()
    {
        for (int s = 0; s < 7; s++)
        {
            List<Matrix4x4> newFilteredMatrices = new List<Matrix4x4>();
            List<User> newFilteredUsers = new List<User>();

            for (int i = 0; i < stageUsers[s].Count; i++)
            {
                if (PassesFilters(stageUsers[s][i]))
                {
                    newFilteredUsers.Add(stageUsers[s][i]);
                    newFilteredMatrices.Add(stageMatrices[s][i]);
                }
            }

            stageUsers[s] = newFilteredUsers;
            stageMatrices[s] = newFilteredMatrices;
        }

        UpdateFilterTracking();
    }
}