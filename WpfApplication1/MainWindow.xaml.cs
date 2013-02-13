using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;

namespace SimioREPL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static List<string> History;
        public static Dictionary<string, string> Functions;
        public static int ExecCount;
        public static int RecallCount;
        public static RoutedCommand RecallCommand = new RoutedCommand();
        public static RoutedCommand ExecCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            Sc.Host = new ScriptHost();
            History = new List<string>();
            ExecCount = 0;
            RecallCount = 0;
            RecallCommand.InputGestures.Add(new KeyGesture(Key.Up,ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(RecallCommand, RecallKeyGesture));
            ExecCommand.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(ExecCommand,ExecKeyGesture));
        }

        private void RecallKeyGesture(object sender, ExecutedRoutedEventArgs e)
        {
            recallButton_Click(null, null);
        }

        private void ExecKeyGesture(object sender, ExecutedRoutedEventArgs e)
        {
            ExecClick(null,null);
        }

        private void CreateFunctionList()
        {
            try
            {
                Functions = new Dictionary<string,string>();
                const string filename = @"c:\SimioReferences\REPL_Functions.txt";
                if (File.Exists(filename))
                {
                    var end = false;
                    var currentLoc = 0;
                    var funcFile = File.ReadAllText(filename);
                    while (!end)
                    {
                        if (currentLoc == 0)
                        {
                            if (funcFile.Substring(0, 9) == "<<*END*>>")
                            {
                                end = true;
                            }
                        }
                        if (end) continue;
                        int x = funcFile.IndexOf("<<*", currentLoc, StringComparison.Ordinal);
                        int y = funcFile.IndexOf("*>>", currentLoc, StringComparison.Ordinal);
                        int z = funcFile.IndexOf("<<*", y, StringComparison.Ordinal);

                        if (x>=0&&y>0&&z>0)
                        {
                            string funcName = funcFile.Substring(x + 3, y - x - 3);
                            string function = funcFile.Substring(y + 3, z - 1 - y - 3);
                            Functions.Add(funcName, function);
                            currentLoc = z - 1;
                            if (funcFile.Substring(z, 9) == "<<*END*>>")
                            {
                                end = true;
                            }
                        }
                        else
                        {
                            //error! don't load function
                            end = true;
                        }
                    }
                    if (Functions.Count > 0)
                    {
                        foreach (var item in Functions)
                        {
                            FuncComboBox.Items.Add(item.Key);
                        }
                    }
                    else
                    {
                        FuncComboBox.IsEnabled = false;
                    }
                }
                else
                {
                    FuncComboBox.IsEnabled = false;
                }
            }
            catch (Exception e)
            {
                OutputTextBox.AppendText(e.Message);
            }
        }

        private void ExecClick(object sender, RoutedEventArgs e)
        {
            try
            {
                object result = Sc.Host.Execute(WriteCheck(InputBox.Text));
                ExecCount++;
                RecallCount = ExecCount;
                OutputTextBox.AppendText(String.Format("{0}>> {1}\n", ExecCount, result));
                History.Add(InputBox.Text);
                InputBox.Clear();
                OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                OutputTextBox.ScrollToEnd();
            }
            catch (Exception ex)
            {
                OutputTextBox.AppendText(String.Format("Error: {0}\n", ex.Message));
            }
        }
        public string WriteCheck(string text)
        {
            if (text.Contains("Write(") || text.Contains("WriteLine("))
            {
                string cout = "string _cout = \"\";";
                string temp = text.Replace("Write(", "_cout += String.Format(");
                temp = temp.Replace("WriteLine(", "_cout += String.Format(\"{0}\\n\",");
                cout += temp;
                cout += "\n _cout";
                return cout;
            }
            return text;
        }

        private void InitClick(object sender, RoutedEventArgs e)
        {
            InputBox.Clear();
            InputBox.Text = String.Format("#r \"{0}\\SimioAPI.dll\"\n#r \"{0}\\SimioAPI.Extensions.dll\"\n#r \"{0}\\UserExtensions\\SimioREPLWPF\\SimioREPLWPF.dll\"\nusing SimioAPI.Extensions;\nusing SimioAPI;\nusing System;\nusing System.Collections.Generic;\nusing System.Collections;\n\"Initialised Environment\"",Environment.CurrentDirectory);
            ExecClick(sender,e);
        }

        private void HistoryClick(object sender, RoutedEventArgs e)
        {
            InputBox.Clear();
            if (History.Count > 0)
            {
                foreach (string past in History)
                {
                    InputBox.AppendText(past);
                    InputBox.AppendText("\n");
                }
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Clear();
        }

        private void recallButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (History.Count > 0 && RecallCount > 1)
            {
                RecallCount = RecallCount - 1;
                InputBox.Clear();
                InputBox.AppendText(History[RecallCount]);
            }
        }

        private void loadCodeButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
                {
                        Filter = "txt files (*.txt)|*.txt",
                        FilterIndex = 1,
                        RestoreDirectory = true
                };
            // Show open file dialog box 
            bool? result = openFileDialog1.ShowDialog();
            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = openFileDialog1.FileName;
                InputBox.Clear();
                InputBox.Text = File.ReadAllText(filename);
            }
        }

        private void funcComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FuncComboBox.SelectedIndex != -1)
            {
                try
                {
                    string text = Functions[FuncComboBox.SelectedItem.ToString()];
                    InputBox.AppendText(text);
                    FuncComboBox.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    OutputTextBox.AppendText(ex.Message);
                }
            }
        }

        private void funcComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            CreateFunctionList();
        }

        private void outputTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            OutputTextBox.AppendText("\n");
        }

        private void inputBox_Loaded(object sender, RoutedEventArgs e)
        {
            InputBox.Clear();
            InputBox.Text = String.Format("#r \"{0}\\SimioAPI.dll\"\n#r \"{0}\\SimioAPI.Extensions.dll\"\n#r \"{0}\\UserExtensions\\SimioREPL\\SimioREPL.dll\"\nusing SimioAPI.Extensions;\nusing SimioAPI;\nusing System;\nusing System.Collections.Generic;\nusing System.Collections;\n\"Initialised Environment\"", Environment.CurrentDirectory);
            ExecClick(sender, e);
        }

        public void Connect(int connectionId, object target)
        {
            //throw new NotImplementedException();
        }
    }
}
