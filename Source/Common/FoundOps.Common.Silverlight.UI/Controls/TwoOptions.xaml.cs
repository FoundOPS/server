using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.Controls
{
    public partial class TwoOptions
    {
        public TwoOptions()
            : this("My title!", "Why did no one setup my message?", "Jump", "Explode")
        {
        }

        public TwoOptions(string title, string message, string optionOne, string optionTwo)
        {
            InitializeComponent();
            this.Title = title;
            this.Message.Text = message;
            this.OptionOne.Content = optionOne;
            this.OptionTwo.Content = optionTwo;
        }

        public Button OptionOneButton
        {
            get { return OptionOne; }
        }

        public Button OptionTwoButton
        {
            get { return OptionTwo; }
        }
    }
}

