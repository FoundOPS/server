using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.Controls
{
    public partial class SaveDiscardCancel
    {
        public SaveDiscardCancel()
        {
            InitializeComponent();
        }

        public Button SaveButton
        {
            get { return Save; }
        }

        public Button DiscardButton
        {
            get { return Discard; }
        }

        public Button CancelButton
        {
            get { return Cancel; }
        }
    }
}

