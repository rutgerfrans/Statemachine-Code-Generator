using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace statemachine
{
    class ExportStmCsharp
    {
        public void Export(List<Node> List)
        {
            FolderBrowserDialog SelFolder = new FolderBrowserDialog();
            if (SelFolder.ShowDialog() == DialogResult.OK)
            {
                string genproj = SelFolder.SelectedPath + "/genproj.cs";
                string genprojOverride = SelFolder.SelectedPath + "/genprojOverride.cs";
                string genprojInputValues = SelFolder.SelectedPath + "/genprojInputValues.cs";
                using (FileStream fs = File.Create(genproj))
                {
                    WriteGenProj(List, fs);
                }
                using (FileStream fs = File.Create(genprojOverride))
                {
                    WriteOverride(fs, List);
                }
                using (FileStream fs = File.Create(genprojInputValues))
                {
                    WriteInputValues(List, fs);
                }
                MessageBox.Show("Statemachine succesvol geëxporteerd naar " + SelFolder.SelectedPath);
            }
        }

        //genproj.cs
        private void WriteGenProj(List<Node> List, FileStream fs)
        {
            AddText(fs, "using System.Timers;\nusing System;\nusing NLog;\n");
            AddText(fs, "namespace statemachine\n{\n\tpartial class genproj\n\t{\n");
            WriteStartTimer(List, fs);
            WriteStep(List, fs);
            WriteUpdateStatemachine(List, fs);
            WriteFunctions(List, fs);
        }
        private void WriteStartTimer(List<Node> List, FileStream fs)
        {
            foreach (Node node in List)
            {
                if (node.type == "start1")
                {
                    AddText(fs, $"\t\tprivate string currentnode = \"{node.name}\";\n\t\tprivate inputvalue inputs = new inputvalue();"
                        + "\n\t\tprivate static Logger logger = LogManager.GetCurrentClassLogger();"
                        + "\n\t\tprivate static Timer t;\n");
                }
            }

            AddText(fs, "\n\t\tpublic void Start(int interval)\n\t\t{"
                + "\n\t\t\tt = new Timer(interval);"
                + "\n\t\t\tt.Elapsed += OnTimedEvent;"
                + "\n\t\t\tt.AutoReset = true;"
                + "\n\t\t\tt.Enabled = true;"
                + "\n\t\t\tConsole.ReadLine();"
                + "\n\t\t\tt.Stop();"
                + "\n\t\t\tt.Dispose();"
                + "\n\t\t\tlogger.Debug(\"Terminating...\");"
                + "\n\t\t}\n");

            AddText(fs, "\n\t\tprivate void OnTimedEvent(Object source, ElapsedEventArgs e)\n\t\t{"
                + "\n\t\t\tstep();"
                + "\n\t\t}\n");
        }
        private void WriteStep(List<Node> List, FileStream fs)
        {
            AddText(fs, "\n\t\tpublic void step()\n\t\t{\n\t\t\tstring newNode = null;\n\t\t\tswitch(currentnode)\n\t\t\t{\n");
            foreach (Node node in List)
            {
                string newRelationName;
                if (node.relations.Count > 1)
                {
                    AddText(fs, "\t\t\t\tcase " + $"\"{node.name}\":\n");
                    for (int i = 0; i < node.relations.Count; i++)
                    {
                        if (node.relations[i].values.Count > 1)
                        {
                            newRelationName = "";
                            var templist = node.relations[i].name.Split(' ').ToList();
                            if (templist != null)
                            {
                                for (int x = 0; x < templist.Count; x++)
                                {
                                    if (templist[x].Length > 1)
                                    {
                                        if (x != templist.Count - 1)
                                        {
                                            if (templist[x].Contains("!"))
                                            {
                                                var newval = templist[x].Split('!');
                                                newRelationName += "!inputs." + newval[1] + " && ";
                                            }
                                            else
                                            {
                                                newRelationName += "inputs." + templist[x] + " && ";
                                            }
                                        }
                                        else
                                        {
                                            if (templist[x].Contains("!"))
                                            {
                                                var newval = templist[x].Split('!');
                                                newRelationName += "!inputs." + newval[1];
                                            }
                                            else
                                            {
                                                newRelationName += "inputs." + templist[x];
                                            }
                                        }
                                    }
                                }
                            }
                            InsertLogicStepper(i, newRelationName, node.relations[i].next.name, node.relations.Count, fs);
                        }
                        else
                        {
                            newRelationName = "";
                            if (node.relations[i].name.Contains("!"))
                            {
                                var nameval = node.relations[i].name.Split('!');
                                newRelationName = "!inputs." + nameval[1];
                            }
                            else
                            {
                                newRelationName = "inputs." + node.relations[i].name;
                            }
                            InsertLogicStepper(i, newRelationName, node.relations[i].next.name, node.relations.Count, fs);
                        }
                    }
                }
                else
                {
                    AddText(fs, "\t\t\t\tcase " + $"\"{node.name}\":" + $"\n\t\t\t\t\tnewNode = \"{node.relations[0].next.name}\";\n\t\t\t\t\tbreak;\n");
                }
            }
            AddText(fs, "\n\t\t\t}\n");
            AddText(fs, "\t\t\tif(newNode != null && newNode !=currentnode)"
                + "\n\t\t\t{\n\t\t\t\tcurrentnode = newNode;"
                + "\n\t\t\t\tUpdateStatemachine();"
                + "\n\t\t\t\tlogger.Debug(\"Currentstate: \" + currentnode);"
                + "\n\t\t\t}\n\t\t}\n");
        }
        private void InsertLogicStepper(int i, string newRelationName, string nextnodeName, int RelCount, FileStream fs)
        {
            if (i == 0)
            {
                AddText(fs, $"\t\t\t\t\tif({newRelationName})" +
                    "\n\t\t\t\t\t{" +
                    $"\n\t\t\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t\t\t}\n");
            }
            else if (i != RelCount - 1)
            {
                AddText(fs, $"\t\t\t\t\telse if({newRelationName})" +
                    "\n\t\t\t\t\t{" +
                    $"\n\t\t\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t\t\t}\n");
            }
            else if (i == RelCount - 1)
            {
                AddText(fs, $"\t\t\t\t\telse if({newRelationName})" +
                    "\n\t\t\t\t\t{" +
                    $"\n\t\t\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t\t\t}" +
                    "\n\t\t\t\t\tbreak;\n");
            }
        }
        private void WriteUpdateStatemachine(List<Node> List, FileStream fs)
        {
            AddText(fs, "\t\tprivate void UpdateStatemachine()\n\t\t{\n\t\t\tswitch(currentnode)\n\t\t\t{\n");
            foreach (Node node in List)
            {
                AddText(fs, "\t\t\t\tcase " +
                    $"\"{node.name}\":" +
                    $"\n\t\t\t\t\t{node.name}(inputs);" +
                    "\n\t\t\t\t\tbreak;\n\n");
            }
            AddText(fs, "\n\t\t\t}\n\t\t}\n");
        }
        private void WriteFunctions(List<Node> List, FileStream fs)
        {
            foreach (Node node in List)
            {
                AddText(fs, "\n\t\tpublic virtual void " + node.name + "(inputvalue val)\n\t\t{\n\t\t}\n");
            }
            AddText(fs, "\n\t}\n}");
        }

        //genprojInputValues.cs
        private void WriteInputValues(List<Node> List, FileStream fs)
        {
            AddText(fs, "using System;\nnamespace statemachine\n{\n\tpublic class inputvalue\n\t{\n");
            WriteValues(List, fs);
        }
        private void WriteValues(List<Node> List, FileStream fs)
        {
            List<InputValue> DoubleCheckList = new List<InputValue>();
            foreach (Node node in List)
            {
                foreach (Relation relation in node.relations)
                {
                    foreach (InputValue value in relation.values)
                    {
                        switch (value)
                        {
                            case BoolInputValue bvalue:
                                if (DoubleCheckList.Count == 0)
                                {
                                    DoubleCheckList.Add(bvalue);
                                }
                                else
                                {
                                    var dvalues = DoubleCheckList.Where(x => x.name == value.name).ToList();
                                    if (dvalues.Count == 0)
                                    {
                                        DoubleCheckList.Add(bvalue);
                                    }
                                }
                                break;
                            case IntInputValue ivalue:
                                if (DoubleCheckList.Count == 0)
                                {
                                    DoubleCheckList.Add(ivalue);
                                }
                                else
                                {
                                    var dvalues = DoubleCheckList.Where(x => x.name == value.name).ToList();
                                    if (dvalues.Count == 0)
                                    {
                                        DoubleCheckList.Add(ivalue);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            foreach (var inputval in DoubleCheckList)
            {
                switch (inputval)
                {
                    case BoolInputValue bvalue:
                        AddText(fs, $"\t\tpublic bool {bvalue.name} = {bvalue.value.ToString().ToLower()};\n");
                        break;
                    case IntInputValue ivalue:
                        AddText(fs, $"\t\tpublic int {ivalue.name} = {ivalue.value};\n");
                        break;
                }
            }
            AddText(fs, "\n\t}\n}");
        }

        //OverrideGenproj.cs
        private void WriteOverride(FileStream fs, List<Node> List)
        {
            AddText(fs, "using statemachine;"
                + "\nnamespace PROJECTNAME\n{"
                + "\n\tclass OverrideclassGenproj : genproj\n\t{\n");

            foreach (var node in List)
            {
                AddText(fs, "\n\t\tpublic override void " + node.name + "(inputvalue val)\n\t\t{\n\t\t}\n");
            }
            AddText(fs, "\t}\n}");

        }

        private void AddText(FileStream fs, string text)
        {
            Byte[] info = new UTF8Encoding(true).GetBytes(text);
            fs.Write(info, 0, info.Length);
        }
    }
}
