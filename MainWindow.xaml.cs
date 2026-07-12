using System.Windows;
using System.Windows.Media;

namespace enhanced_recorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnCheckBoxState_Changed(object sender, RoutedEventArgs e) 
        {
            SetItems_Visibility((bool)this.showMenu_checkBox.IsChecked!);   
        }

        private void SetItems_Visibility(bool isChecked)
        {
            Visibility value = isChecked ? Visibility.Hidden : Visibility.Visible;
            this.color_canva.Visibility = value;
            this.summary_block.Visibility = value;
            this.summary_block.Visibility = value;
            this.date_picker.Visibility = value;
        }


        private void SetDate_Tomorrow() 
        {
            this.date_picker.SelectedDate = new DateTime().AddDays(1);
        }

        private void Start_recording_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Stop_recording_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Play_recording_button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}