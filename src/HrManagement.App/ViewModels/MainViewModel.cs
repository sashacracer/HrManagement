using System.Collections.ObjectModel;
using System.Windows;
using HrManagement.App.Models;
using HrManagement.App.Services;

namespace HrManagement.App.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly EmployeeService _employeeService;
    private readonly UserSession _session;
    private string _searchText = string.Empty;
    private EmployeeRowViewModel? _selectedEmployee;
    private int _employeeCount;
    private int _departmentCount;
    private int _absenceCount;
    private bool _isBusy;

    public MainViewModel(EmployeeService employeeService, UserSession session)
    {
        _employeeService = employeeService;
        _session = session;
        RefreshCommand = new AsyncCommand(RefreshAsync);
        DismissCommand = new AsyncCommand(DismissAsync, () => SelectedEmployee is not null);
    }

    public ObservableCollection<EmployeeRowViewModel> Employees { get; } = [];

    public IReadOnlyList<Department> Departments { get; private set; } = [];

    public IReadOnlyList<Position> Positions { get; private set; } = [];

    public string CurrentUser => _session.DisplayName;

    public string CurrentRole => _session.Role;

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public EmployeeRowViewModel? SelectedEmployee
    {
        get => _selectedEmployee;
        set
        {
            if (SetProperty(ref _selectedEmployee, value))
            {
                DismissCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public int EmployeeCount
    {
        get => _employeeCount;
        private set => SetProperty(ref _employeeCount, value);
    }

    public int DepartmentCount
    {
        get => _departmentCount;
        private set => SetProperty(ref _departmentCount, value);
    }

    public int AbsenceCount
    {
        get => _absenceCount;
        private set => SetProperty(ref _absenceCount, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public AsyncCommand RefreshCommand { get; }

    public AsyncCommand DismissCommand { get; }

    public async Task InitializeAsync()
    {
        Departments = await _employeeService.GetDepartmentsAsync();
        Positions = await _employeeService.GetPositionsAsync();
        OnPropertyChanged(nameof(Departments));
        OnPropertyChanged(nameof(Positions));
        await RefreshAsync();
    }

    public async Task RefreshAsync()
    {
        IsBusy = true;

        try
        {
            var employees = await _employeeService.GetEmployeesAsync(SearchText);
            Employees.Clear();

            foreach (var employee in employees)
            {
                Employees.Add(new EmployeeRowViewModel(employee));
            }

            var dashboard = await _employeeService.GetDashboardAsync();
            EmployeeCount = dashboard.Employees;
            DepartmentCount = dashboard.Departments;
            AbsenceCount = dashboard.Absences;
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Не удалось загрузить данные",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SaveEmployeeAsync(EmployeeEditModel model)
    {
        await _employeeService.SaveAsync(model, _session);
        await RefreshAsync();
    }

    private async Task DismissAsync()
    {
        if (SelectedEmployee is null)
        {
            return;
        }

        var confirmation = MessageBox.Show(
            $"Оформить увольнение сотрудника {SelectedEmployee.FullName}?",
            "Подтверждение кадровой операции",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmation != MessageBoxResult.Yes)
        {
            return;
        }

        await _employeeService.DismissAsync(SelectedEmployee.Id, _session);
        await RefreshAsync();
    }
}
