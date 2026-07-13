using enhanced_recorder.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace enhanced_recorder.ViewModels;
class RecordingsViewModel : ViewModelBase
{
    #region SetUp
    public string OutputFolder { get; }
    private string _selectedRecording;

    public ObservableCollection<string> Recordings = [];

    public DelegateCommand PlayCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand OpenFolderCommand { get; }

    public string SelectedRecording
    {
        get => _selectedRecording;
        set
        {
            if (_selectedRecording != value)
            {
                _selectedRecording = value;
                OnPropertyChanged(nameof(SelectedRecording));
                EnableCommands();
            }
        }
    }
    #endregion
    public RecordingsViewModel()
    {
        OutputFolder = Path.Combine(Path.GetTempPath(), "EnhancedRecorder");
        Directory.CreateDirectory(OutputFolder);
        foreach(string file in Directory.GetFiles(OutputFolder))
        {
            Recordings.Add(file);
        }
        PlayCommand = new DelegateCommand(Play);
        DeleteCommand = new DelegateCommand(Delete);
        OpenFolderCommand = new DelegateCommand(OpenFolder);
        EnableCommands();
    }
    #region Functions
    private void EnableCommands()
    {
        PlayCommand.IsEnabled = SelectedRecording != null;
        DeleteCommand.IsEnabled = SelectedRecording != null;
    }

    private void Play()
    {
        if (SelectedRecording != null)
        {
            ShellExecute(Path.Combine(OutputFolder, SelectedRecording));
        }
    }

    private void Delete()
    {
        if(SelectedRecording != null)
        {
            try
            {
                File.Delete(Path.Combine(OutputFolder, SelectedRecording));
                Recordings.Remove(SelectedRecording);
                SelectedRecording = Recordings.FirstOrDefault()!;
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"No se pudo borrar el archivo {ex.Message}");
            }
        }
    }

    private void OpenFolder()
    {
        ShellExecute(OutputFolder);
    }

    private static void ShellExecute(string file)
    {
        Process process = new()
        {
            StartInfo = new(file)
            {
                UseShellExecute = true
            }
        };
        process.Start();
    }
    #endregion
}
