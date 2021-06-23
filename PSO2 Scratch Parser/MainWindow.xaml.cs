using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.Diagnostics;

namespace PSO2_Scratch_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string AppStatus { get; set; }
        public string ContentStatus { get; set; }
        private readonly ScratchParser ScratchParser;
        private readonly TextBoxOutput textBoxOutput;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            AppStatus = "PSO2 Scratch Parser";

            ScratchParser = new ScratchParser();
            textBoxOutput = new TextBoxOutput(TextBoxLog);
            TextWriterTraceListener outputTextListener = new TextWriterTraceListener(textBoxOutput);
            Trace.Listeners.Add(outputTextListener);
        }

        private void saveItemList(Object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Properties.Settings.Default.SelectSaveJsonDirectory;
            saveFileDialog.Filter = "JSON (*.json)|*.json|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScratchParser.WriteItemList(saveFileDialog.FileName);
                Trace.WriteLine($"Saved parsed data to {saveFileDialog.FileName}.");

                Properties.Settings.Default.SelectSaveJsonDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        private void saveBonusList(Object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Properties.Settings.Default.SelectSaveJsonDirectory;
            saveFileDialog.Filter = "JSON (*.json)|*.json|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScratchParser.WriteBonusList(saveFileDialog.FileName);
                Trace.WriteLine($"Saved parsed data to {saveFileDialog.FileName}.");

                Properties.Settings.Default.SelectSaveJsonDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        /*public void button_ParseFromJSONFile(Object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Properties.Settings.Default.SelectSourceDirectory;
            openFileDialog.Filter = "JSON file (*.json)|*.json|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScratchParser.parseFromJSONFile(openFileDialog.FileName);
                UpdateParseControls();

                Properties.Settings.Default.SelectSourceDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            }
        }*/

        public void button_ParseURL(Object sender, EventArgs e)
        {
            AskUrlDialogWindow askUrlDialogWindow = new AskUrlDialogWindow();

            if (askUrlDialogWindow.ShowDialog() == true)
            {
                if (askUrlDialogWindow.URL.Length == 0)
                {
                    System.Windows.MessageBox.Show("URL needed", "Missing Scratch URL", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ScratchParser.ParseScratch(askUrlDialogWindow.URL);
                UpdateParseControls();
            }
        }

        public void button_DownloadImage(Object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = Properties.Settings.Default.SelectSaveImageDirectory;

                DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var downloadDirectory = dialog.SelectedPath;
                    ScratchParser.SaveImages(downloadDirectory);

                    Properties.Settings.Default.SelectSaveImageDirectory = dialog.SelectedPath;
                }
            }
        }

        public void button_ClearScratchList(Object sender, EventArgs e)
        {
            ScratchParser.Clear();
            UpdateParseControls();
            Trace.WriteLine("Clear fetched data.");
        }

        public void UpdateParseControls()
        {
            var isEnabled = ScratchParser != null && ScratchParser.HasData;
            sourceSaveItemListBtn.IsEnabled = isEnabled;
            sourceSaveBonusListBtn.IsEnabled = isEnabled;
            downloadImageBtn.IsEnabled = isEnabled;
            clearBtn.IsEnabled = isEnabled;
        }

        private void TextBoxLog_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxLog.ScrollToEnd();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Properties.Settings.Default.Save();
        }

        public void menu_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
