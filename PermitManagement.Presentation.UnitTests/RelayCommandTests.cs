using PermitManagement.Testing.Shared;

namespace PermitManagement.Presentation.UnitTests;

public class RelayCommandTests
{
    [Gwt("Given a RelayCommand",
        "when constructed with null execute delegate",
        "then ArgumentNullException is thrown")]
    public void T0()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
        Assert.Equal("executeAsync", ex.ParamName);
    }

    [Gwt("Given a RelayCommand with no canExecute",
        "when CanExecute is called",
        "then it returns true")]
    public void T1()
    {
        // Arrange
        var cmd = new RelayCommand(async () =>
        {
            await Task.CompletedTask;
        });

        // Act
        var canExecute = cmd.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Gwt("Given a RelayCommand with a canExecute returning false",
        "when CanExecute is called",
        "then it returns false")]
    public void T2()
    {
        // Arrange
        var cmd = new RelayCommand(() => Task.CompletedTask, () => false);

        // Act
        var result = cmd.CanExecute(null);

        // Assert
        Assert.False(result);
    }

    [Gwt("Given a RelayCommand",
        "when Execute is called",
        "then the execute delegate is invoked")]
    public async Task T3()
    {
        // Arrange
        var executed = false;
        var cmd = new RelayCommand(async () =>
        {
            executed = true;
            await Task.CompletedTask;
        });

        // Act
        cmd.Execute(null);
        await Task.Delay(10); // allow async void to complete

        // Assert
        Assert.True(executed);
    }

    [Gwt("Given a RelayCommand",
        "when RaiseCanExecuteChanged is called",
        "then CanExecuteChanged event is raised once")]
    public void T4()
    {
        // Arrange
        var cmd = new RelayCommand(() => Task.CompletedTask);
        var raisedCount = 0;
        cmd.CanExecuteChanged += (_, _) => raisedCount++;

        // Act
        cmd.RaiseCanExecuteChanged();

        // Assert
        Assert.Equal(1, raisedCount);
    }

    [Gwt("Given a RelayCommand with true and false canExecute states",
        "when CanExecuteChanged is raised",
        "then the UI can requery the new state")]
    public void T5()
    {
        // Arrange
        var canExecute = false;
        var cmd = new RelayCommand(() => Task.CompletedTask, () => canExecute);
        var raised = false;
        cmd.CanExecuteChanged += (_, _) => raised = true;

        // Act
        canExecute = true;
        cmd.RaiseCanExecuteChanged();

        // Assert
        Assert.True(raised);
        Assert.True(cmd.CanExecute(null));
    }
}
