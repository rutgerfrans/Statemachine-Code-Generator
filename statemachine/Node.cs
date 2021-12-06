using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace statemachine
{
    class Node
    {
        public string id;
        public string name;
        public string type;
        public List<Relation> relations = new List<Relation>();

        public Node(string Id, string Name, string Type)
        {
            id = Id;
            name = Name;
            type = Type;
        }

    
        //Stepper die naar volgende node gaat als relatie waar is.
        public Node Step(string NextNodeID)
        {       
            var validRelation = relations.FirstOrDefault(x => x.valid);
            if (validRelation != null && validRelation.next.id == NextNodeID)
            {
                return validRelation.next;
            }
            else
            {
                return this;
            }
        }

        //Voegt bijhorende relatie aan lijst van node toe.
        public void AddRelation(Node Next, string Source, string Name, string Id)
        {
            relations.Add(new Relation(Next, Source, Name, Id));
        } 
    }
}
