using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace ConquestOfElysium5_MapEditor.View
{
    /// <summary>
    /// This bad billy is passed by reference to Core
    /// from View, to serve as a callback means to
    /// inform the user via Progress bar and Message.
    /// </summary>
    public class User_Notification
    {
        public event EventHandler<string> Message_Relayed;
        public event EventHandler<DoubleAnimation> Progress_Relayed;

        internal void Relay_Message(string message)
        {
            Message_Relayed?.Invoke(this, message);
        }

        internal void Relay_Progress(double progress, double duration)
        {
            Duration anim_duration = new Duration(TimeSpan.FromSeconds(duration));
            Progress_Relayed?.Invoke(this, new DoubleAnimation(progress, anim_duration));
        }
    }
}
