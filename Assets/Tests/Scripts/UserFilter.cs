public static class UserFilter
{
    public static bool PassesFilters(User user)
    {
        if (FilterSettings.AgeFilter > 0 && user.age < FilterSettings.AgeFilter) return false;
        if (FilterSettings.WeightFilter > 0 && user.weight < FilterSettings.WeightFilter) return false;
        if (FilterSettings.HeightFilter > 0 && user.height < FilterSettings.HeightFilter) return false;
        if (FilterSettings.AlchoholIntakeFilter > 0 && user.alchohol_intake < FilterSettings.AlchoholIntakeFilter) return false;

        if (FilterSettings.GenderFilter != Genders.None &&
            !string.Equals(user.gender ?? "", FilterSettings.GenderFilter.ToString(), System.StringComparison.OrdinalIgnoreCase))
            return false;

        if (FilterSettings.MoodFilter != Moods.None)
        {
            if (FilterSettings.MoodFilter == Moods.Depression && !(user.depression > 0f)) return false;
            if (FilterSettings.MoodFilter == Moods.Burnout && !(user.burnout > 0f)) return false;
        }

        return true;
    }
}

