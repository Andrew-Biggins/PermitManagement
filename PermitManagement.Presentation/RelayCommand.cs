using System.Windows.Input;

namespace PermitManagement.Presentation;

public sealed class RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    public async void Execute(object? parameter) => await _executeAsync();

    private readonly Func<Task> _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
}