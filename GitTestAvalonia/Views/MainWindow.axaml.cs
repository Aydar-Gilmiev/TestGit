using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Interactivity;
using Avalonia.Media;
using GitTestAvalonia.Models;

namespace GitTestAvalonia.Views;

public partial class MainWindow : Avalonia.Controls.Window
{
    private static readonly Regex EmailPattern = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public MainWindow()
    {
        InitializeComponent();
        UpdateUserCount();
    }

    private void RegisterButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var fullName = FullNameTextBox.Text?.Trim() ?? string.Empty;
        var email = EmailTextBox.Text?.Trim().ToLowerInvariant() ?? string.Empty;
        var password = PasswordTextBox.Text ?? string.Empty;
        var confirmation = ConfirmPasswordTextBox.Text ?? string.Empty;

        var validationMessage = Validate(fullName, email, password, confirmation);
        if (validationMessage is not null)
        {
            ShowError(validationMessage);
            return;
        }

        if (UserStorage.Users.Any(user =>
                string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)))
        {
            ShowError("Пользователь с таким email уже зарегистрирован в текущем сеансе.");
            return;
        }

        UserStorage.Users.Add(new User
        {
            FullName = fullName,
            Email = email
        });

        StatusTextBlock.Foreground = new SolidColorBrush(Color.Parse("#067647"));
        StatusTextBlock.Text = "Регистрация выполнена. Пользователь сохранён в памяти приложения.";
        FullNameTextBox.Text = string.Empty;
        EmailTextBox.Text = string.Empty;
        PasswordTextBox.Text = string.Empty;
        ConfirmPasswordTextBox.Text = string.Empty;
        UpdateUserCount();
        FullNameTextBox.Focus();
    }

    private static string? Validate(string fullName, string email, string password, string confirmation)
    {
        if (fullName.Length < 2)
            return "Введите имя длиной не менее двух символов.";
        if (!EmailPattern.IsMatch(email))
            return "Введите корректный адрес электронной почты.";
        if (password.Length < 8)
            return "Пароль должен содержать не менее восьми символов.";
        if (password != confirmation)
            return "Пароли не совпадают.";

        return null;
    }

    private void ShowError(string message)
    {
        StatusTextBlock.Foreground = new SolidColorBrush(Color.Parse("#B42318"));
        StatusTextBlock.Text = message;
    }

    private void UpdateUserCount()
    {
        UserCountTextBlock.Text = $"Пользователей в текущем сеансе: {UserStorage.Users.Count}";
    }
}
