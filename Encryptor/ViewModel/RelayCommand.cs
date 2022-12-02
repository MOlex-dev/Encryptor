/*
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |              ENCRYPTOR              |
 *    |          RelayCommand Class         |
 *    |                                     |
 *    |    Copyright (c) MOlex-dev, 2022    |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 */

using System;
using System.Windows.Input;

namespace Encryptor.ViewModel;

/// <summary>
/// Class <c>RelayCommand</c> allows to use commands to process Button.Click events
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool>? _canExecute;
    
    public event EventHandler? CanExecuteChanged
    {
        add    => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    
    /// <summary>
    /// Can command be executed?
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    /// <summary>
    /// Execute action
    /// </summary>
    /// <param name="parameter">Parameters for execution</param>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
}
