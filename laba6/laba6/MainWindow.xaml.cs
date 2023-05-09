using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Markup;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Json;

namespace laba6
{



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //local:MainWindow.DocumentXaml="{Binding Model.Doc, UpdateSourceTrigger=PropertyChanged}"

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //используем системные шрифты
            cmbFontStyle.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbColorBack.ItemsSource = new List<string>() { "Transparent", "White", "Black", "Red", "Blue", "Yellow", "Green" };
            //задаем возможные шрифты
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            DataContext = new ViewModel();

        }
        private static HashSet<Thread> _recursionProtection = new HashSet<Thread>();

        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            _recursionProtection.Add(Thread.CurrentThread);
            obj.SetValue(DocumentXamlProperty, value);
            _recursionProtection.Remove(Thread.CurrentThread);
        }

        public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof(string),
            typeof(MainWindow),
            new FrameworkPropertyMetadata(
                "",
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (obj, e) => {
                    if (_recursionProtection.Contains(Thread.CurrentThread))
                        return;

                    var richTextBox = (RichTextBox)obj;
                    try
                    {
                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(richTextBox)));
                        var doc = (FlowDocument)XamlReader.Load(stream);

                        richTextBox.Document = doc;
                    }
                    catch (Exception)
                    {
                        richTextBox.Document = new FlowDocument();
                    }

                    richTextBox.TextChanged += (obj2, e2) =>
                    {
                        RichTextBox richTextBox2 = obj2 as RichTextBox;
                        if (richTextBox2 != null)
                        {
                            SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox2.Document));
                        }
                    };
                }
            )
        );
        private void cmbFontStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontStyle.SelectedItem != null)
                rtb.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontStyle.SelectedItem);
        }
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num;
            if ((Int32.TryParse(cmbFontSize.Text, out num))&& (Int32.Parse(cmbFontSize.Text)>0))
            {
                rtb.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
            }
        }

        private void cmbColorBack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbColorBack.SelectedItem != null)
                rtb.Selection.ApplyPropertyValue(Inline.BackgroundProperty, cmbColorBack.SelectedItem);
        }
        

    }
}
