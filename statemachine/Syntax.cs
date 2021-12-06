using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using CodingSeb.ExpressionEvaluator;

namespace statemachine
{
    class Syntax
    {
        public List<Error> ErrorList = new List<Error>();
        public int ErrorCounter = 0;
        public List<string> ForbiddenSymbolsRelationName = new List<string>() {"&&","||","@","#","$","^","*","-","_","+","=",@"\","/","<",">",",","?","`","~"};
        public List<string> ForbiddenSymbolsValue = new List<string>() {"=",">>","<<",">=","<=","&","|","@","#","$","^","*","-","_","+","=",@"\","/",",","?","`","~"," "};
        public void ErrorRec(List<Node> NodeList, List<Edge> EdgeList)
        {
            CheckStartNodes(NodeList);
            CheckNodeType(NodeList);
            CheckValidProcessNode(NodeList);
            CheckValidDicisionNode(NodeList);
            CheckExistingName(NodeList, EdgeList);
            CheckRelConditionSyntax(NodeList);
        }

        private void CheckStartNodes(List<Node> NodeList)
        {
            var StartNodes = NodeList.Where(x => x.type == "start1").ToList();

            if (StartNodes.Count > 1)
            {
                foreach (var node in StartNodes)
                {
                    ErrorCounter++;
                    string newNodeName = Regex.Replace(node.name, @"\t|\n|\r", " ");
                    Error newError = new Error(ErrorCounter, "L001", "To many start nodes", "Er zijn teveel start nodes aanwezig. Zorg dat er maar 1 startnode per flowchart aanweizg is.", node.id, newNodeName);
                    ErrorList.Add(newError);
                }
            }
            else if (StartNodes.Count == 0)
            {
                ErrorCounter++;
                Error newError = new Error(ErrorCounter, "L002", "Missing start node", "Er zijn geen start nodes aanwezig. Zorg dat er maximaal 1 startnode per flowchart aanwezig is.", "N/A", "N/A");
                ErrorList.Add(newError);
            }
        }
        private void CheckNodeType(List<Node> NodeList)
        {
            foreach (var node in NodeList)
            {
                if (node.type != "process" && node.type != "decision" && node.type != "start1")
                {
                    ErrorCounter++;
                    string newNodeName = Regex.Replace(node.name, @"\t|\n|\r", " ");
                    Error newError = new Error(ErrorCounter, "L003", "Invalid node type", "Node heeft verkeerde type, zorg dat je het type \"process\", \"decision\" of \"start\" gebruikt. ", node.id, newNodeName);
                    ErrorList.Add(newError);
                }
            }
        }
        private void CheckValidProcessNode(List<Node> NodeList)
        {
            foreach (var node in NodeList)
            {
                if (node.type == "process" && node.relations.Count > 1)
                {
                    ErrorCounter++;
                    string newNodeName = Regex.Replace(node.name, @"\t|\n|\r", " ");
                    Error newError = new Error(ErrorCounter, "L004", "Invalid process node", "Process node heeft te veel relaties. Zorg dat de process node maar 1 relatie bevat.", node.id, newNodeName);
                    ErrorList.Add(newError);
                }
                else if (node.type == "process" && node.relations.Count == 0)
                {
                    ErrorCounter++;
                    string newNodeName = Regex.Replace(node.name, @"\t|\n|\r", " ");
                    Error newError = new Error(ErrorCounter, "L007", "Ending Statemachine", "Een statemachine kan geen einde hebben. Zorg dat de flowchart geen einde heeft, maar in een cycles met idle states leeft.", node.id, newNodeName);
                    ErrorList.Add(newError);
                }
            }
        }
        private void CheckValidDicisionNode(List<Node> NodeList)
        {
            foreach (var node in NodeList)
            {
                if (node.type == "decision" && node.relations.Count == 1)
                {
                    ErrorCounter++;
                    string newNodeName = Regex.Replace(node.name, @"\t|\n|\r", " ");
                    Error newError = new Error(ErrorCounter, "L005", "Invalid decision node", "Decision node heeft te weinig relaties. Zorg dat de decision node meer dan 1 relatie bevat.", node.id, newNodeName);
                    ErrorList.Add(newError);
                }
                else if (node.type == "decision" && node.relations.Count == 0)
                {
                    ErrorCounter++;
                    string newNodeName = Regex.Replace(node.name, @"\t|\n|\r", " ");
                    Error newError = new Error(ErrorCounter, "L007", "Ending statemachine", "Een statemachine kan geen einde hebben. Zorg dat de flowchart geen einde heeft, maar in een cycles met idle states leeft.", node.id, newNodeName);
                    ErrorList.Add(newError);
                }
            }
        }
        private void CheckExistingName(List<Node> NodeList, List<Edge> EdgeList)
        {
            foreach (var node in NodeList)
            {
                if (node.name == "" || node.name == null)
                {
                    ErrorCounter++;
                    Error newError = new Error(ErrorCounter, "L006N", "Missing node name", "De node heeft geen naam. Zorg dat iedere node een naam bevat.", node.id, "N/A");
                    ErrorList.Add(newError);
                }
            }
            foreach (var edge in EdgeList)
            {
                if (edge.name == "" || edge.name == null)
                {
                    ErrorCounter++;
                    Error newError = new Error(ErrorCounter, "L006E", "Missing edge name", "De edge heeft geen naam. Zorg dat iedere edge een naam bevat.", edge.id, "N/A");
                    ErrorList.Add(newError);
                }
            }
        }
        private void CheckRelConditionSyntax(List<Node> NodeList)
        {
            foreach (var node in NodeList)
            {
                if (node.type == "decision")
                {
                    foreach (var relation in node.relations)
                    {
                        //Check opzet relation
                        CheckSpacesRelation(relation);
                        //CheckRelationOpzet(relation);

                        //Check opzet value
                        var RelationValues = relation.name.Split(new char[] { '!', '&', ' ', '|' }).ToList();
                        if (RelationValues.Count == 1)
                        {
                            CheckValueName(RelationValues[0], relation);
                            CheckValueOpzet(RelationValues[0], relation);
                        }
                        else if (RelationValues.Count > 1)
                        {
                            foreach (var value in RelationValues)
                            {
                                CheckValueName(value, relation);
                                CheckValueOpzet(value, relation);
                            }
                        }
                    }
                }
            }
        }
        private void CheckValueOpzet(string value, Relation relation)
        {

            //controleert helaas alleen met getallen: --> varx > 0  maar niet --> varx > vary.
            if ((value.Contains("==") | value.Contains(">") | value.Contains("<")) | value.Contains("=>") | value.Contains("=<"))
            {
                if (value != "" && value != null)
                {
                    string[] checkValue = value.Split(new string[] { ">", "<", "==" }, StringSplitOptions.None);
                    var ValidValueName = int.TryParse(checkValue[0], out _);
                    var ValidValue = int.TryParse(checkValue[1], out _);
                    if (ValidValueName || !ValidValue)
                    {
                        ErrorCounter++;
                        Error newError = new Error(ErrorCounter, "L008V", "Invalid value variable", "De value is verkeerd genoteerd. De variabele van de value wordt vergelijken met een ander verkeerd type variabele. ", relation.id, value);
                        ErrorList.Add(newError);
                    }
                }
            }
            else 
            {
                foreach (var sym in ForbiddenSymbolsValue)
                {
                    if (value.Contains(sym))
                    {
                        ErrorCounter++;
                        Error newError = new Error(ErrorCounter, "L008O", "Invalid value comparison operator", "De value is verkeerd genoteerd. De comparison operator wordt verkeerd gebruikt, controleer aan de hand van de handleiding de juiste notatie. ", relation.id, value);
                        ErrorList.Add(newError);
                    }
                }
            }
        }
        private void CheckValueName(string value, Relation relation)
        {
            if (value.Contains(" ") | value.Contains(".") | value.Contains("-") | value.Contains("_") | value.Contains("#") | value.Contains("$") | value.Contains("%") | value.Contains("^") | value.Contains("@") | value.Contains("*") | value.Contains("+"))
            {
                ErrorCounter++;
                Error newError = new Error(ErrorCounter, "L009", "Invalid value name", "De value is verkeerd genoteerd. Zorg dat de value alleen bestaat uit letters.", relation.id, relation.name);
                ErrorList.Add(newError);
            }
        }
        private void CheckSpacesRelation(Relation relation)
        {
            if (!relation.name.Contains(" ") && (relation.name.Contains("&") || relation.name.Contains("|")))
            {
                ErrorCounter++;
                Error newError = new Error(ErrorCounter, "L010", "Invalid relation missing spaces", "De relatie is verkeerd genoteerd. De relatie bevat de juiste comparison operators, maar er missen spaties tussen deze operator. Controleer aan de hand van de handleiding de juiste notatie.", relation.id, relation.name);
                ErrorList.Add(newError);
            }
        }
    }               
}
