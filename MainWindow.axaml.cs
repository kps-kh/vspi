using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace vspi;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MusicPicker.ItemsSource = new[] { "Playlist", "xin", "chinese", "remix" };
        MusicPicker.SelectedIndex = 0;

        PlayerPicker.ItemsSource = new[] { "brave", "mpv", "vlc" };
        PlayerPicker.SelectedIndex = 0;

        XscreenPicker.ItemsSource = new[] { "blank", "unblank", "off", "on" };
        XscreenPicker.SelectedIndex = 0;

        ExecutePicker.ItemsSource = new[] { "sleep +1", "white", "dogs", "BT on", "BT off", "halt", "reboot" };
        ExecutePicker.SelectedIndex = 0;
    }

    void RunCommand(string cmd)
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

    void SendMpvIpcCommand(string json)
    {
        try
        {
            using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            socket.Connect(new UnixDomainSocketEndPoint("/tmp/mpvsocket"));
            socket.Send(Encoding.UTF8.GetBytes(json + "\n"));

            var buffer = new byte[1024];
            if (socket.Poll(500_000, SelectMode.SelectRead))
            {
                int bytesRead = socket.Receive(buffer);
                Debug.WriteLine("MPV Response: " + Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
        }
        catch (Exception ex)
        {
            ShowMessage("MPV IPC Error", ex.Message);
        }
    }

    void ShowMessage(string title, string message)
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

    void OnVolumeUpClicked(object? sender, RoutedEventArgs e) =>
        RunCommand("pactl set-sink-volume @DEFAULT_SINK@ +5%");

    void OnVolumeDownClicked(object? sender, RoutedEventArgs e) =>
        RunCommand("pactl set-sink-volume @DEFAULT_SINK@ -5%");

    void OnStopClicked(object? sender, RoutedEventArgs e) =>
        RunCommand("killall mpv vlc");

    void OnMusicPicker(object? sender, RoutedEventArgs e)
    {
        if (MusicPicker.SelectedIndex == -1)
        {
            ShowMessage("Missing", "Select a music option.");
            return;
        }

        var cmd = MusicPicker.SelectedItem?.ToString();
        var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var play = cmd switch
        {
            "Playlist" => $"killall mpv vlc; mpv --playlist={path}/Music/playlist.m3u --loop-playlist=inf --shuffle --input-ipc-server=/tmp/mpvsocket",
            "xin" => $"killall mpv vlc; mpv --no-video --loop-playlist=inf --shuffle --input-ipc-server=/tmp/mpvsocket {path}/Videos/xin",
            "chinese" => $"killall mpv vlc; mpv --no-video --loop-playlist=inf --shuffle --input-ipc-server=/tmp/mpvsocket {path}/Music/chinesetraditional",
            "remix" => $"killall mpv vlc; mpv --no-video --loop-playlist=inf --shuffle --input-ipc-server=/tmp/mpvsocket {path}/Music/remix",
            _ => null
        };

        if (play != null)
            RunCommand(play);
    }

    void OnPlayerPicker(object? sender, RoutedEventArgs e)
    {
        if (PlayerPicker.SelectedIndex == -1)
        {
            ShowMessage("Missing", "Select a player.");
            return;
        }

        var cmd = PlayerPicker.SelectedItem?.ToString();

        if (cmd == "mpv")
        {
            SendMpvIpcCommand("{\"command\": [\"cycle\", \"pause\"], \"request_id\": 1}");
        }
        else
        {
            var control = cmd switch
            {
                "brave" => "playerctl play-pause -p brave",
                "vlc" => "dbus-send --type=method_call --dest=org.mpris.MediaPlayer2.vlc /org/mpris/MediaPlayer2 org.mpris.MediaPlayer2.Player.PlayPause",
                _ => null
            };

            if (control != null)
                RunCommand(control);
        }
    }

    void OnXscreenPicker(object? sender, RoutedEventArgs e)
    {
        if (XscreenPicker.SelectedIndex == -1)
        {
            ShowMessage("Missing", "Select a command.");
            return;
        }

        var cmd = XscreenPicker.SelectedItem?.ToString();

        var action = cmd switch
        {
            "blank" => "nohup xscreensaver -no-splash >/dev/null 2>&1 & sleep 1.5; xscreensaver-command -activate",
            "unblank" => "xscreensaver-command -deactivate",
            "off" => "xscreensaver-command -exit",
            "on" => "nohup xscreensaver -no-splash > /dev/null 2>&1 &",
            _ => null
        };

        if (action != null)
            RunCommand(action);
    }

    void OnExecutePicker(object? sender, RoutedEventArgs e)
    {
        if (ExecutePicker.SelectedIndex == -1)
        {
            ShowMessage("Missing", "Select a command.");
            return;
        }

        var cmd = ExecutePicker.SelectedItem?.ToString();
        var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var action = cmd switch
        {
            "sleep +1" => "echo 'killall mpv vlc; playerctl pause; pgrep xscreensaver >/dev/null 2>&1 && xscreensaver-command -activate || (nohup xscreensaver -no-splash >/dev/null 2>&1 &)' | at now + 60 minutes > /dev/null 2>&1",
            "white" => $"killall mpv vlc; mpv --no-video --loop-playlist=inf --shuffle --input-ipc-server=/tmp/mpvsocket {path}/Music/white.mp3",
            "dogs" => $"killall mpv vlc; mpv --no-video --loop-playlist=inf --shuffle --input-ipc-server=/tmp/mpvsocket {path}/Music/2dogs.mp3",
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