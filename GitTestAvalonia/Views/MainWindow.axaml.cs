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
            Email = email,
            Password = password
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

    private void LoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var email = LoginEmailTextBox.Text?.Trim().ToLowerInvariant() ?? string.Empty;
        var password = LoginPasswordTextBox.Text ?? string.Empty;

        if (!EmailPattern.IsMatch(email))
        {
            ShowLoginError("Введите корректный адрес электронной почты.");
            return;
        }

        if (password.Length == 0)
        {
            ShowLoginError("Введите пароль.");
            return;
        }

        var user = UserStorage.Users.FirstOrDefault(candidate =>
            string.Equals(candidate.Email, email, StringComparison.OrdinalIgnoreCase));

        if (user is null || user.Password != password)
        {
            ShowLoginError("Неверный email или пароль.");
            return;
        }

        OpenHomeWindow(user);
    }

    private void SwitchToLoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RegistrationPanel.IsVisible = false;
        StatusTextBlock.IsVisible = false;
        RegisterButton.IsVisible = false;
        SwitchToLoginButton.IsVisible = false;
        LoginPanel.IsVisible = true;
        FormTitleTextBlock.Text = "Вход";
        FormSubtitleTextBlock.Text = "Введите данные зарегистрированного пользователя";
        Title = "Вход";
        LoginEmailTextBox.Focus();
    }

    private void SwitchToRegistrationButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RegistrationPanel.IsVisible = true;
        StatusTextBlock.IsVisible = true;
        RegisterButton.IsVisible = true;
        SwitchToLoginButton.IsVisible = true;
        LoginPanel.IsVisible = false;
        FormTitleTextBlock.Text = "Регистрация";
        FormSubtitleTextBlock.Text = "Заполните данные нового пользователя";
        Title = "Регистрация";
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

    private void ShowLoginError(string message)
    {
        LoginStatusTextBlock.Foreground = new SolidColorBrush(Color.Parse("#B42318"));
        LoginStatusTextBlock.Text = message;
    }

    private void UpdateUserCount()
    {
        UserCountTextBlock.Text = $"Пользователей в текущем сеансе: {UserStorage.Users.Count}";
    }

    private void OpenHomeWindow(User user)
    {
        var homeWindow = new HomeWindow(user);

        homeWindow.LogoutRequested += (_, _) =>
        {
            LoginPasswordTextBox.Text = string.Empty;
            LoginStatusTextBlock.Text = string.Empty;
            Show();
            LoginEmailTextBox.Focus();
        };

        homeWindow.Closed += (_, _) =>
        {
            if (!IsVisible)
                Close();
        };

        homeWindow.Show();
        Hide();
    }
}
