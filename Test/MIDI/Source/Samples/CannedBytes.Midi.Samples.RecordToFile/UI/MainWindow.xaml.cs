using CannedBytes.Midi.Samples.RecordToFile.UI;
using System.Windows;

namespace CannedBytes.Midi.Samples.RecordToFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppData appData;
        public NewModelView nmw;
        LogWindow lw;

        public MainWindow()
        {
            InitializeComponent();
            lw = new LogWindow();
            lw.Show();

             appData = new AppData(this.Dispatcher);
            AppData.Singleton = appData;

            this.CommandBindings.Add(new StartStopCommandHandler(appData).ToCommandBinding());

            this.DataContext = appData;
            nmw = new NewModelView();
            NewModelView.Singleton =nmw;
            nmw.LastKey = "Init....";
            lw.DataContext = nmw;
        }
    }
}