namespace CarnetDigital.Frontend.Helpers;

public static class LoginAttemptHelper
{
    private static readonly Dictionary<string, int> FailedAttempts = new();

    private static readonly HashSet<string> BlockedUsers = new();

    public static bool IsBlocked(string username)
    {
        return BlockedUsers.Contains(username.ToLower());
    }

    public static void RegisterFailedAttempt(string username)
    {
        username = username.ToLower();

        if (!FailedAttempts.ContainsKey(username))
            FailedAttempts[username] = 0;

        FailedAttempts[username]++;

        if (FailedAttempts[username] >= 3)
        {
            BlockedUsers.Add(username);
        }
    }

    public static void ResetAttempts(string username)
    {
        username = username.ToLower();

        FailedAttempts.Remove(username);
    }
}
