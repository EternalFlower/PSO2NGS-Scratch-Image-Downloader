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
using System.Globalization;
using System.Windows.Shell;
using System.ComponentModel;
using PSO2_Scratch_Parser.Culture;


namespace PSO2_Scratch_Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string AppStatus { get; set; }
        public string ContentStatus { get; set; }
        private bool _dataAvailable;
        private readonly ScratchParser ScratchParser;
        private readonly TextBoxOutput textBoxOutput;

        public bool DataAvailable
        {
            get => _dataAvailable;
            set
            {
                _dataAvailable = value;
                OnPropertyChanged("DataAvailable");
            }
        }

        public MainWindow()
        {
            CultureResources.ChangeCulture(Properties.Settings.Default.DefaultCulture);

            InitializeComponent();
            DataContext = this;

            LanguagesComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.SelectionChanged_ChangeLanguage);
            LanguagesComboBox.SelectedItem = Properties.Settings.Default.DefaultCulture; ;

            AppStatus = "PSO2 Scratch Parser";

            DataAvailable = false;

            ScratchParser = new ScratchParser();
            textBoxOutput = new TextBoxOutput(TextBoxLog);
            TextWriterTraceListener outputTextListener = new TextWriterTraceListener(textBoxOutput);
            Trace.Listeners.Add(outputTextListener);
        }

        private void SaveItemList(Object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.SelectSaveJsonDirectory,
                Filter = "JSON (*.json)|*.json|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScratchParser.WriteItemListJSON(saveFileDialog.FileName);
                Trace.WriteLine($"Saved Item List JSON to {saveFileDialog.FileName}.");

                Properties.Settings.Default.SelectSaveJsonDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        private void SaveBonusList(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.SelectSaveJsonDirectory,
                Filter = "JSON (*.json)|*.json|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScratchParser.WriteBonusListJSON(saveFileDialog.FileName);
                Trace.WriteLine($"Saved Bonus Item List to {saveFileDialog.FileName}.");

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

        private void OnClick_FetchScratch(object sender, EventArgs e)
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

        private void OnClick_DownloadAllImages(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Properties.Settings.Default.SelectSaveImageDirectory;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var downloadDirectory = dialog.SelectedPath;
                ScratchParser.SaveImages(downloadDirectory);

                Properties.Settings.Default.SelectSaveImageDirectory = dialog.SelectedPath;
            }
        }

        private void OnClick_DownloadItemImages(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Properties.Settings.Default.SelectSaveImageDirectory;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var downloadDirectory = dialog.SelectedPath;
                ScratchParser.SaveImages(downloadDirectory, true, false);

                Properties.Settings.Default.SelectSaveImageDirectory = dialog.SelectedPath;
            }
        }

        private void OnClick_DownloadBonusImages(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Properties.Settings.Default.SelectSaveImageDirectory;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var downloadDirectory = dialog.SelectedPath;
                ScratchParser.SaveImages(downloadDirectory, true, false);

                Properties.Settings.Default.SelectSaveImageDirectory = dialog.SelectedPath;
            }
        }

        private void OnClick_SaveItemNames(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.SelectSaveJsonDirectory,
                Filter = "Text File (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScratchParser.WriteItemName(saveFileDialog.FileName);
                Trace.WriteLine($"Saved item names to {saveFileDialog.FileName}.");

                Properties.Settings.Default.SelectSaveJsonDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        private void OnClick_SaveBonusNames(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.SelectSaveJsonDirectory,
                Filter = "Text File (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScratchParser.WriteBonusName(saveFileDialog.FileName);
                Trace.WriteLine($"Saved item names to {saveFileDialog.FileName}.");

                Properties.Settings.Default.SelectSaveJsonDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        public void OnClick_ClearScratch(object sender, EventArgs e)
        {
            ScratchParser.Clear();
            UpdateParseControls();
            Trace.WriteLine("Clear fetched data.");
        }

        public void UpdateParseControls()
        {
            DataAvailable = ScratchParser != null && ScratchParser.HasData;
        }

        private void OnTextChanged_TextBoxLog(object sender, TextChangedEventArgs e)
        {
            TextBoxLog.ScrollToEnd();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Properties.Settings.Default.Save();
        }

        private void SelectionChanged_ChangeLanguage(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo selected_culture = LanguagesComboBox.SelectedItem as CultureInfo;

            if (Properties.Resource.Culture != null && !Properties.Resource.Culture.Equals(selected_culture))
            {
                Debug.WriteLine(string.Format("Change Language to [{0}]", selected_culture.NativeName));

                Properties.Settings.Default.DefaultCulture = selected_culture;
                Properties.Settings.Default.Save();

                CultureResources.ChangeCulture(selected_culture);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string property = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
