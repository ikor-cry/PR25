using ReminderApp;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ReminderApp
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Reminder> reminders =
            new ObservableCollection<Reminder>();

        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            RemindersGrid.ItemsSource = reminders;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(HourTextBox.Text, out int hour) ||
                !int.TryParse(MinuteTextBox.Text, out int minute))
            {
                MessageBox.Show("Введите часы и минуты числами!");
                return;
            }

            if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
            {
                MessageBox.Show("Некорректное время!");
                return;
            }

            if (string.IsNullOrWhiteSpace(TextReminderTextBox.Text))
            {
                MessageBox.Show("Введите текст напоминания!");
                return;
            }

            reminders.Add(new Reminder
            {
                Hour = hour,
                Minute = minute,
                Text = TextReminderTextBox.Text,
                IsShown = false
            });

            HourTextBox.Clear();
            MinuteTextBox.Clear();
            TextReminderTextBox.Clear();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RemindersGrid.SelectedItem is Reminder selected)
            {
                reminders.Remove(selected);
            }
            else
            {
                MessageBox.Show("Выберите напоминание!");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var reminder in reminders)
            {
                if (DateTime.Now.Hour == reminder.Hour &&
                    DateTime.Now.Minute == reminder.Minute &&
                    !reminder.IsShown)
                {
                    MessageBox.Show(reminder.Text, "Напоминание!");

                    reminder.IsShown = true;
                }
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (openDialog.ShowDialog() == true)
            {
                reminders.Clear();

                string[] lines = File.ReadAllLines(openDialog.FileName);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new string[] { "%%" },
                        StringSplitOptions.None);

                    if (parts.Length == 3 &&
                        int.TryParse(parts[0], out int hour) &&
                        int.TryParse(parts[1], out int minute))
                    {
                        reminders.Add(new Reminder
                        {
                            Hour = hour,
                            Minute = minute,
                            Text = parts[2],
                            IsShown = false
                        });
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt",
                DefaultExt = ".txt"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var lines = reminders.Select(r =>
                    $"{r.Hour:D2}%%{r.Minute:D2}%%{r.Text}");

                File.WriteAllLines(saveDialog.FileName, lines);

                MessageBox.Show("Напоминания сохранены!");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            timer.Stop();
            base.OnClosed(e);
        }
    }
}