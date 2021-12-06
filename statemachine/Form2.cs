using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace statemachine
{
    public partial class Form2 : Form
    {
        //[USECASE 1: Uploaden XML]
        readonly ReadXml Rxml = new ReadXml();
        readonly Syntax Syn = new Syntax();
        public Form2()
        {
            InitializeComponent();
        }

        //================== Button2_Click() ===================
        //Deze functie activeert form2 en de daadwerkelijke statemachine
        private void Button2_Click(object sender, EventArgs e)
        {
            ResetStatemachine();
            if ((textBox1.Text != "") && File.Exists(textBox1.Text))
            {
                Rxml.FileLocation = textBox1.Text;


                Rxml.LoadLists();
                foreach (var node in Rxml.NodeList)
                {
                    Rxml.FindRelation(node);
                    Rxml.FindValues(node);
                }

                Syn.ErrorRec(Rxml.NodeList, Rxml.EdgeList);

                if (Syn.ErrorList.Count > 0)
                {
                    foreach (var error in Syn.ErrorList)
                    {
                        richTextBox1.Text += $"[Error][Nr:{error.id}][Type:{error.type}][{error.name}][Id:{error.elementid}][Name:{error.elementname}]\n";
                    }
                }
                else if (Syn.ErrorList.Count == 0)
                {
                    Form1 Statemachine = new Form1(textBox1.Text);
                    Statemachine.Show();
                    Statemachine.FormClosed += new FormClosedEventHandler(delegate { Close(); });
                    this.Hide();
                }
            }
            else
            {
                MessageBox.Show("Select or Insert valid .Graphml file.");
            }
        }

        //================== Button1_Click() ===================
        //Deze functie uploadt de geselecteerde file.
        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDir = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "Database files (*.graphml, *.accdb)|*.graphml;*.accdb",
                FilterIndex = 0,
                RestoreDirectory = true,
            };

            if (FileDir.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = FileDir.FileName;
            }
        }

        private void ResetStatemachine()
        {
            richTextBox1.Clear();
            Rxml.NodeList.Clear();
            Rxml.EdgeList.Clear();
            Syn.ErrorList.Clear();
            Syn.ErrorCounter = 0;
        }

    }
}
