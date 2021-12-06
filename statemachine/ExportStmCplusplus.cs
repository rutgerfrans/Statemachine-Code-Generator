using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace statemachine
{
    class ExportStmCplusplus
    {
        public void Export(List<Node> List)
        {
            FolderBrowserDialog SelFolder = new FolderBrowserDialog();
            if (SelFolder.ShowDialog() == DialogResult.OK)
            {
                string genprojcpp = SelFolder.SelectedPath + "/genproj.cpp";
                string genprojh = SelFolder.SelectedPath + "/genproj.h";
                string genprojInputValuescpp = SelFolder.SelectedPath + "/genprojInputValues.cpp";
                string genprojInputValuesh = SelFolder.SelectedPath + "/genprojInputValues.h";

                using (FileStream fs = File.Create(genprojcpp))
                {
                    WriteGenProjcpp(fs);
                }
                using (FileStream fs = File.Create(genprojh))
                {
                    WriteGenProjh(List, fs);
                }
                using (FileStream fs = File.Create(genprojInputValuescpp))
                {
                    WriteGenProjInputValuescpp(fs);
                }
                using (FileStream fs = File.Create(genprojInputValuesh))
                {
                    WriteGenProjInputValuesh(List, fs);
                }

                MessageBox.Show("Statemachine succesvol geëxporteerd naar " + SelFolder.SelectedPath);
            }
        }


        //genproj.cpp
        private void WriteGenProjcpp(FileStream fs)
        {
            AddText(fs, "#include \"genproj.h\"");
        }

        //genproj.h
        private void WriteGenProjh(List<Node> List, FileStream fs)
        {
            //all includes
            AddText(fs, "#pragma once\n#include <iostream>\n#include <stream>\n#include <thread>" +
                "\n#include \"genprojInputValues.h\"\n#include \"spdlog/spdlog.h\"\n#inlcude \"spdlog/sinks/basic_file_sink.h\"");
            foreach (Node node in List)
            {
                if (node.type == "start1")
                {
                    AddText(fs, "\nclass genproj\n{\npublic:" +
                        $"\n\tstd::string currentnode = \"{node.name}\";" +
                        "\n\tgenprojInputValues *inputs = new genprojInputValues();" +
                        "\n\tstd::string newNode;");
                }
            }
            WriteStart(fs);
            WriteStep(List, fs);
            WriteUpdateStatemachine(List, fs);
            WriteFunctions(List, fs);
        }
        private void WriteStart(FileStream fs)
        {
            AddText(fs, "\n\tvoid start(int interval){" +
                "\n\t\t//Aanpasbaar op voorkeur//" +
                "\n\t\tspdlog::set_pattern(\"[% H:% M:% S % z][% n][% ^---% L-- -%$][thread % t] % v\");" +
                "\n\t\tauto file_logger = spdlog::basic_logger_mt(\"basic_logger\", \"logs/basic.txt\");" +
                "\n\t\tspdlog::set_level(spdlog::level::debug);");
            AddText(fs, "\n\t\twhile(true){" +
                "\n\t\t\tstep();" +
                "\n\t\t\tstd::this_thread::sleep_for(std::chrono::milliseconds(interval));" +
                "\n\t\t}\n\t}");
        }
        private void WriteStep(List<Node> List, FileStream fs)
        {
            AddText(fs, "\n\tvoid step(){");
            for (int i = 0; i < List.Count; i++)
            {
                string newRelationName;
                if (i == 0)
                {
                    AddText(fs, $"\n\t\tif (currentnode == \"{List[i].name}\")" +
                        "{\n");
                }
                else
                {
                    AddText(fs, $"\n\t\telse if (currentnode == \"{List[i].name}\")" +
                        "{\n");
                }
                if (List[i].relations.Count > 1)
                {
                    for (int x = 0; x < List[i].relations.Count; x++)
                    {
                        if (List[i].relations[x].values.Count > 1)
                        {
                            newRelationName = "";
                            var templist = List[i].relations[x].name.Split(' ').ToList();
                            if (templist != null)
                            {
                                for (int y = 0; y < templist.Count; y++)
                                {
                                    if (templist[y].Length > 1)
                                    {
                                        if (y != templist.Count - 1)
                                        {
                                            if (templist[y].Contains("!"))
                                            {
                                                var newval = templist[y].Split('!');
                                                newRelationName += "!inputs->" + newval[1] + " && ";
                                            }
                                            else
                                            {
                                                newRelationName += "inputs->" + templist[y] + " && ";
                                            }
                                        }
                                        else
                                        {
                                            if (templist[y].Contains("!"))
                                            {
                                                var newval = templist[y].Split('!');
                                                newRelationName += "!inputs->" + newval[1];
                                            }
                                            else
                                            {
                                                newRelationName += "inputs->" + templist[y];
                                            }
                                        }
                                    }
                                }
                            }
                            InsertLogicStepper(x, newRelationName, List[i].relations[x].next.name, List[i].relations.Count, fs);
                        }
                        else
                        {
                            newRelationName = "";
                            if (List[i].relations[x].name.Contains("!"))
                            {
                                var nameval = List[i].relations[x].name.Split('!');
                                newRelationName = "!inputs->" + nameval[1];
                            }
                            else
                            {
                                newRelationName = "inputs->" + List[i].relations[x].name;
                            }
                            InsertLogicStepper(x, newRelationName, List[i].relations[x].next.name, List[i].relations.Count, fs);
                        }
                    }
                }
                else
                {
                    AddText(fs, $"\t\t\tnewNode = \"{List[i].relations[0].next.name}\";" +
                        "\n\t\t}");
                }
            }
            AddText(fs, "\n\t\tif (newNode != \"\" && newNode != currentnode){" +
                "\n\t\t\tcurrentnode = newNode;" +
                "\n\t\t\tUpdateStatemachine();" +
                "\n\t\t\tspdlog::debug(currentnode)\n\t\t}\n\t}\n");
        }
        private void InsertLogicStepper(int x, string newRelationName, string nextnodeName, int RelCount, FileStream fs)
        {
            if (x == 0)
            {
                AddText(fs, $"\t\t\tif({newRelationName})" +
                    "\n\t\t\t{" +
                    $"\n\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t}\n");
            }
            else if (x != RelCount - 1)
            {
                AddText(fs, $"\t\t\telse if({newRelationName})" +
                    "\n\t\t\t{" +
                    $"\n\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t}\n");
            }
            else if (x == RelCount - 1)
            {
                AddText(fs, $"\t\t\telse if({newRelationName})" +
                    "\n\t\t\t{" +
                    $"\n\t\t\t\tnewNode = \"{nextnodeName}\";" +
                    "\n\t\t\t}\n\t\t}");
            }
        }
        private void WriteUpdateStatemachine(List<Node> List, FileStream fs)
        {
            AddText(fs, "\n\tvoid UpdateStatemachine() {");
            for (int i = 0; i < List.Count; i++)
            {
                if (i == 0)
                {
                    AddText(fs, $"\n\t\tif (currentnode == \"{List[i].name}\")" + "{" +
                        $"\n\t\t\t{List[i].name}(inputs);" + "\n\t\t}");
                }
                else
                {
                    AddText(fs, $"\n\t\telse if (currentnode == \"{List[i].name}\")" + "{" +
                        $"\n\t\t\t{List[i].name}(inputs);" + "\n\t\t}");
                }
            }
            AddText(fs, "\n\t}");

        }
        private void WriteFunctions(List<Node> List, FileStream fs)
        {
            foreach (Node node in List)
            {
                AddText(fs, "\n\tvoid " + node.name + "(genprojInputValue *val){\n\t}\n");
            }
            AddText(fs, "\n};");
        }

        //genprojInputValues.cpp
        private void WriteGenProjInputValuescpp(FileStream fs)
        {
            AddText(fs, "#include \"genprojInputValues.h\"");
        }

        //genprojInputValues.h
        private void WriteGenProjInputValuesh(List<Node> List, FileStream fs)
        {
            AddText(fs, "#pragma once\nclass genprojInputValues\n{\npublic:\n");
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
                        AddText(fs, $"\tbool {bvalue.name} = {bvalue.value.ToString().ToLower()};\n");
                        break;
                    case IntInputValue ivalue:
                        AddText(fs, $"\tint {ivalue.name} = {ivalue.value};\n");
                        break;
                }
            }
            AddText(fs, "\n};");
        }

        private void AddText(FileStream fs, string text)
        {
            Byte[] info = new UTF8Encoding(true).GetBytes(text);
            fs.Write(info, 0, info.Length);
        }

    }
}
