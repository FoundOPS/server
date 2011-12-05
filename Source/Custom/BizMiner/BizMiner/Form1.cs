using System.Windows.Forms;

namespace BizMiner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static void ConvertPDFToTextButtonClick(object sender, System.EventArgs e)
        {
            PDFLoader.ConvertPDFFilesToTextFiles();
        }

        private void LoadDataButtonClick(object sender, System.EventArgs e)
        {
            var dataLineItems = PDFLoader.LoadDataLineItems();
            this.dataGridView1.DataSource = dataLineItems;
        }

    }
}
