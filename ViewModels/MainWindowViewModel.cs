
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MusicPlayer.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private object _currentPage;

    public object CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage == value)
                return;

            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public ICommand ShowPlayListsCommand { get; }
    public ICommand ShowSettingsCommand { get; }

    public MainWindowViewModel()
    {
        CurrentPage = new PlayListsViewModel();

        ShowPlayListsCommand = new RelayCommand(
            () => CurrentPage = new PlayListsViewModel()
        );

        ShowSettingsCommand = new RelayCommand(
            () => CurrentPage = new SettingsViewModel()
        );
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName)
        );
    }
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;

    public RelayCommand(Action execute)
    {
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        _execute();
    }

    public event EventHandler? CanExecuteChanged;
}
