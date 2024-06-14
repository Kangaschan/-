using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class InitForm : Form
    {
        public bool quit;
        public string ip;
        public int port;
        public string Username;
        public InitForm()
        {
            quit = false;
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            quit = true;
            this.Close();
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            ip = textBoxIP.Text;
            port = Convert.ToInt32(textBoxPort.Text);
            Username = textBoxUsername.Text;
            this.Close();
        }

        private void InitForm_Load(object sender, EventArgs e)
        {

        }

       
    }
}
