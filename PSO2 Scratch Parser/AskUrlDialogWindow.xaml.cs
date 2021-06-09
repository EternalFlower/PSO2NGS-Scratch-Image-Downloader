using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PSO2_Scratch_Parser
{
    /// <summary>
    /// Interaction logic for AskUrlDialogWindow.xaml
    /// </summary>
    public partial class AskUrlDialogWindow : Window
    {
        public AskUrlDialogWindow()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtURL.SelectAll();
            txtURL.Focus();
        }

        public string URL
        {
            get { return txtURL.Text; }
        }
    }
}
