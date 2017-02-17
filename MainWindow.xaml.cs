using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {

        string openFileName;
        string saveFileName;
        string rdlTemplate;
        ArrayList validateFiles = new ArrayList();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello Wolrd","My App",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            refreshConvertWindow();
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name


            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                openFileName= dlg.FileName;
                button1.IsEnabled = true;
                label6.Content = openFileName;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".rdl"; // Default file extension
            dlg.Filter = "RDL documents (.rdl)|*.rdl"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                saveFileName = dlg.FileName;
                label7.Content = saveFileName;
                button2.IsEnabled = true;
            }
        }
        private void button2_Click_Old()
        {
            try
            {


                XmlDocument doc = new XmlDocument();
                string readTemplateText = File.ReadAllText(rdlTemplate);
                doc.LoadXml(readTemplateText);

               
                string readText = File.ReadAllText(openFileName);




                readText =readText.Replace('[', ' ');
                readText = readText.Replace(']', ' ');

                StringBuilder builder = new StringBuilder(readText);
                builder.Replace("[", "");
                builder.Replace("]", "");

                builder.Replace("},", "}");

                readText = builder.ToString(); // Value of y is "Hello 1st 2nd world"
                //File.WriteAllText(saveFileName, readText);
                string[] array = readText.Split('\n');

                foreach (string s in array)
                {
                    //JObject json=JObject.Parse(s);
                    var dict = JObject.Parse(s);
                    XmlElement node = doc.CreateElement("TablixRow");
                    node.InnerXml = findFirstRow(doc).InnerXml;

                    foreach (var kv in dict)
                    {
                        //Console.WriteLine(kv.Key + ":" + kv.Value);
                        xmlNodeChange(node, kv.Key, kv.Value.ToString());
                    }



                    findFirstRowParent(doc).AppendChild(node);


                }


                doc.Save(saveFileName);

                XDocument doc2 = XDocument.Load(saveFileName);
                foreach (var node in doc2.Root.Descendants())
                {
                    // If we have an empty namespace...
                    if (node.Name.NamespaceName == "")
                    {
                        // Remove the xmlns='' attribute. Note the use of
                        // Attributes rather than Attribute, in case the
                        // attribute doesn't exist (which it might not if we'd
                        // created the document "manually" instead of loading
                        // it from a file.)
                        node.Attributes("xmlns").Remove();
                        // Inherit the parent namespace instead
                        node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                    }
                }

                doc2.Save(saveFileName);

                MessageBox.Show("Saved", "Done", MessageBoxButton.OK, MessageBoxImage.Information);

                //        File.WriteAllText(saveFileName,convertToXML(readText));
                //       MessageBox.Show("Successfully converted file", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (JsonException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
           // button2_Click_Old();
            
            string json = File.ReadAllText(openFileName);
            json = "{\"array\":{\"items\": " + json + "}}";
            Console.WriteLine(json);
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
            doc.Save(saveFileName);

            
            

        }
        private void xmlNodeChange(XmlNode d,string tag,string value)
        {
            XmlNode tagNode = findTagNode(d, tag);
            if (tagNode == null) return;
            tagNode.InnerText = value;
        }

        private XmlNode findTagNode(XmlNode d,string tag)
        {
            if (d.InnerText == tag)
                return d;
            foreach (XmlNode x in d.ChildNodes)
            {
                XmlNode t = findTagNode(x,tag);
                if (t != null) return t;
            }
            return null;
        }

        private XmlNode findFirstRow(XmlNode d)
        {
            if (d.Name == "TablixRow")
                return d;
            foreach (XmlNode x in d.ChildNodes)
            {
                XmlNode t = findFirstRow(x);
                if (t != null) return t;
            }
            return null;

        }
        private XmlNode findFirstRowParent(XmlNode d)
        {
            if (d.Name == "TablixRows")
                return d;
            foreach (XmlNode x in d.ChildNodes)
            {
                XmlNode t = findFirstRowParent(x);
                if (t != null) return t;
            }
            return null;

        }
        private string convertToXML(string jsonStr)
        {
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonStr);
            return doc.OuterXml;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            refreshValidationWindow();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.Filter =
        "RDL File (*.RDL;*.RDLC)|*.RDL;*.RDLC|" +
        "All files (*.*)|*.*";

            dlg.Multiselect = true;
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
               
                // Open document
                foreach (string file in dlg.FileNames)
                {
                    validateFiles.Add(new RDLDocument(file));
                }
                button3.IsEnabled = true;
                label4.Content = "" + validateFiles.Count + " file(s) selected";
            }

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

            //validate files as a background process in a different thread
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            
            bw.DoWork += new DoWorkEventHandler(
                delegate (object o, DoWorkEventArgs args)
                {
                    BackgroundWorker b = o as BackgroundWorker;
                    int count = 0;
                    foreach(RDLDocument file in validateFiles){
                        file.validate();
                        count += 1;
                        b.ReportProgress(count);
                    }
                }
            );

            bw.ProgressChanged += new ProgressChangedEventHandler(
                delegate (object o, ProgressChangedEventArgs args)
                {
                    progressBar.Value = args.ProgressPercentage*100/validateFiles.Count;
                    label5.Content = ((RDLDocument)validateFiles[0]).fileName;
                }
            );
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate (object o, RunWorkerCompletedEventArgs args)
                 {
                     label5.Content = "";
                     button5.IsEnabled = true;
                     
                     int errorCount = 0;
                     int errorFiles = 0;
                     foreach(RDLDocument d in validateFiles)
                     {
                         errorCount += d.errors.Count;
                         if (d.errors.Count != 0)
                             errorFiles += 1;
                     }
                     string message = "" + validateFiles.Count + " files checked \n";
                     message += "" + errorFiles + " files has " + errorCount + " errors\n";
                     textBox.Text = message;
                     progressBar.Value = 100;
                 }
            );

            bw.RunWorkerAsync();

        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            ErrorLogWindow win = new ErrorLogWindow(validateFiles);
            win.Show();
        }

        private void refreshValidationWindow()
        {
            validateFiles.Clear();
            label4.Content = "";
            label5.Content = "";
            button5.IsEnabled = false;
            button3.IsEnabled = false;
            progressBar.Value = 0;
        }

        private void refreshConvertWindow()
        {
            button1.IsEnabled = false;
            button2.IsEnabled = false;
            label6.Content = "";
            label7.Content = "";
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            refreshValidationWindow();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.Filter =
        "RDL File (*.RDL;*.RDLC)|*.RDL;*.RDLC|" +
        "All files (*.*)|*.*";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                foreach (string file in dlg.FileNames)
                {
                    validateFiles.Add(new RDLDocument(file));
                }
                button3.IsEnabled = true;
                label4.Content = "" + validateFiles.Count + " file(s) selected";
            }
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name


            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                rdlTemplate = dlg.FileName;
               
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
