using PermitManagement.Core.Entities;
using PermitManagement.Presentation.Interfaces;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static PermitManagement.Presentation.Shared;

namespace PermitManagement.Presentation;

public class PermitViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private readonly IPermitApiClient _api;
    private readonly Dictionary<string, List<string>> _errors = [];

    private string _selectedZone = "A";
    private string _vehicleRegistration = string.Empty;
    private DateTime _startDate = DateTime.Today;
    private string _selectedDuration = "1 Week";
    
    public PermitViewModel(IPermitApiClient api)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        LoadPermitsCommand = new RelayCommand(LoadPermitsAsync);
        AddPermitCommand = new RelayCommand(AddPermitAsync, CanAddPermit);
        ValidateRegistration();
    }

    public ObservableCollection<Permit> Permits { get; } = [];

    public string SelectedZone
    {
        get => _selectedZone;
        set => SetProperty(ref _selectedZone, value);
    }

    public string VehicleRegistration
    {
        get => _vehicleRegistration;
        set
        {
            SetProperty(ref _vehicleRegistration, value);
            ValidateRegistration();
        }
    }

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public string SelectedDuration
    {
        get => _selectedDuration;
        set => SetProperty(ref _selectedDuration, value);
    }

    public ICommand LoadPermitsCommand { get; }
    public RelayCommand AddPermitCommand { get; }

    public async Task LoadPermitsAsync()
    {
        var permits = await _api.GetActivePermitsAsync(SelectedZone);
        Permits.Clear();
        foreach (var p in permits)
            Permits.Add(p);
    }

    public async Task AddPermitAsync()
    {
        var endDate = SelectedDuration switch
        {
            "1 Day" => StartDate.AddDays(1),
            "1 Week" => StartDate.AddDays(7),
            "1 Month" => StartDate.AddMonths(1),
            "1 Year" => StartDate.AddYears(1),
            _ => StartDate.AddDays(7)
        };

        var newPermit = new Permit(new Vehicle(VehicleRegistration), new Zone(SelectedZone),
                                   StartDate, endDate);
        await _api.AddPermitAsync(newPermit);
        await LoadPermitsAsync();
    }

    public bool HasErrors => _errors.Count != 0;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
        => propertyName != null && _errors.TryGetValue(propertyName, out List<string>? value)
            ? value : Enumerable.Empty<string>();

    private void ValidateRegistration()
    {
        const string key = nameof(VehicleRegistration);
        var hadErrorsBefore = _errors.ContainsKey(key);

        var errorMessage = GetRegistrationError(VehicleRegistration);

        if (errorMessage is null)
            _errors.Remove(key);
        else
            _errors[key] = [errorMessage];

        var hasErrorsNow = _errors.ContainsKey(key);

        if (hadErrorsBefore != hasErrorsNow)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(key));
            AddPermitCommand.RaiseCanExecuteChanged();
        }
    }

    private static string? GetRegistrationError(string? registration)
    {
        if (string.IsNullOrWhiteSpace(registration))
            return "Registration is required.";

        return RegPattern.IsMatch(registration)
            ? null
            : InvalidRegistrationMessage;
    }

    private bool CanAddPermit() => !HasErrors && !_errors.ContainsKey(nameof(VehicleRegistration));

    private void SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = "")
    {
        if (!Equals(member, value))
        {
            member = value;
            OnPropertyChanged(propertyName);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
