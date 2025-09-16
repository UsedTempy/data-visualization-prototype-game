using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Mesh meshTree1;
    [SerializeField] private Mesh meshTree2;
    [SerializeField] private Mesh meshTree3;
    [SerializeField] private Mesh meshTree4;
    [SerializeField] private Mesh meshTree5;
    [SerializeField] private Mesh meshTreeDepression;
    [SerializeField] private Mesh meshTreeBurnout;
    [SerializeField] private Material materialTree;
    [SerializeField] private Transform gridParent;

    private List<Matrix4x4>[] stageMatrices;
    private List<Matrix4x4>[] filteredMatrices;

    private RenderParams treeRP;

    private int xCount = 185;
    private int zCount = 185;
    private float spacing = 1.55f;

    DataAPI api = new DataAPI("http://localhost:3069");
    User[] users;

    private float lastAgeFilter, lastWeightFilter, lastHeightFilter, lastAlcoholFilter;
    private Genders lastGenderFilter;
    private Moods lastMoodFilter;

    private List<User>[] stageUsers;

    void Start()
    {
        StartCoroutine(api.GetRequest("/retrieveAll", (_users) =>
        {
            if (_users != null)
            {
                users = _users;
                InitializeGame();
            }
            else
            {
                Debug.LogError("Failed to load trees");
            }
        }));
    }

    private bool FiltersChanged()
    {
        return FilterSettings.AgeFilter != lastAgeFilter ||
            FilterSettings.WeightFilter != lastWeightFilter ||
            FilterSettings.HeightFilter != lastHeightFilter ||
            FilterSettings.AlchoholIntakeFilter != lastAlcoholFilter ||
            FilterSettings.GenderFilter != lastGenderFilter ||
            FilterSettings.MoodFilter != lastMoodFilter;
    }

    private void UpdateFilterTracking()
    {
        lastAgeFilter = FilterSettings.AgeFilter;
        lastWeightFilter = FilterSettings.WeightFilter;
        lastHeightFilter = FilterSettings.HeightFilter;
        lastAlcoholFilter = FilterSettings.AlchoholIntakeFilter;
        lastGenderFilter = FilterSettings.GenderFilter;
        lastMoodFilter = FilterSettings.MoodFilter;
    }

    public bool PassesFilters(User user)
    {
        // Age filter
        if (FilterSettings.AgeFilter > 0 && user.age < FilterSettings.AgeFilter)
            return false;

        // Weight filter
        if (FilterSettings.WeightFilter > 0 && user.weight < FilterSettings.WeightFilter)
            return false;

        // Height filter
        if (FilterSettings.HeightFilter > 0 && user.height < FilterSettings.HeightFilter)
            return false;

        // Alcohol intake filter
        if (FilterSettings.AlchoholIntakeFilter > 0 && user.alchohol_intake < FilterSettings.AlchoholIntakeFilter)
            return false;

        // Gender filter
        if (FilterSettings.GenderFilter != Genders.None && user.gender.ToUpper() != FilterSettings.GenderFilter.ToString().ToUpper())
            return false;

        // Mood filter
        if (FilterSettings.MoodFilter != Moods.None)
        {
            if (FilterSettings.MoodFilter == Moods.Depression && !(user.depression > 0f))
                return false;
            if (FilterSettings.MoodFilter == Moods.Burnout && !(user.burnout > 0f))
                return false;
        }

        return true; // Passed all active filters
    }


    private void InitializeGame()
    {
        Vector3[,] gridPositions = GridGenerator.GenerateGridPositions(xCount, zCount, spacing, spacing, gridParent);

        stageMatrices = new List<Matrix4x4>[7];
        stageUsers = new List<User>[7];

        for (int s = 0; s < stageMatrices.Length; s++)
        {
            stageMatrices[s] = new List<Matrix4x4>();
            stageUsers[s] = new List<User>();   
        }

        for (int z = 0; z < gridPositions.GetLength(0); z++)
        {
            for (int x = 0; x < gridPositions.GetLength(1); x++)
            {
                int i = z * gridPositions.GetLength(1) + x;
                User user = users[i];

                if (!PassesFilters(user))
                    continue; // Skip this tree

                Vector3 pos = gridPositions[z, x] + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                float scale = GameCalculations.MapExponential(user.height);
                Vector3 size = GameCalculations.ScaleToVector3(scale);
                Matrix4x4 trs = Matrix4x4.TRS(pos, rot, size);

                int stage = GameCalculations.GetStageFromAge(user.age) - 1; // 0-4
                if (user.depression > 0f) stage = 5;
                else if (user.burnout > 0f) stage = 6;

                stageMatrices[stage].Add(trs);
                stageUsers[stage].Add(user); // Keep user aligned with its TRS
            }
        }

        treeRP = new RenderParams(materialTree);
    }

    private void RenderStage(Mesh mesh, List<Matrix4x4> matrices)
    {
        int batchSize = 1023;
        for (int i = 0; i < matrices.Count; i += batchSize)
        {
            int count = Mathf.Min(batchSize, matrices.Count - i);
            Graphics.RenderMeshInstanced(treeRP, mesh, 0, matrices.GetRange(i, count).ToArray());
        }
    }

    private void Update()
    {
        if (stageMatrices == null) return;

        // Only rebuild filtered matrices if filters have changed
        if (filteredMatrices == null || FiltersChanged())
        {
            // Initialize filtered matrices
            filteredMatrices = new List<Matrix4x4>[7];
            for (int s = 0; s < 7; s++)
                filteredMatrices[s] = new List<Matrix4x4>();

            // Apply filters
            for (int s = 0; s < 7; s++)
            {
                for (int i = 0; i < stageMatrices[s].Count; i++)
                {
                    User user = stageUsers[s][i]; // Correct user for this matrix
                    if (PassesFilters(user))
                        filteredMatrices[s].Add(stageMatrices[s][i]);
                }
            }

            // Update filter tracking values
            UpdateFilterTracking();
        }

        // Render filtered matrices (every frame)
        RenderStage(meshTree1, filteredMatrices[0]);
        RenderStage(meshTree2, filteredMatrices[1]);
        RenderStage(meshTree3, filteredMatrices[2]);
        RenderStage(meshTree4, filteredMatrices[3]);
        RenderStage(meshTree5, filteredMatrices[4]);
        RenderStage(meshTreeDepression, filteredMatrices[5]);
        RenderStage(meshTreeBurnout, filteredMatrices[6]);
    }

}
