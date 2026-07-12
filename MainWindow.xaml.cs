using System.Windows;
using System.Windows.Media;

namespace enhanced_recorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _checkBoxState;
        private Brush _color = Brushes.Blue;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnCheckBoxState_Changed(object sender, RoutedEventArgs e) 
        {
            switch (this.showMenu_checkBox.IsChecked)
            {
                case true:
                    _checkBoxState = true;
                    _color = Brushes.Green;
                    break;
                case false:
                    _checkBoxState = false;
                    _color = Brushes.Red;
                    break;
            }
            this.summary_text.Text = $"{_checkBoxState}";
            this.color_canva.Background = _color;
        }

        private void SetDate_Tomorrow() 
        {
            this.date_picker.SelectedDate = new DateTime().AddDays(1);
        }

        private void start_recording_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void stop_recording_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void play_recording_button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}