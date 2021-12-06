using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace statemachine
{
    class Logs
    {
        //[USECASE 9: Registreren Loggings]

        //exporteren loggings
        public void Logging(ListView listview)
        {
            SaveFileDialog FileToSave = new SaveFileDialog
            {
                FileName = "StatemachineLogs.txt",
                Filter = "Text File | *.txt",
            };


            if (FileToSave.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(FileToSave.OpenFile());
                writer.WriteLine("Tijd:\t\tOvergang:\tStatus:");
                for (int i = 0; i < listview.Items.Count; i++)
                {
                    writer.WriteLine(listview.Items[i].SubItems[0].Text + "\t" + listview.Items[i].SubItems[1].Text + "\t" + listview.Items[i].SubItems[2].Text);
                }
                writer.Dispose();
                writer.Close();
            }
        }
    }
}
