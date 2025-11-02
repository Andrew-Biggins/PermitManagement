using PermitManagement.Core.Entities;
using PermitManagement.Presentation.Interfaces;
using PermitManagement.Shared;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static PermitManagement.Shared.ValidationRules;
using static PermitManagement.Shared.Constants;

namespace PermitManagement.Presentation;

public class PermitViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private readonly IPermitApiClient _api;
    private readonly Dictionary<string, List<string>> _errors = [];

    private string _selectedZone = "A";
    private string _vehicleRegistration = DefaultValidRegistrationNumber;
    private string _statusMessage = string.Empty;
    private DateTime _startDate = DateTime.Today;
    private string _selectedDuration = "1 Week";
    private bool _permitCheckResult;
    private bool _showAllPermits;

    public PermitViewModel(IPermitApiClient api)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        AddPermitCommand = new RelayCommand(AddPermitAsync, CanAddPermit);
        _ = ValidateRegistration(); 
        _ = CheckPermitAsync();
        _ = LoadPermitsAsync();
    }

    public ObservableCollection<Permit> Permits { get; } = [];

    public static IEnumerable<string> AvailableZones => Enum.GetValues(typeof(ZoneName))
                                                            .OfType<ZoneName>()
                                                            .Select(z => z.ToString());

    public string SelectedZone
    {
        get => _selectedZone;
        set
        {
            if (_selectedZone != value)
            {
                _selectedZone = value;
                OnPropertyChanged();
                _ = CheckPermitAsync();
                if (!ShowAllPermits) _ = LoadPermitsAsync();
            }
        }
    }

    public string VehicleRegistration
    {
        get => _vehicleRegistration;
        set
        {
            if (_vehicleRegistration != value)
            {
                _vehicleRegistration = value;
                OnPropertyChanged();
                _ = ValidateRegistration();
            }
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

    public bool HasValidPermit => _permitCheckResult && !HasErrors;
    public bool NoValidPermit => !_permitCheckResult && !HasErrors;

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool ShowAllPermits
    {
        get => _showAllPermits;
        set
        {
            if (_showAllPermits != value)
            {
                _showAllPermits = value;
                OnPropertyChanged();
                _ = LoadPermitsAsync(); 
            }
        }
    }

    public RelayCommand AddPermitCommand { get; }

    public async Task LoadPermitsAsync()
    {
        IEnumerable<Permit> permits;

        if (ShowAllPermits)
            permits = await _api.GetActivePermitsAsync(null); // all zones
        else
            permits = await _api.GetActivePermitsAsync(SelectedZone);

        Permits.Clear();
        foreach (var p in permits)
            Permits.Add(p);
    }

    private async Task CheckPermitAsync()
    {
        if (string.IsNullOrWhiteSpace(VehicleRegistration) || string.IsNullOrWhiteSpace(SelectedZone))
            return;

        var result = await _api.CheckPermitAsync(VehicleRegistration.ToUpperInvariant(), SelectedZone);
        _permitCheckResult = result;
        OnPropertyChanged(nameof(HasValidPermit));
        OnPropertyChanged(nameof(NoValidPermit));
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

        var newPermit = new Permit(new Vehicle(VehicleRegistration.ToUpperInvariant()), new Zone(SelectedZone),
                                   StartDate, endDate);
        await _api.AddPermitAsync(newPermit);
        await CheckPermitAsync();
        await LoadPermitsAsync();
        StatusMessage = "Permit added successfully!";
        _ = Task.Run(async () =>
        {
            await Task.Delay(3000);
            StatusMessage = string.Empty;
        });
    }

    public bool HasErrors => _errors.Count != 0;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
        => propertyName != null && _errors.TryGetValue(propertyName, out List<string>? value)
            ? value : Enumerable.Empty<string>();

    private async Task ValidateRegistration()
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
            OnPropertyChanged(nameof(NoValidPermit));
            OnPropertyChanged(nameof(HasValidPermit));
            AddPermitCommand.RaiseCanExecuteChanged();
        }

        if (!hasErrorsNow) await CheckPermitAsync();
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
    private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
