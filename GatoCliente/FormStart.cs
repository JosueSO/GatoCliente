using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace GatoCliente
{
    public partial class FormStart : Form
    {
        Client Client;

        public FormStart()
        {
            InitializeComponent();

            Client = new Client("localhost", 3000);
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            LabelMessage.Visible = true;

            ButtonStart.Enabled = false;

            Thread thread = new Thread(Listen);

            Client.Start();

            thread.Start();
        }

        public void Listen()
        {
            string message;
            MessageClass messageClass;

            do
            {
                message = Client.Receive();

                messageClass = JsonConvert.DeserializeObject<MessageClass>(message);
            } while (messageClass.MessageType != "start");

            Invoke(new MethodInvoker(() => StartGame(messageClass.MessageContent)));
        }

        public void StartGame(string labelText)
        {
            this.Hide();

            FormGame formGame = new FormGame(Client, labelText);

            formGame.ShowDialog();

            this.Close();
        }
    }
}
