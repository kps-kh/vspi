using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using System;
using System.Diagnostics;

namespace vspi
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MusicPicker.ItemsSource = new[] { "1live", "xin", "chinese", "remix" };
            MusicPicker.SelectedIndex = 0;
            PlayerPicker.ItemsSource = new[] { "brave", "vlc" };
            PlayerPicker.SelectedIndex = 0;
            XscreenPicker.ItemsSource = new[] { "blank", "unblank", "off", "on" };
            XscreenPicker.SelectedIndex = 0;
            ExecutePicker.ItemsSource = new[] { "sleep +1", "white", "dogs", "BT on", "BT off", "halt", "reboot" };
            ExecutePicker.SelectedIndex = 0;
        }

        private void RunCommand(string cmd)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{cmd}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
            }
            catch (Exception ex)
            {
                ShowMessage("Error", ex.Message);
            }
        }

        private void ShowMessage(string title, string message)
        {
            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var dialog = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = message, Margin = new Thickness(10), TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                        okButton
                    }
                }
            };

            okButton.Click += (_, __) => dialog.Close();
            dialog.ShowDialog(this);
        }

        private void OnVolumeUpClicked(object? sender, RoutedEventArgs e) =>
            RunCommand("pactl set-sink-volume @DEFAULT_SINK@ +5%");

        private void OnVolumeDownClicked(object? sender, RoutedEventArgs e) =>
            RunCommand("pactl set-sink-volume @DEFAULT_SINK@ -5%");

        private void OnMusicPicker(object? sender, RoutedEventArgs e)
        {
            if (MusicPicker.SelectedIndex == -1)
            {
                ShowMessage("Missing", "Select a music option.");
                return;
            }

            string? cmd = MusicPicker.SelectedItem?.ToString();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string? play = cmd switch
            {
                "1live" => $"killall -9 vlc; cvlc -LZ {path}/Music/einslive.xspf",
                "xin" => $"killall -9 vlc; cvlc --no-video -LZ {path}/Videos/xin",
                "chinese" => $"killall -9 vlc; cvlc -LZ {path}/Music/chinesetraditional",
                "remix" => $"killall -9 vlc; cvlc -LZ {path}/Music/remix",
                _ => null
            };

            if (play != null)
                RunCommand(play);
        }

        private void OnPlayerPicker(object? sender, RoutedEventArgs e)
        {
            if (PlayerPicker.SelectedIndex == -1)
            {
                ShowMessage("Missing", "Select a player.");
                return;
            }

            string? cmd = PlayerPicker.SelectedItem?.ToString();

            string? control = cmd switch
            {
                // Toggle pause/resume for VLC
                "vlc" => "dbus-send --type=method_call --dest=org.mpris.MediaPlayer2.vlc /org/mpris/MediaPlayer2 org.mpris.MediaPlayer2.Player.PlayPause",
                "brave" => "playerctl play-pause -p brave",
                _ => null
            };

            if (control != null)
                RunCommand(control);
        }
        private void OnStopClicked(object? sender, RoutedEventArgs e) =>
                RunCommand("killall -9 vlc");

        private void OnXscreenPicker(object? sender, RoutedEventArgs e)
        {
            if (XscreenPicker.SelectedIndex == -1)
            {
                ShowMessage("Missing", "Select a command.");
                return;
            }

            string? cmd = XscreenPicker.SelectedItem?.ToString();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string? action = cmd switch
            {
                "blank" => "pgrep xscreensaver >/dev/null 2>&1 && xscreensaver-command -activate || (nohup xscreensaver -no-splash >/dev/null 2>&1 &)",
                "unblank" => "xscreensaver-command -deactivate",
                "off" => "xscreensaver-command -exit",
                "on" => "nohup xscreensaver -no-splash > /dev/null 2>&1 &",
                _ => null
            };

            if (action != null)
                RunCommand(action);
        }

        private void OnExecutePicker(object? sender, RoutedEventArgs e)
        {
            if (ExecutePicker.SelectedIndex == -1)
            {
                ShowMessage("Missing", "Select a command.");
                return;
            }

            string? cmd = ExecutePicker.SelectedItem?.ToString();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string? action = cmd switch
            {
                "sleep +1" => "echo 'killall -9 vlc; playerctl pause; pgrep xscreensaver >/dev/null 2>&1 && xscreensaver-command -activate || (nohup xscreensaver -no-splash >/dev/null 2>&1 &)' | at now + 60 minutes > /dev/null 2>&1",
                "white" => $"cvlc -LZ {path}/Music/white.mp3",
                "dogs" => $"cvlc -LZ {path}/Music/2dogs.mp3",
                "BT on" => "rfkill unblock bluetooth",
                "BT off" => "rfkill block bluetooth",
                "halt" => "systemctl poweroff",
                "reboot" => "systemctl reboot",
                _ => null
            };

            if (action != null)
                RunCommand(action);
        }
    }
}
