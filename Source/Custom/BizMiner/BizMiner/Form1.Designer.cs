namespace BizMiner
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.convertPDFToTextButton = new System.Windows.Forms.Button();
            this.loadDataButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AccessibleName = "";
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 30);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(881, 550);
            this.dataGridView1.TabIndex = 0;
            // 
            // convertPDFToTextButton
            // 
            this.convertPDFToTextButton.Location = new System.Drawing.Point(389, 1);
            this.convertPDFToTextButton.Name = "convertPDFToTextButton";
            this.convertPDFToTextButton.Size = new System.Drawing.Size(194, 23);
            this.convertPDFToTextButton.TabIndex = 1;
            this.convertPDFToTextButton.Text = "Convert PDF files to Text";
            this.convertPDFToTextButton.UseVisualStyleBackColor = true;
            this.convertPDFToTextButton.Click += new System.EventHandler(ConvertPDFToTextButtonClick);
            // 
            // loadDataButton
            // 
            this.loadDataButton.Location = new System.Drawing.Point(43, 1);
            this.loadDataButton.Name = "loadDataButton";
            this.loadDataButton.Size = new System.Drawing.Size(194, 23);
            this.loadDataButton.TabIndex = 2;
            this.loadDataButton.Text = "Load Data";
            this.loadDataButton.UseVisualStyleBackColor = true;
            this.loadDataButton.Click += new System.EventHandler(this.LoadDataButtonClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 580);
            this.Controls.Add(this.loadDataButton);
            this.Controls.Add(this.convertPDFToTextButton);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button convertPDFToTextButton;
        private System.Windows.Forms.Button loadDataButton;
    }
}

