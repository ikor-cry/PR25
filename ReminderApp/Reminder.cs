using System;
using System.Collections.Generic;
using System.Text;

namespace ReminderApp
{
    public class Reminder
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string Text { get; set; }
        public bool IsShown { get; set; }
    }
}
