using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace CS2ServerStarter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TextBoxCS2Path.Text = ConfigurationManager.AppSettings.Get("CS2Path");
            TextBoxMap.Text = ConfigurationManager.AppSettings.Get("Map");
            TextBoxArgs.Text = ConfigurationManager.AppSettings.Get("Args");


            ComboBoxGameType.SelectedIndex = int.Parse(ConfigurationManager.AppSettings.Get("Gametype"));
            ComboBoxGameMode.SelectedIndex = int.Parse(ConfigurationManager.AppSettings.Get("Gamemode"));


            Console.WriteLine("Config loaded");
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                    settings.Add(key, value);
                else
                    settings[key].Value = value;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //start writing
            string[] lines =
            {
                "@echo off",
                ".\\game\\bin\\win64\\cs2.exe " + TextBoxArgs.Text + " +map " + TextBoxMap.Text + " +game_type " +
                ComboBoxGameType.SelectedIndex + " +game_mode " + ComboBoxGameMode.SelectedIndex + " -insecure",
                "@echo off"
            };

            // Set a variable to the path.
            var docPath = TextBoxCS2Path.Text;

            // Write the string array to a new file named "Start.bat".
            using (var outputFile = new StreamWriter(Path.Combine(docPath, "Start.bat")))
            {
                foreach (var line in lines)
                    outputFile.WriteLine(line);
            }
            //stop writing

            //start server
            var cmdargs = @"Start.bat";
            var proc1 = new ProcessStartInfo();
            proc1.UseShellExecute = false;
            proc1.WorkingDirectory = TextBoxCS2Path.Text;

            proc1.FileName = @"cmd.exe";
            proc1.Verb = "runas";
            proc1.Arguments = "/c " + cmdargs;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;

            Process.Start(proc1);
        }

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            AddOrUpdateAppSettings("CS2Path", TextBoxCS2Path.Text);
            AddOrUpdateAppSettings("Map", TextBoxMap.Text);
            AddOrUpdateAppSettings("Args", TextBoxArgs.Text);
            AddOrUpdateAppSettings("Gamemode", ComboBoxGameMode.Text);
            AddOrUpdateAppSettings("Gametype", ComboBoxGameType.Text);
            Console.WriteLine("Config update");
        }
    }
}