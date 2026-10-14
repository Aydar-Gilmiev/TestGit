using System.Collections.Generic;

namespace GitTestAvalonia.Models;

public class User
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public static class UserStorage
{
    public static List<User> Users { get; } = new();
}
