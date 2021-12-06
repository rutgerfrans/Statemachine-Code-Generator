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
    class ReadXml
    {
        //[USECASE 7: Inlezen XML]
        //[USECASE 8: Doorlopen XML]

        public Node CurrentPosition;
        readonly public List<Edge> EdgeList = new List<Edge>();
        readonly public List<Node> NodeList = new List<Node>();
        public string FileLocation;
        public XmlDocument doc = new XmlDocument();

        //Laad lijsten met de nodes en edges van de flowchart.
        public void LoadLists()
        {
            doc.Load(FileLocation);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    foreach (XmlNode childNode2 in childNode.ChildNodes)
                    {
                        foreach (XmlNode childNode3 in childNode2.ChildNodes)
                        {
                            string id = childNode.Attributes[0].Value;
                            char[] characters = id.ToArray();
                            //data van xml toekennen aan element object
                            //data van object in database schrijven
                            if (childNode3.Attributes.Count >= 1)//controleren of er een type is om vervolgens de types toetekennen
                            {
                                string attr = childNode3.Attributes[0].Value;
                                var value = attr.Split(Convert.ToChar(".")).ToList();
                                string shapetype = value[value.Count - 1];
                                NodeList.Add(new Node(childNode.Attributes[0].Value, childNode.InnerText, shapetype));
                            }
                            else if (Convert.ToString(characters[0]) == "e")// checken of het een edge is (relation)
                            {
                                EdgeList.Add(new Edge(childNode.Attributes[0].Value, childNode.InnerText, "Relation", childNode.Attributes[1].Value, childNode.Attributes[2].Value));
                            }
                        }
                    }
                }
            }
        }

        //Zoekt de relatie(s) van de currentnode.
        public void FindRelation(Node CurrentNode)
        {
            //edgesFromNode bevat alle edges die vertrekken vanaf die node
            var edgesFromNode = EdgeList.Where(x => x.source == CurrentNode.id);
            foreach (var edge in edgesFromNode)
            {
                //targetnode is de node waar de edge van edgesFormNode naar wijst
                var targetNode = NodeList.FirstOrDefault(x => x.id == edge.target);
                if (targetNode != null)
                {
                    //als de edge wel of geen naam heeft
                    if (!string.IsNullOrEmpty(edge.name))
                    {
                        //voegt de edge toe aan de bijbehorende relaties van de currentnode
                        CurrentNode.AddRelation(targetNode, edge.source, edge.name, edge.id);
                    }
                    else
                    {
                        //voegt de edge toe aan de bijbehorende relaties van de currentnode
                        CurrentNode.AddRelation(targetNode, edge.source, edge.name, edge.id);
                    }
                }
                else
                {
                    //error, pijl gaat niet naar ander blok
                }
            }
            foreach (var node in NodeList)
            {
                if (node.type == "start1")
                {
                    //textbox.Text = node.id + "\t" + node.name;
                    CurrentPosition = node;
                }
            }
        }

        //Zoekt de values van de relatie(s) van de currentnode.
        public void FindValues(Node currentnode)
        {
            List<string> DoubleCheckList = new List<string>();
            foreach (var relation in currentnode.relations)
            {
                var RelationValues = relation.name.Split(new char[] { '!', '&', ' ', '|'}).ToList();
                foreach (var Value in RelationValues)
                {
                    if (Value != "")
                    {
                        if (currentnode.type == "decision")
                        {
                            if (Value.Contains("=") || Value.Contains("<") || Value.Contains(">"))// is integer value
                            {
                                string[] IntValue = Value.Split(new string[] { ">", "<", "==" }, StringSplitOptions.None);
                                if (!DoubleCheckList.Contains(IntValue[0]))
                                {
                                    relation.AddIntInputValues(currentnode.id, IntValue[0], 0);
                                    DoubleCheckList.Add(IntValue[0]);
                                }
                                else
                                {
                                    relation.AddIntInputValues(currentnode.id, IntValue[0], 0);
                                }
                            }
                            else// is bool value
                            {
                                string[] BoolValue = Value.Split(new string[] { "!" }, StringSplitOptions.None);
                                if (!DoubleCheckList.Contains(BoolValue[0]))
                                {
                                    relation.AddBoolInputValues(currentnode.id, BoolValue[0], false);
                                    DoubleCheckList.Add(BoolValue[0]);
                                }
                                else
                                {
                                    relation.AddBoolInputValues(currentnode.id, BoolValue[0], false);
                                }
                            }
                        }
                    }
                }
            }
        }

        //Voegt de nodeids toe waar één value voor komt.
        //In de nodelist zoekt hij per node naar de values.
        //Met doublechecklist checkt hij op dubbele values en bij welke nodes deze voorkomen.
        public void FindIds(List<Node> nodelist, List<InputValue> DoubleCheckList)
        {
            List<string> tempcnlist = new List<string>();

            //alle values worden toegevoegd aan de doublechecklist
            foreach (Node node in nodelist)
            {
                foreach (Relation relation in node.relations)
                {
                    foreach (InputValue value in relation.values)
                    {
                        DoubleCheckList.Add(value);
                    }
                }
            }
            foreach (Node node in nodelist)
            {
                foreach (Relation relation in node.relations)
                {
                    //iedere value krijgt de id van de node waar de value vandaan komt.
                    foreach (InputValue value in relation.values)
                    {
                        value.currentnodeids.Add(value.currentnodeid);
                    }

                    // alle dubbele values worden verwijderd
                    foreach (InputValue value in relation.values)
                    {
                        List<InputValue> DoubleValue = DoubleCheckList.Where(x => x.name.Equals(value.name)).ToList();
                        if (DoubleValue.Count > 1)
                        {
                            DoubleCheckList.Remove(value);
                        }
                    }
                }
            }


            foreach (Node node in nodelist)
            {
                foreach (Relation relation in node.relations)
                {
                    foreach (InputValue value in relation.values)
                    {
                        List<InputValue> DoubleValue = DoubleCheckList.Where(x => x.name.Equals(value.name) && x.currentnodeid != value.currentnodeid).ToList();
                        if (DoubleValue.Count != 0)
                        {
                            foreach (InputValue dvalue in DoubleValue)
                            {
                                value.currentnodeids.Add(dvalue.currentnodeid);
                            }
                        }
                    }
                }
            }
            foreach (Node node in nodelist)
            {
                foreach (Relation relation in node.relations)
                {
                    foreach (InputValue value in relation.values)
                    {
                        List<InputValue> DoubleValue = DoubleCheckList.Where(x => x.name.Equals(value.name) && x.currentnodeid != value.currentnodeid).ToList();
                        if (DoubleValue.Count > 1)
                        {
                            foreach (var dvalue in DoubleValue)
                            {
                                DoubleCheckList.Remove(dvalue);
                            }
                        }
                    }
                }
            }
        }
    }
}
