using laba6.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace laba6
{
    public class ViewModel : INotifyPropertyChanged
    {
        public Model.Text Model { get; set; }

        public ViewModel() => Model = new Model.Text
        {
            Doc = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Bold>Hello, this is RichTextBox</Bold></Paragraph></FlowDocument>"
        };
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        private BaseCommand saveCommand;
        public BaseCommand SaveCommand
        {
            get
            {
                if (saveCommand != null)
                    return saveCommand;
                else
                {
                    Action<object> Execute = o =>
                    {
                        SaveFileDialog sfd = new SaveFileDialog
                        {
                            InitialDirectory = Environment.CurrentDirectory,
                            Filter = "Файл в json|*.json"
                        };
                        if (sfd.ShowDialog() == true)
                        {
                            string s = JsonConvert.SerializeObject(Model, Formatting.Indented);
                            FileStream fs = File.Create(sfd.FileName);
                            StreamWriter sw = new StreamWriter(fs);
                            sw.Write(s);
                            sw.Close();
                            fs.Close();
                        }
                    };
                    Func<object, bool> CanExecute = o => Model.Doc != null;
                    saveCommand = new BaseCommand(Execute, CanExecute);
                    return saveCommand;
                }
            }
        }

        private BaseCommand openCommand;
        public BaseCommand OpenCommand
        {
            get
            {
                return openCommand ??
                (openCommand = new BaseCommand(obj =>
                {
                    OpenFileDialog ofd = new OpenFileDialog
                    {
                        InitialDirectory = Environment.CurrentDirectory,
                        Filter = "Файл в json|*.json"
                    };
                    if (ofd.ShowDialog() == true)
                    {
                        using (Stream sw = new FileStream(ofd.FileName, FileMode.Open))
                        {
                            byte[] array = new byte[sw.Length];
                            sw.Read(array, 0, array.Length);
                            string s = Encoding.Default.GetString(array);
                            Text jsonText = JsonConvert.DeserializeObject<Text>(s);
                            Model.Doc = jsonText.Doc.ToString();
                        }
                    }
                }));
            }
        }

    }
}
