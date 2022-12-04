using Microsoft.Win32;
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
using System.Windows.Shapes;
using FiveWordsFiveLetters.BL;
using System.IO;

namespace FiveWordsFiveLetters.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Words.GetUpdateProgressEvent += UpdateProgress;
            Words.FinishedEvent += FinishedProgress;
            Words.UpdateAllCombinationsEvent += UpdateAllCombinations;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog()
            {
                Filter = "txt files (*.txt)|*.txt"
            };

            var hasSelectedFile = openDialog.ShowDialog() == true;
            if (hasSelectedFile) {
                CurrentFileLabel.Text = "Current File: " + openDialog.SafeFileName;
                CurrentFileLabel.Visibility = Visibility.Visible;
                Words.Path = openDialog.FileName;
            }
        }

        private void TopPanel_MouseDownDrag(object sender, MouseButtonEventArgs e) => DragMove();
        
        private void ExitButton_Click(object sender, RoutedEventArgs e) => Close();

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            var isMaximized = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            WindowState = isMaximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try {
                currentProgress = 1;
                Words.AllCombinations = string.Empty;
                WordDisplayLog.Text = string.Empty;
                Words.CombineWords();
            } catch {
                PopupDialog.Visibility = Visibility.Visible;
                PopupDialogTitle.Text = "WARNING!";
                PopupDialogMessage.Text = "Remember to select a file!";
            }
        }

        private void PopupDialogButtonClose_Click(object sender, RoutedEventArgs e)
        {
            PopupDialog.Visibility = Visibility.Hidden;
        }

        private int currentProgress = 1;
        private void UpdateProgress(int progress, int length) {
            currentProgress += progress;
            Dispatcher.Invoke(() => {
                ProgressBar.Maximum = length;
                ProgressBar.Value = currentProgress;
                ProgressDisplay.Text = $"{currentProgress}/{length}";
            });
        }

        private void FinishedProgress() {
            PopupDialog.Visibility = Visibility.Visible;
            PopupDialogTitle.Text = "FINISHED!";
            PopupDialogTitle.Foreground = new BrushConverter().ConvertFrom("#57D9A3") as Brush;
            PopupDialogMessage.Text = "Finished Combinations";
        }

        private void UpdateAllCombinations() {
            Dispatcher.Invoke(() => {
                WordDisplayLog.Text = Words.AllCombinations;
            });
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Environment.CurrentDirectory + "/output.txt", Words.AllCombinations);
        }
    }
}
