using System.Windows;
using System.Windows.Input;
using HrManagement.App.Services;
using HrManagement.App.ViewModels;

namespace HrManagement.App.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(EmployeeService employeeService, UserSession session)
    {
        InitializeComponent();
        _viewModel = new MainViewModel(employeeService, session);
        DataContext = _viewModel;
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();
    }

    private void Dashboard_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(0, "Обзор");
    }

    private void Employees_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(1, "Сотрудники");
    }

    private void Absences_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(2, "Отсутствия");
    }

    private void Reports_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(3, "Отчёты");
    }

    private void ShowPage(int index, string title)
    {
        ContentTabs.SelectedIndex = index;
        PageTitle.Text = title;
    }

    private async void AddEmployee_Click(object sender, RoutedEventArgs e)
    {
        var model = new EmployeeEditModel
        {
            DepartmentId = _viewModel.Departments.FirstOrDefault()?.Id ?? 0,
            PositionId = _viewModel.Positions.FirstOrDefault()?.Id ?? 0
        };

        await ShowEmployeeDialogAsync(model, "Новый сотрудник");
    }

    private async void EditEmployee_Click(object sender, RoutedEventArgs e)
    {
        var employee = _viewModel.SelectedEmployee?.Employee;

        if (employee is null)
        {
            MessageBox.Show("Выберите сотрудника в таблице.", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var model = new EmployeeEditModel
        {
            Id = employee.Id,
            PersonnelNumber = employee.PersonnelNumber,
            LastName = employee.LastName,
            FirstName = employee.FirstName,
            MiddleName = employee.MiddleName,
            BirthDate = employee.BirthDate.ToDateTime(TimeOnly.MinValue),
            HireDate = employee.HireDate.ToDateTime(TimeOnly.MinValue),
            Email = employee.Email,
            Phone = employee.Phone,
            DepartmentId = employee.DepartmentId,
            PositionId = employee.PositionId
        };

        await ShowEmployeeDialogAsync(model, "Карточка сотрудника");
    }

    private async Task ShowEmployeeDialogAsync(EmployeeEditModel model, string title)
    {
        var dialog = new EmployeeDialog(model, _viewModel.Departments, _viewModel.Positions)
        {
            Owner = this,
            Title = title
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            await _viewModel.SaveEmployeeAsync(model);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void ResetSearch_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SearchText = string.Empty;
        await _viewModel.RefreshAsync();
    }

    private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.F1:
                ShowPage(0, "Обзор");
                break;
            case Key.F2:
                ShowPage(1, "Сотрудники");
                break;
            case Key.F3:
                ShowPage(2, "Отсутствия");
                break;
            case Key.F4:
                ShowPage(3, "Отчёты");
                break;
            case Key.F5:
                AddEmployee_Click(this, new RoutedEventArgs());
                break;
            default:
                return;
        }

        e.Handled = true;
    }
}
