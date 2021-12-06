using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace statemachine
{
    public partial class Form1 : Form
    {
        //[USECASE 2: Aflezen Currentstate]
        //[USECASE 3: Aanpassen values]
        //[USECASE 4: Aflezen Loggings]

        readonly Logs Log = new Logs();
        readonly ExpEval ExpEva = new ExpEval();
        readonly ReadXml Rxml = new ReadXml();
        readonly ExportStmCplusplus StmC = new ExportStmCplusplus();
        readonly ExportStmCsharp StmCsharp = new ExportStmCsharp();
        readonly ExportStmJava StmJava = new ExportStmJava();

        public Form1(string path)
        {
            Rxml.FileLocation = path;
            InitializeComponent();
        }

        //handmatige stepper.
        private void Button1_Click(object sender, EventArgs e)
        {
            stepper();      
        }

        //update alle states.
        private void UpdateStatusView(ListViewItem Log)
        {
            listView1.Items.Clear();
            textBox1.Text = Rxml.CurrentPosition.id + "\t" + Rxml.CurrentPosition.name;
            foreach (var rel in Rxml.CurrentPosition.relations)
            {
                ListViewItem viewrel = CreateViewRel(rel.id, rel.source, rel.next.id, rel.name);
                listView1.Items.Add(viewrel);
            }
            ExpEva.ExpressionEvaluator(listView1, Rxml.CurrentPosition);
            listView2.Items.Add(Log);
        }

        //initizialiseren programma.
        private void Form1_Load(object sender, EventArgs e)
        {
            //init lists
            Rxml.LoadLists();
            foreach (var node in Rxml.NodeList)
            {
                Rxml.FindRelation(node);
                Rxml.FindValues(node);
            }
            Rxml.FindIds(Rxml.NodeList, ExpEva.DoubleCheckList);

            //init first state
            textBox1.Text = Rxml.CurrentPosition.id + "\t" + Rxml.CurrentPosition.name;
            ListViewItem viewrel = CreateViewRel(Rxml.CurrentPosition.relations[0].id, Rxml.CurrentPosition.relations[0].source, Rxml.CurrentPosition.relations[0].next.id, Rxml.CurrentPosition.relations[0].name);
            listView1.Items.Add(viewrel);

            //init values
            ExpEva.ExpressionEvaluator(listView1, Rxml.CurrentPosition);
            ExpEva.UpdateDataGrid(dataGridView1);

            InitTimer();
        }

        //export logging knop.
        private void Button2_Click(object sender, EventArgs e)
        {
            Log.Logging(listView2);
        }

        //checked relation op valid of invalid.
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ExpEva.ChangeIntValue(Rxml.NodeList, dataGridView1, e);
            ExpEva.ExpressionEvaluator(listView1, Rxml.CurrentPosition);
        }

        //value aanpassen.
        private void Button4_Click(object sender, EventArgs e)
        {
            ExpEva.ChangeBoolValue(dataGridView1, Rxml.NodeList);
        }

        //reset statemachine knop.
        private void button5_Click(object sender, EventArgs e)
        {
            ClearAll();
            Form2 StatemachineMenu = new Form2();
            StatemachineMenu.Show();
            StatemachineMenu.FormClosed += new FormClosedEventHandler(delegate { Close(); });
            this.Hide();
        }

        //reset statemachine.
        private void ClearAll()
        {
            Rxml.EdgeList.Clear();
            Rxml.NodeList.Clear();
            ExpEva.DoubleCheckList.Clear();
        }

        //initializeren timer.
        public void InitTimer()
        {
            Timer timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 2000;
            timer1.Start();
        }

        //automatische stepper.
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                stepper();
            }
        }

        //export statemachine button.
        private void button6_Click(object sender, EventArgs e)
        {
            string code = ShowDialog("C#","C++","Java","");
            switch (code)
            {
                case "C#":
                    StmCsharp.Export(Rxml.NodeList);
                    break;
                case "C++":
                    StmC.Export(Rxml.NodeList);
                    break;
                case "Java":
                    StmJava.Export(Rxml.NodeList);
                    break;
                case null:
                    break;
            }
        }

        //Geeft keuze menu voor code weer.
        public string ShowDialog(string Csharp, string Cplusplus, string Java, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 200;
            prompt.Height = 180;
            prompt.Text = caption;

            FlowLayoutPanel panel = new FlowLayoutPanel();
            CheckBox chk1 = new CheckBox() { Text = Csharp};
            CheckBox chk2 = new CheckBox() { Text = Cplusplus};
            CheckBox chk3 = new CheckBox() { Text = Java};
            chk1.CheckStateChanged += (sender, e) => { if (chk1.Checked) chk2.Checked = false; chk3.Checked = false; };
            chk2.CheckStateChanged += (sender, e) => { if (chk2.Checked) chk3.Checked = false; chk1.Checked = false; };
            chk3.CheckStateChanged += (sender, e) => { if (chk3.Checked) chk1.Checked = false; chk2.Checked = false; };

            Button gen = new Button() { Text = "Generate" };
            gen.Click += (sender, e) => { prompt.Close(); };
            Button cancel = new Button() { Text = "Cancel" };
            cancel.Click += (sender, e) => { chk1.Checked = false; chk2.Checked = false; chk3.Checked = false; prompt.Close(); };
            Label panelName = new Label() { Text = "Selecteer een taal:" }; 
            prompt.Icon = null;
            panel.Height = 180;
            panel.Controls.Add(panelName);
            panel.Controls.Add(chk1);
            panel.Controls.Add(chk2);
            panel.Controls.Add(chk3);
            panel.SetFlowBreak(chk1, true);
            panel.SetFlowBreak(chk2, true);
            panel.SetFlowBreak(chk3, true);
            panel.Controls.Add(gen);
            panel.Controls.Add(cancel);
            prompt.Controls.Add(panel);
            prompt.StartPosition = FormStartPosition.CenterParent;
            prompt.ShowDialog();

            if (chk1.Checked && chk2.Checked == false && chk3.Checked == false)
            {
                return chk1.Text;
            }
            else if (chk2.Checked && chk1.Checked == false && chk3.Checked == false)
            {
                return chk2.Text;
            }
            else if (chk3.Checked && chk1.Checked == false && chk2.Checked == false)
            {
                return chk3.Text;
            }
            else
            {
                return null;
            }
        }

        //creëert een nieuwe log.
        private ListViewItem CreateLog(string datetime, string source, string target, string name)
        {
            ListViewItem log = new ListViewItem(new string[] { datetime
                , source + " to " + target
                , name
            });  
            return log;
        }

        //creëert een listview item voor de bijbehorende relaties.
        private ListViewItem CreateViewRel(string id, string source, string target, string name)
        {
            ListViewItem ViewRel = new ListViewItem(new string[] { id
                , source + " to " + target
                , name
            });
            return ViewRel;
        }

        //stepper, stepped bij een valid relation naar de volgende node.
        private void stepper()
        {
            for (int i = 0; i < Rxml.CurrentPosition.relations.Count; i++)
            {
                if (Rxml.CurrentPosition.relations[i].valid)
                {
                    ListViewItem log = CreateLog(DateTime.Now.ToString("H:mm:ss"), Rxml.CurrentPosition.relations[i].source, Rxml.CurrentPosition.relations[i].next.id, Rxml.CurrentPosition.name);
                    Rxml.CurrentPosition = Rxml.CurrentPosition.Step(Rxml.CurrentPosition.relations[i].next.id);
                    UpdateStatusView(log);
                    i = Rxml.CurrentPosition.relations.Count;
                }
            }
        }
    }
}
