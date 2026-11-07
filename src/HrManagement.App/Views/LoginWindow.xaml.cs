using System.Windows;
using HrManagement.App.Services;

namespace HrManagement.App.Views;

public partial class LoginWindow : Window
{
    private readonly AuthService _authService;

    public LoginWindow(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    public UserSession? Session { get; private set; }

    private async void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = string.Empty;
        SignInButton.IsEnabled = false;

        try
        {
            Session = await _authService.SignInAsync(LoginBox.Text, PasswordBox.Password);

            if (Session is null)
            {
                ErrorText.Text = "Неверный логин или пароль либо учётная запись отключена.";
                return;
            }

            DialogResult = true;
        }
        catch (Exception exception)
        {
            ErrorText.Text = exception.Message;
        }
        finally
        {
            SignInButton.IsEnabled = true;
        }
    }
}
