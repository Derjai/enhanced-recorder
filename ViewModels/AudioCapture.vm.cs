using enhanced_recorder.Base;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using System.Windows;


namespace enhanced_recorder.ViewModels;

internal class AudioCapture : ViewModelBase, IDisposable
{
    #region SetUp
    private MMDevice _selectedDevice;
    private int _sampleRate;
    private int _bitDepth;
    private int _channelCount;
    private int _sampleTypeIndex;
    private WasapiCapture _capture;
    private WaveFileWriter _waveFileWriter;
    private string _fileName;
    private string _message;
    private float _peak;
    private readonly SynchronizationContext _synchronizationContext;

    private float _recordLevel;
    private int _shareModeIndex;

    public IEnumerable<MMDevice> CaptureDevices { get; }

    public DelegateCommand RecordCommand { get;}
    public DelegateCommand StopCommand { get;}
    public MMDevice SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (_selectedDevice != value)
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
                GetDefaultRecordingFormat(value);
            }
        }
    }
    public int SampleRate
    {
        get => _sampleRate;
        set
        {
            if (_sampleRate != value)
            {
                _sampleRate = value;
                OnPropertyChanged(nameof(SampleRate));
            }
        }
    }

    public int BitDepth
    {
        get => _bitDepth;
        set
        {
            if (_bitDepth != value)
            {
                _bitDepth = value;
                OnPropertyChanged(nameof(BitDepth));
            }
        }
    }

    public int ChannelCount
    {
        get => _channelCount;
        set
        {
            if (_channelCount != value)
            {
                _channelCount = value;
                OnPropertyChanged(nameof(ChannelCount));
            }
        }
    }

    public int SampleTypeIndex
    {
        get => _sampleTypeIndex;
        set
        {
            if (_sampleTypeIndex != value)
            {
                _sampleTypeIndex = value;
                OnPropertyChanged(nameof(SampleTypeIndex));
                BitDepth = _sampleTypeIndex == 1 ? 16 : 32;
                OnPropertyChanged(nameof(IsBitDepthConfigurable));
            }
        }
    }

    public string Message
    {
        get => _message;
        set
        {
            if (_message != value)
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
    }

    public int ShareModeIndex
    {
        get => _shareModeIndex;
        set
        {
            if (_shareModeIndex != value)
            {
                _shareModeIndex = value;
                OnPropertyChanged(nameof(ShareModeIndex));
            }
        }
    }

    public float RecordLevel
    {
        get => _recordLevel;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_recordLevel != value)
            {
                _recordLevel = value;
                if (_capture != null)
                {
                    SelectedDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value;
                }
                OnPropertyChanged(nameof(RecordLevel));
            }
        }
    }

    public bool IsBitDepthConfigurable => SampleTypeIndex == 1;

    public float Peak
    {
        get => _peak;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_peak != value)
            {
                _peak = value;
                OnPropertyChanged(nameof(Peak));
            }
        }
    }

    public RecordingsViewModel RecordingsViewModel { get; }
    #endregion


    public AudioCapture()
    {
        _synchronizationContext = SynchronizationContext.Current!;
        MMDeviceEnumerator devices = new();
        CaptureDevices = [.. devices.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)];
        MMDevice defaultDevice = devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
        SelectedDevice = CaptureDevices.FirstOrDefault(device => device.ID == defaultDevice.ID)!;
        RecordCommand = new DelegateCommand(Record);
        StopCommand = new DelegateCommand(Stop) { IsEnabled= false };
        RecordingsViewModel = new RecordingsViewModel();
    }

    #region Functions
    private void Stop()
    {
        _capture?.StopRecording();
    }

    private void Record() 
    {
        try
        {
            _capture = new WasapiCapture(SelectedDevice)
            {
                ShareMode = ShareModeIndex == 0 ? AudioClientShareMode.Shared : AudioClientShareMode.Exclusive,
                WaveFormat = SampleTypeIndex == 0 ? WaveFormat.CreateIeeeFloatWaveFormat(_sampleRate, _channelCount) :
                        new WaveFormat(_sampleRate, _bitDepth, _channelCount)
            };
            _fileName = $"EnhancedRecorder {DateTime.Now:yyy-MM-dd HH-mm-ss}.wav";
            RecordLevel = SelectedDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
            _capture.StartRecording();
            _capture.RecordingStopped += OnRecordingStopped;
            _capture.DataAvailable += CaptureOnDataAvailable;
            RecordCommand.IsEnabled = false;
            StopCommand.IsEnabled = true;
            Message = "Recording...";
        }
        catch (Exception ex) 
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        _waveFileWriter.Dispose();
        _waveFileWriter = null;
        RecordingsViewModel.Recordings.Add(_fileName);
        RecordingsViewModel.SelectedRecording = _fileName;
        if (e.Exception == null) Message = "Recording Stopped";
        else Message = "Recording error:" +e.Exception.Message;
        _capture.Dispose();
        _capture = null;
        RecordCommand.IsEnabled = true;
        StopCommand.IsEnabled = false;
    }

    private void CaptureOnDataAvailable(object? sender, WaveInEventArgs e)
    {
        _waveFileWriter ??= new WaveFileWriter(Path.Combine(), _capture.WaveFormat);
        _waveFileWriter.Write(e.Buffer, 0,e.BytesRecorded);
        UpdatePeakMeter();
    }

    void UpdatePeakMeter()
    {
        _synchronizationContext.Post(context => Peak = SelectedDevice.AudioMeterInformation.MasterPeakValue, null);
    }

    public void Dispose()
    {
        Stop();
    }

    private void GetDefaultRecordingFormat(MMDevice value)
    {
        using var c = new WasapiCapture(value);
        SampleTypeIndex = c.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat ? 0 : 1;
        SampleRate = c.WaveFormat.SampleRate;
        BitDepth = c.WaveFormat.BitsPerSample;
        ChannelCount = c.WaveFormat.Channels;
        Message = "";
    }
    #endregion
}
