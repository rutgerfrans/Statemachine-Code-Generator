using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace statemachine
{
    class ExportStmJava
    {
        public void Export(List<Node> List)
        {
            FolderBrowserDialog SelFolder = new FolderBrowserDialog();
            if (SelFolder.ShowDialog() == DialogResult.OK)
            {
                string genproj = SelFolder.SelectedPath + "/genproj.java";
                string genprojOverride = SelFolder.SelectedPath + "/genprojOverride.java";
                string genprojInputValues = SelFolder.SelectedPath + "/genprojInputValues.java";

                using (FileStream fs = File.Create(genproj))
                {
                    WriteGenProj(List, fs);
                }
                using (FileStream fs = File.Create(genprojInputValues))
                {
                    WriteGenProjInputValues(List, fs);
                }
                using (FileStream fs = File.Create(genprojOverride))
                {
                    WriteGenProjOverride(List, fs);
                }
                MessageBox.Show("Statemachine succesvol geëxporteerd naar " + SelFolder.SelectedPath);
            }
        }

        //genproj.java
        private void WriteGenProj(List<Node> List, FileStream fs)
        {
            AddText(fs, "import org.apache.logging.log4j.*;\npublic class genproj" +
                "\n{\n\tprivate static Logger log = LogManager.getLogger(genproj.class);" +
                "\n\tprivate String currentnode = \"Start\";" +
                "\n\tprivate genprojInputValues inputs = new genprojInputValues();\n");
            WriteStart(fs);
            WriteStep(List, fs);
            WriteUpdateStatemachine(List, fs);
            WriteFunctions(List, fs);
        }
        private void WriteStart(FileStream fs)
        {
            AddText(fs, "\n\tpublic void Start(int interval)\n\t{" +
                "\n\t\twhile(true)\n\t\t{" +
                "\n\t\t\ttry\n\t\t\t{" +
                "\n\t\t\t\tstep();" +
                "\n\t\t\t\tThread.sleep(interval);" +
                "\n\t\t\t}" +
                "\n\t\t\tcatch(InterruptedException e)" +
                "\n\t\t\t{\n\t\t\t}\n\t\t}\n\t}\n");
        }
        private void WriteStep(List<Node> List, FileStream fs)
        {
            AddText(fs, "\n\tpublic void step()\n\t{\n\t\tString newNode = null;\n\t\tswitch(currentnode)\n\t\t{\n");
            foreach (Node node in List)
            {
                string newRelationName;
                if (node.relations.Count > 1)
                {
                    AddText(fs, "\t\t\tcase " + $"\"{node.name}\":\n");
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
                    AddText(fs, "\t\t\tcase " + $"\"{node.name}\":" + $"\n\t\t\t\tnewNode = \"{node.relations[0].next.name}\";\n\t\t\t\tbreak;\n");
                }
            }
            AddText(fs, "\n\t\t}\n");
            AddText(fs, "\t\tif(newNode != null && newNode !=currentnode)"
                + "\n\t\t{\n\t\t\tcurrentnode = newNode;"
                + "\n\t\t\tUpdateStatemachine();"
                + "\n\t\t\tlog.debug(currentnode);"
                + "\n\t\t}\n\t}\n");
        }
        private void InsertLogicStepper(int i, string newRelationName, string nextnodeName, int RelCount, FileStream fs)
        {
            if (i == 0)
            {
                AddText(fs, $"\t\t\t\tif({newRelationName})" +
                    "\n\t\t\t\t{" +
                    $"\n\t\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t\t}\n");
            }
            else if (i != RelCount - 1)
            {
                AddText(fs, $"\t\t\t\telse if({newRelationName})" +
                    "\n\t\t\t\t{" +
                    $"\n\t\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t\t}\n");
            }
            else if (i == RelCount - 1)
            {
                AddText(fs, $"\t\t\t\telse if({newRelationName})" +
                    "\n\t\t\t\t{" +
                    $"\n\t\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t\t}" +
                    "\n\t\t\t\tbreak;\n");
            }
        }
        private void WriteFunctions(List<Node> List, FileStream fs)
        {
            foreach (Node node in List)
            {
                AddText(fs, "\n\tpublic void " + node.name + "(genprojInputValues val)\n\t{\n\t}\n");
            }
            AddText(fs, "\n}");
        }
        private void WriteUpdateStatemachine(List<Node> List, FileStream fs)
        {
            AddText(fs, "\tprivate void UpdateStatemachine()\n\t{\n\t\tswitch(currentnode)\n\t\t{\n");
            foreach (Node node in List)
            {
                AddText(fs, "\t\t\tcase " +
                    $"\"{node.name}\":" +
                    $"\n\t\t\t\t{node.name}(inputs);" +
                    "\n\t\t\t\tbreak;\n\n");
            }
            AddText(fs, "\n\t\t}\n\t}\n");
        }

        //genprojInputValues.java
        private void WriteGenProjInputValues(List<Node> List, FileStream fs)
        {
            AddText(fs, "public class genprojInputValues\n{\n");
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
                        AddText(fs, $"\tpublic boolean {bvalue.name} = {bvalue.value.ToString().ToLower()};\n");
                        break;
                    case IntInputValue ivalue:
                        AddText(fs, $"\tpublic int {ivalue.name} = {ivalue.value};\n");
                        break;
                }
            }
            AddText(fs, "\n}");
        }

        //genprojOverride.java
        private void WriteGenProjOverride(List<Node> List, FileStream fs)
        {
            AddText(fs, "public class genprojOverride extends genproj\n{\n");
            foreach (var node in List)
            {
                AddText(fs, "\n\tpublic void " + node.name + "(genprojInputValues val)\n\t{\n\t}\n");
            }
            AddText(fs, "\n}");
        }

        private void AddText(FileStream fs, string text)
        {
            Byte[] info = new UTF8Encoding(true).GetBytes(text);
            fs.Write(info, 0, info.Length);
        }
    }
}
