using System;
using System.Collections;
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
using System.Windows.Shapes;

namespace Report
{
    /// <summary>
    /// Interaction logic for ErrorLogWindow.xaml
    /// </summary>
    public partial class ErrorLogWindow : Window
    {
        ArrayList fileList;
        public ErrorLogWindow(ArrayList list)
        {
            InitializeComponent();
            this.fileList = list;
            foreach(RDLDocument d in fileList)
            {
                comboBox.Items.Add(d.fileName);
            }
            comboBox.SelectedIndex = 0;
            
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(comboBox.SelectedIndex);
            string errorMessage = "";
            foreach(string error in ((RDLDocument)fileList[comboBox.SelectedIndex]).errors)
            {
                errorMessage += error + "\n";
            }
            textBox.Text = errorMessage;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            foreach(RDLDocument d in fileList)
            {
                if (d == null)
                {
                    Console.WriteLine("D is null");
                }
                d.fixRDL();
            }
            MessageBox.Show("Done", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            textBox.Text = "";
        }
    }
}
