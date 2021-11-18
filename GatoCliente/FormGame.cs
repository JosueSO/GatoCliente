using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace GatoCliente
{
    public partial class FormGame : Form
    {
        Client Client;
        List<Label> LabelPositions;
        string other_player_label;
        bool my_turn;

        public FormGame(Client _client, string labelText)
        {
            InitializeComponent();

            Client = _client;
            LabelPlayer.Text = labelText;

            my_turn = labelText == "O";

            other_player_label = labelText == "O" ? "X" : "O";

            LabelPositions = new List<Label>
            {
                LabelPos1,
                LabelPos2,
                LabelPos3,
                LabelPos4,
                LabelPos5,
                LabelPos6,
                LabelPos7,
                LabelPos8,
                LabelPos9,
            };

            Thread thread = new Thread(Listen);
            thread.Start();
        }

        private void Listen()
        {
            string message;
            MessageClass messageClass;
            bool play = true;

            while (play)
            {
                message = Client.Receive();

                messageClass = JsonConvert.DeserializeObject<MessageClass>(message);

                if (messageClass.MessageType == "play")
                {
                    Invoke(new MethodInvoker(() => SetPosition(messageClass.MessageContent)));
                }
                else if (messageClass.MessageType == "finish")
                {
                    play = false;
                    Invoke(new MethodInvoker(() => FinishGame(messageClass.MessageContent)));
                }
            }
        }

        public void FinishGame(string message)
        {
            if (message == LabelPlayer.Text)
            {
                message = "!Felicidades, eres el ganador!";
            }
            else
            {
                message = "Échale más ganas la próxima vez";
            }

            MessageBox.Show(message);

            my_turn = false;
        }

        public void SetPosition(string aux_pos)
        {
            my_turn = true;

            int pos = int.Parse(aux_pos);

            Label label = LabelPositions[pos - 1];

            label.Text = other_player_label;
            label.Enabled = false;
        }

        private void LabelPos_Click(object sender, EventArgs e)
        {
            if (my_turn)
            {
                my_turn = false;

                string labelKey = ((Label)sender).Name;

                labelKey = labelKey.Substring(labelKey.Length - 1);

                int pos = int.Parse(labelKey);

                Label label = LabelPositions[pos - 1];

                label.Text = LabelPlayer.Text;
                label.Enabled = false;

                MessageClass messageClass = new MessageClass
                {
                    MessageType = "play",
                    MessageContent = labelKey
                };

                string message = JsonConvert.SerializeObject(messageClass);

                Client.Send(message);
            }
        }
    }
}
