using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Построение_графиков
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeParameterGrid();

            //Height += 30;
            //Width += 30;
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (PlotView.Model != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "График";
                dlg.DefaultExt = ".png";
                dlg.Filter = "PNG Files|*.png";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filePath = dlg.FileName;
                    using (var stream = File.Create(filePath))
                    {
                        var pngExporter = new PngExporter { Width = 800, Height = 600 };
                        pngExporter.Export(PlotView.Model, stream);
                    }

                    MessageBox.Show("График успешно сохранён!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Нет графика для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            string message = "Программа для построения графиков.\n\n" +
                             "Создатель: Шабакаев Р.Р.\n" +
                             "Принимает курсовой проект: Мачнева Е.А.\n\n" +
                             "Приложение позволяет строить графики различных функций и" +
                             "сохранять их в PNG.";
            string caption = "О программе";

            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void InitializeParameterGrid()
        {
            ParametrDataGrid.Columns.Add(new DataGridTextColumn { Header = "Параметр", Binding = new System.Windows.Data.Binding("Name") });
            ParametrDataGrid.Columns.Add(new DataGridTextColumn { Header = "Значение", Binding = new System.Windows.Data.Binding("Value") });
        }

        private void ChoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ChoiceListBox.SelectedItem as ListBoxItem;
            if (selectedItem != null)
            {
                string selectedFunction = selectedItem.Content.ToString();
                UpdateParameterGrid(selectedFunction);
            }
        }

        private void UpdateParameterGrid(string selectedFunction)
        {
            ParametrDataGrid.Items.Clear();
            if (selectedFunction.Contains("a"))
                ParametrDataGrid.Items.Add(new { Name = "a", Value = 1.0 });
            if (selectedFunction.Contains("b"))
                ParametrDataGrid.Items.Add(new { Name = "b", Value = 1.0 });
            if (selectedFunction.Contains("c"))
                ParametrDataGrid.Items.Add(new { Name = "c", Value = 0.0 });
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            double fromX = double.Parse(FromTextBox.Text);
            double toX = double.Parse(BeforeTextBox.Text);
            var selectedItem = ChoiceListBox.SelectedItem as ListBoxItem;
            if (selectedItem != null)
            {
                string selectedFunction = selectedItem.Content.ToString();
                PlotGraph(selectedFunction, fromX, toX);
            }
        }

        private void PlotGraph(string selectedFunction, double fromX, double toX)
        {
            double a = GetParameter("a", 1.0);
            double b = GetParameter("b", 1.0);
            double c = GetParameter("c", 0.0);
            var plotModel = new PlotModel { Title = selectedFunction };
            var lineSeries = new LineSeries();
            for (double x = fromX; x <= toX; x += 0.01)
            {
                double y = CalculateFunction(selectedFunction, x, a, b, c);
                lineSeries.Points.Add(new DataPoint(x, y));
            }
            plotModel.Series.Add(lineSeries);
            PlotView.Model = plotModel;
        }

        private double CalculateFunction(string selectedFunction, double x, double a, double b, double c)
        {
            switch (selectedFunction)
            {
                case "y = x^3":
                    return Math.Pow(x, 3);
                case "y = a*x^3":
                    return a * Math.Pow(x, 3);
                case "y = cos(x)":
                    return Math.Cos(x);
                case "y = sqrt(x)":
                    return Math.Sqrt(x);
                case "y = x^a":
                    return Math.Pow(x, a);
                case "y = ln(x)":
                    return Math.Log(x);
                case "y = a*x^2 + b*x + c":
                    return a * Math.Pow(x, 2) + b * x + c;
                case "y = a*sin(x*b) + c":
                    return a * Math.Sin(b * x) + c;
                case "y = a*x + b":
                    return a * x + b;
                case "y = sin(x)*cos(x)":
                    return Math.Sin(x) * Math.Cos(x);
                default:
                    return 0;
            }
        }

        private double GetParameter(string name, double defaultValue)
        {
            foreach (var item in ParametrDataGrid.Items)
            {
                var row = item as dynamic;
                if (row.Name == name)
                {
                    return double.Parse(row.Value.ToString());
                }
            }
            return defaultValue;
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            e.Handled = !IsTextAllowed(textBox, e.Text);
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        private static bool IsTextAllowed(TextBox textBox, string inputText)
        {
            foreach (char c in inputText)
            {
                if (char.IsDigit(c))
                {
                    continue;
                }
                if (c == '-' && textBox.Text.Length == 0)
                {
                    continue;
                }
                return false;
            }
            return true;
        }
    }
}