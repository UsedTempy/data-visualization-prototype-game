[System.Serializable]
public class User
{
    public float age;
    public string gender;
    public float weight;
    public float height;
    public float depression = -1;
    public float burnout = -1;
    public float alchohol_intake;
}

[System.Serializable]
public class UserResponse
{
    public bool success;
    public User[] user;
}