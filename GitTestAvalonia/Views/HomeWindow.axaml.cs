using System;
using System.Globalization;
using System.Linq;
using Avalonia.Interactivity;
using GitTestAvalonia.Models;

namespace GitTestAvalonia.Views;

public partial class HomeWindow : Avalonia.Controls.Window
{
    public event EventHandler? LogoutRequested;

    public HomeWindow()
    {
        InitializeComponent();
    }

    public HomeWindow(User user) : this()
    {

        GreetingTextBlock.Text = $"Добро пожаловать, {GetFirstName(user.FullName)}!";
        FullNameTextBlock.Text = user.FullName;
        EmailTextBlock.Text = user.Email;
        AvatarTextBlock.Text = GetInitials(user.FullName);
        CurrentDateTextBlock.Text = DateTime.Now.ToString(
            "d MMMM yyyy",
            CultureInfo.GetCultureInfo("ru-RU"));
    }

    private void LogoutButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke(this, EventArgs.Empty);
        Close();
    }

    private static string GetFirstName(string fullName)
    {
        return fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
               ?? "пользователь";
    }

    private static string GetInitials(string fullName)
    {
        var words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(words.Take(2).Select(word => char.ToUpperInvariant(word[0])));
    }
}
