using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class StringVerifier
    {
        public event EventHandler Succeeded;
        public event EventHandler Cancelled;

        #region Properties and Variables

        #endregion

        public StringVerifier()
        {
            InitializeComponent();
            GenerateString();
        }

        private void VerifyClick(object sender, RoutedEventArgs e)
        {
            if (_generatedString != EnteredTextBox.Text)
            {
                GenerateString();
                return;
            }

            if (Succeeded != null)
                Succeeded(this, null);

            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            if (Cancelled != null)
                Cancelled(this, null);

            this.Close();
        }

        private static readonly Random Random = new Random((int)DateTime.UtcNow.Ticks);
        private string _generatedString;

        private void GenerateString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < 8; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
                builder.Append(ch);
            }

            _generatedString = builder.ToString();

            StringToVerifyTextBlock.Text = _generatedString;
        }


    }
}

