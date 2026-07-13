using System.Windows.Input;

namespace enhanced_recorder.Base;

class DelegateCommand(Action action) : ICommand
{
    private readonly Action _action = action;
    private bool _isEnabled = true;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return _isEnabled;
    }

    public void Execute(object? parameter)
    {
        _action();
    }

    public bool IsEnabled
    {
        get { return _isEnabled; }
        set { 
            if (_isEnabled != value) {
                _isEnabled = value;
                OnCanExecuteChanged();
            } 
        }
    }

    private void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
