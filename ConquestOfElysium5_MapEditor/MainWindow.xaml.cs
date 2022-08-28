using ConquestOfElysium5_MapEditor.Core;
using ConquestOfElysium5_MapEditor.View;
using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Windows.Forms.Integration;
using System.Windows.Interop;

namespace ConquestOfElysium5_MapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static System.Windows.Controls.Label _Message;

        public MainWindow()
        {
            InitializeComponent();

            _Message = Message;

            Loaded += MainWindow_Loaded;
        }

        public static void Post_Message(string message)
            => _Message.Content = message;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            // This is a super hacky way to create a GL context.
            OpenTK.GLControl glcontrol;
            View.Child = new WindowsFormsHost() { Child = glcontrol = new OpenTK.GLControl() };
            glcontrol.Invalidate();
            */

            try
            {
                COE5.Data_Directory =
                    COE5_Data_Directory
                    .Get__COE5_Data_Directory();
            }
            catch (Exception ex)
            {
                Show__Error_Popup($"Failure to find COE5 Data Directory: {ex.Message}");
            }

            try
            {
                COE5.Roaming_Directory =
                    COE5_Roaming_Directory
                    .Get__COE5_Roaming_Directory();
            }
            catch (Exception ex)
            {
                Show__Error_Popup($"Failure to find COE5 Roaming Directory: {ex.Message}");
            }
        }

        private void File__Load_Map(object sender, RoutedEventArgs e)
        {
            Process_Directory_Dialog<OpenFileDialog>
            (
                new OpenFileDialog() { FileName = COE5.Roaming_Directory.Maps_Directory },
                (dialog) => 
                {
                    COE5_Map map =
                        COE5.Roaming_Directory
                        .Load_Map(dialog.FileName);

                    COE5_Map_Editor.Load_Map(map);
                }
            );
        }

        private void File__Select_COE5_Roaming_Directory(object sender, RoutedEventArgs e)
        {
            Process_Directory_Dialog
            (
                new FolderBrowserDialog(),
                dialog =>
                {
                    COE5.Roaming_Directory =
                        COE5_Roaming_Directory
                        .Get__COE5_Roaming_Directory(dialog.SelectedPath);
                }
            );
        }

        private void File__Select_COE5_Data_Directory(object sender, RoutedEventArgs e)
        {
            Process_Directory_Dialog
            (
                new FolderBrowserDialog(),
                dialog =>
                {
                    COE5.Data_Directory =
                        COE5_Data_Directory
                        .Get__COE5_Data_Directory(dialog.SelectedPath);
                }
            );
        }

        private void Process_Directory_Dialog<TDialog>
        (
            TDialog dialog,
            Action<TDialog> callback
        )
        where TDialog : CommonDialog
        {
            DialogResult result =
                dialog.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK)
                return;

            try
            {
                callback(dialog);
            }
            catch (Exception ex)
            {
                Show__Error_Popup(ex.Message);
            }
        }

        private void Show__Error_Popup(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
