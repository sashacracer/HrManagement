using System.Windows;
using HrManagement.App.Models;
using HrManagement.App.Services;

namespace HrManagement.App.Views;

public partial class EmployeeDialog : Window
{
    public EmployeeDialog(
        EmployeeEditModel employee,
        IReadOnlyList<Department> departments,
        IReadOnlyList<Position> positions)
    {
        InitializeComponent();
        DataContext = new EmployeeDialogModel(employee, departments, positions);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private sealed record EmployeeDialogModel(
        EmployeeEditModel Employee,
        IReadOnlyList<Department> Departments,
        IReadOnlyList<Position> Positions)
    {
        public string PersonnelNumber
        {
            get => Employee.PersonnelNumber;
            set => Employee.PersonnelNumber = value;
        }

        public string LastName
        {
            get => Employee.LastName;
            set => Employee.LastName = value;
        }

        public string FirstName
        {
            get => Employee.FirstName;
            set => Employee.FirstName = value;
        }

        public string? MiddleName
        {
            get => Employee.MiddleName;
            set => Employee.MiddleName = value;
        }

        public DateTime BirthDate
        {
            get => Employee.BirthDate;
            set => Employee.BirthDate = value;
        }

        public DateTime HireDate
        {
            get => Employee.HireDate;
            set => Employee.HireDate = value;
        }

        public string Email
        {
            get => Employee.Email;
            set => Employee.Email = value;
        }

        public string Phone
        {
            get => Employee.Phone;
            set => Employee.Phone = value;
        }
    }
}
