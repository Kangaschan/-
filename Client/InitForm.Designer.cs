namespace Client
{
    partial class InitForm
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
            textBoxIP = new TextBox();
            labelIP = new Label();
            label1 = new Label();
            textBoxPort = new TextBox();
            label2 = new Label();
            textBoxUsername = new TextBox();
            buttonSubmit = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // textBoxIP
            // 
            textBoxIP.Location = new Point(124, 22);
            textBoxIP.Name = "textBoxIP";
            textBoxIP.Size = new Size(234, 23);
            textBoxIP.TabIndex = 0;
            textBoxIP.Text = "127.0.0.1";
            // 
            // labelIP
            // 
            labelIP.AutoSize = true;
            labelIP.Location = new Point(33, 25);
            labelIP.Name = "labelIP";
            labelIP.Size = new Size(64, 15);
            labelIP.TabIndex = 1;
            labelIP.Text = "IP сервера";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 54);
            label1.Name = "label1";
            label1.Size = new Size(35, 15);
            label1.TabIndex = 3;
            label1.Text = "Порт";
            // 
            // textBoxPort
            // 
            textBoxPort.Location = new Point(124, 51);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Size = new Size(234, 23);
            textBoxPort.TabIndex = 2;
            textBoxPort.Text = "5000";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 83);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 5;
            label2.Text = "Имя";
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(124, 80);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(234, 23);
            textBoxUsername.TabIndex = 4;
            textBoxUsername.Text = "User";
            // 
            // buttonSubmit
            // 
            buttonSubmit.Location = new Point(254, 134);
            buttonSubmit.Name = "buttonSubmit";
            buttonSubmit.Size = new Size(104, 23);
            buttonSubmit.TabIndex = 6;
            buttonSubmit.Text = "Подтвердить";
            buttonSubmit.UseVisualStyleBackColor = true;
            buttonSubmit.Click += buttonSubmit_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(33, 134);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(104, 23);
            buttonCancel.TabIndex = 7;
            buttonCancel.Text = "Отмена";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // InitForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 186);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSubmit);
            Controls.Add(label2);
            Controls.Add(textBoxUsername);
            Controls.Add(label1);
            Controls.Add(textBoxPort);
            Controls.Add(labelIP);
            Controls.Add(textBoxIP);
            Name = "InitForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Welcome";
            Load += InitForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxIP;
        private Label labelIP;
        private Label label1;
        private TextBox textBoxPort;
        private Label label2;
        private TextBox textBoxUsername;
        private Button buttonSubmit;
        private Button buttonCancel;
    }
}