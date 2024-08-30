using System.Timers;

namespace Chess.HelperClasses
{
    public class ChessTimer
    {
        private System.Timers.Timer timer;
        private int min;
        private int sec;
        private Label label;

        public event Action? OnGameOver;

        public ChessTimer(int minutes, int seconds, Label label)
        {
            min = minutes;
            sec = seconds;
            timer = new System.Timers.Timer
            {
                Interval = 1000
            };
            timer.Elapsed += OnTimeEvent!;
            this.label = label;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void OnTimeEvent(object sender, ElapsedEventArgs e)
        {
            sec--;
            if (min == 0 && sec < 0)
            {
                sec = 0;
                timer.Stop();
                OnGameOver?.Invoke();
                return;
            }
            if (sec < 0)
            {
                sec = 59;
                min--;
            }
            label.Invoke(new Action(() => label.Text = min.ToString().PadLeft(2, '0') + ":" + sec.ToString().PadLeft(2, '0')));
        }
    }
}
