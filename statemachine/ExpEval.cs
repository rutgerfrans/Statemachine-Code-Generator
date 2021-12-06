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
using CodingSeb.ExpressionEvaluator;

namespace statemachine
{
    class ExpEval
    {
        readonly public List<InputValue> DoubleCheckList = new List<InputValue>();
        //[USECASE 12: Expressie Evaluator]


        //Checked of de conditie van de relatie van de currentposition valid of invalid is.
        //Dit doormiddel van de library van codingseb.
        public void ExpressionEvaluator(ListView ListView1, Node currentposition)
        {
            if (currentposition != null)
            {
                for (int i = 0; i < currentposition.relations.Count; i++)
                {
                    if (currentposition.relations.Count > 1)
                    {
                        string expression = currentposition.relations[i].name;
                        ExpressionEvaluator evaluator = new ExpressionEvaluator();
                        evaluator.Variables.Clear();
                        foreach (InputValue value in currentposition.relations[i].values)
                        {
                            switch (value)
                            {
                                case BoolInputValue boolvalue:
                                    evaluator.Variables.Add(value.name, boolvalue.value);
                                    break;
                                case IntInputValue intvalue:
                                    evaluator.Variables.Add(value.name, intvalue.value);
                                    break;
                            }
                        }
                        if (evaluator.Evaluate(expression).ToString() == "True")
                        {
                            currentposition.relations[i].valid = true;
                            ListView1.Items[i].BackColor = Color.Green;
                        }
                        else if (evaluator.Evaluate(expression).ToString() == "False")
                        {
                            currentposition.relations[i].valid = false;
                            ListView1.Items[i].BackColor = Color.Red;
                        }
                    }
                    else if (currentposition.relations.Count == 1)
                    {
                        currentposition.relations[i].valid = true;
                        ListView1.Items[i].BackColor = Color.Green;
                    }
                }
            }
        }

        //Na elke overgang update deze functie het datagrid.
        public void UpdateDataGrid(DataGridView datagridview)
        {
            ClearDataGrid(datagridview);
            foreach (InputValue item in DoubleCheckList)
            {
                string idval = "";
                var Doublevalue = DoubleCheckList.Where(x => x == item).ToList();
                if (Doublevalue.Count < 2)
                {
                    switch (item)
                    {
                        case BoolInputValue bitem:
                            foreach (string id in bitem.currentnodeids)
                            {
                                idval += id;
                            }
                            datagridview.Rows.Add(idval, bitem.name, bitem.value);
                            break;
                        case IntInputValue iitem:
                            foreach (string id in iitem.currentnodeids)
                            {
                                idval += id;
                            }
                            datagridview.Rows.Add(idval, iitem.name, iitem.value);
                            break;
                    }
                }
            }
        }

        //Maakt het datagrid leeg.
        private void ClearDataGrid(DataGridView datagridview)
        {
            datagridview.Rows.Clear();
            datagridview.Refresh();
        }

        //Veranderd de geselecteerde integer value naar de gewenste waarde.
        public void ChangeIntValue(List<Node> NodeList, DataGridView datagridview, DataGridViewCellEventArgs e)
        {
            foreach (var node in NodeList)
            {
                foreach (var relation in node.relations)
                {
                    foreach (InputValue value in relation.values)
                    {
                        if (value.name == datagridview.Rows[e.RowIndex].Cells[1].Value.ToString())
                        {
                            var doubleValue = DoubleCheckList.Where(x => x.name == value.name).ToList();
                            switch (value)
                            {
                                case IntInputValue ivalue:
                                    ivalue.value = Convert.ToInt32(datagridview.Rows[e.RowIndex].Cells[2].Value);
                                    foreach (IntInputValue item in doubleValue)
                                    {
                                        item.value = Convert.ToInt32(datagridview.Rows[e.RowIndex].Cells[2].Value);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        //Veranderd de geselecteerde bool value naar de gewenste waarde.
        public void ChangeBoolValue(DataGridView datagridview, List<Node> nodelist)
        {
            List<InputValue> Allinputvalues = new List<InputValue>();
            fillinputvaluelist(nodelist, Allinputvalues);


            bool done = false;
            for (int i = 0; i < datagridview.Rows.Count; i++)
            {
                if (datagridview.Rows[i].Selected)
                {
                    foreach (Node node in nodelist)
                    {
                        foreach (Relation relation in node.relations)
                        {
                            foreach  (InputValue value in relation.values)
                            {
                                if (datagridview.Rows[i].Cells[1].Value.ToString() == value.name && !done)
                                {
                                    var doubleValue = Allinputvalues.Where(x => x.name == value.name).ToList();
                                    switch (value)
                                    {
                                        case BoolInputValue bvalue:
                                            if (doubleValue.Count > 1)
                                            {
                                                if (!bvalue.value)
                                                {
                                                    foreach (BoolInputValue item in doubleValue)
                                                    {
                                                        item.value = true;
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (BoolInputValue item in doubleValue)
                                                    {
                                                        item.value = false;
                                                    }
                                                }
                                                datagridview.Rows[i].Cells[2].Value = bvalue.value;
                                                datagridview.Refresh();
                                                Allinputvalues.Clear();
                                            }
                                            else
                                            {
                                                if (bvalue.value == false)
                                                {
                                                    bvalue.value = true;
                                                }
                                                else if (bvalue.value == true)
                                                {
                                                    bvalue.value = false;
                                                }
                                                datagridview.Rows[i].Cells[2].Value = bvalue.value;
                                                datagridview.Refresh();
                                                Allinputvalues.Clear();
                                            }
                                            done = true;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //vult een lijst met alle inputvalues, ook de dubbele
        private void fillinputvaluelist(List<Node> Nodelist, List<InputValue> Inputvaluelist)
        {
            foreach (var node in Nodelist)
            {
                foreach (var rel in node.relations)
                {
                    foreach (InputValue val in rel.values)
                    {
                        Inputvaluelist.Add(val);
                    }
                }
            }
        }
    }
}
