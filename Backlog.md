## v1.14.1 
Initial commit

## v1.14.2
Initial commit

## v1.15.1
Er is een algoritme gebouwd die waardes van relaties die dubbel voorkomen bij andere relaties detecteert.

En deze worden vervolgens maar als een waarde weergegeven in het datagrid.

## v1.15.2
### DoubleValues
Op het algoritme van v1.15.0 is een extensie gebouwd om 

de dubbele waardens een lijst te geven met de nodes waar ze voorkomen.

Voorbeeld:
> nputvalue2 {n23, send.succesfull, false} 
> inputvalue1 {n24, send.succesfull, false}
> inputvalue.currentnodeids{n23,n24}

### OrderValues
Het ordenen van nodes is gefixt.

## v.1.15.3
### Commenting
Er is in elke class commenting toegevoegd.

## v.1.16.1
### Symbo Syntax
Deze update is als voorbereiding voor het inplementeren van meerdere type inputvalues.

Het "=" symbool in de flowcharts zijn verwijderd.

Eerst diende deze als teken dat die conditie true wordt als de waarde waar het "=" symbool voor staat

ook een value heeft die true is.
Nu is dat stuk code aangepast en wordt er niet meer gezocht naar een "=" symbool, maar of de waarde van de conditie

geen "!" symbool bevat.

### Voorbeeld:
=products.toPick & !TypePicking.parallel

products.toPick & !TypePicking.parallel

Op deze manier kan het "=" symbool worden toegepast om integers mee te vergelijken.

Dus of een count van een lijst bijvoorbeeld gelijk staat aan een integer.

### Voorbeeld:
Queue.Lenght = 0

### inputvalue type
Om een nieuwe type inputvalue toe te voegen,

Heb ik in overleg met Marcel, child classes toegevoegd.

Er is nu een hoofdclasse "Inputvalue" die de attributes; currentnodeid en name bevat.

Ook bevat deze de lijst met de currentnodes.
De 2 childclasses zijn BoolInputValue en IntInputValue. Deze bevatten alletwee een attribute genaamd value.

Echter is deze bij BoolInputvalue een bool en bij IntInputvalue een intger.

### Voorbeeld:
#### Hoofdclass:

    class InputValue
    {
        public string currentnodeid;
        public string name;
        public List<string> currentnodeids = new List<string>();

        public InputValue(string CurrentnodeId, string Name)
        {
            currentnodeid = CurrentnodeId;
            name = Name;
        }
    }

#### ChildClasses:

    class BoolInputValue : InputValue
    {
        public bool value = false;

        public BoolInputValue(string CurrentnodeId, string Name, bool Value) :base(CurrentnodeId,Name)
        {     
            value = Value;
        }
    }

    class IntInputValue : InputValue
    {
        public int value;

        public IntInputValue(string CurrentnodeId, string Name, int Value) :base(CurrentnodeId,Name)
        {
            value = Value;
        }
    }

Vervolgens zijn alle stukken code die hier mee samen werken geupdate.

Vooral FindIds() en Outputstatemachine(). Deze zijn aangepast op de manier van zoeken in een lijst.

Er moet nu namelijk gezocht worden naar 2 verschillende objecten in een lijst. Dit doe ik doormiddel van switch type cases.

## v.1.16.2
### Changevalue
In de changevalue functie is een stuk code toegevoegd die check op interger inputvalues.

Als deze gedetecteerd worden, wordt de ingevoerde value van textbox2 gebruikt als nieuwe value

voor de geselecteerde value in het datagrid.

case IntInputValue ivalue:
	if (doubleValue.Count > 1)
	{
		foreach (IntInputValue item in doubleValue)
		{
                	item.value = Convert.ToInt32(textBox.Text);
                }
                datagridview.Rows[i].Cells[2].Value = ivalue.value;
                datagridview.Refresh();
        }
        else
        {
		ivalue.value = Convert.ToInt32(textBox.Text);
		datagridview.Rows[i].Cells[2].Value = ivalue.value;
		datagridview.Refresh();
	}
	break;

### Outputstatemachine
In de outputstatemachine functie is code bijgeschreven om relaties waar of onwaar te maken doormiddel

van integers inputvalues. De functie leest de values, die in changevalue zijn aangepast in, en past 

daarop de kleur en waarde van de relaties aan.

Deze functie werkt echter alleen nog voor relaties die enkel en alleen condities bevatten met integer values.

                    else if (intConditions != null)
                    {
                        foreach (var con in intConditions)
                        {
                            //gelijk aan
                            if (con.Contains("="))
                            {
                                var ConValue = con.Split('=').ToList();
                                foreach (InputValue value in DoubleCheckList)
                                {
                                    switch (value)
                                    {
                                        case IntInputValue bvalue:
                                            if (bvalue.name.Contains(ConValue[0]))
                                            {
                                                if (Convert.ToInt32(ConValue[1]) == Convert.ToInt32(bvalue.value.ToString()))
                                                {
                                                    ListView1.Items[i].BackColor = Color.Green;
                                                }
                                                else
                                                {
                                                    ListView1.Items[i].BackColor = Color.Red;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            //kleiner
                            else if (con.Contains("<"))
                            {
                                var ConValue = con.Split('<').ToList();
                                foreach (InputValue value in DoubleCheckList)
                                {
                                    switch (value)
                                    {
                                        case IntInputValue bvalue:
                                            if (bvalue.name.Contains(ConValue[0]))
                                            {
                                                if (Convert.ToInt32(ConValue[1]) > Convert.ToInt32(bvalue.value.ToString()))
                                                {
                                                    ListView1.Items[i].BackColor = Color.Green;
                                                }
                                                else
                                                {
                                                    ListView1.Items[i].BackColor = Color.Red;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            //groter
                            else if (con.Contains(">"))
                            {
                                var ConValue = con.Split('>').ToList();
                                foreach (InputValue value in DoubleCheckList)
                                {
                                    switch (value)
                                    {
                                        case IntInputValue bvalue:
                                            if (bvalue.name.Contains(ConValue[0]))
                                            {
                                                if (Convert.ToInt32(ConValue[1]) < Convert.ToInt32(bvalue.value.ToString()))
                                                {
                                                    ListView1.Items[i].BackColor = Color.Green;
                                                }
                                                else
                                                {
                                                    ListView1.Items[i].BackColor = Color.Red;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }


## v.1.16.3
### Outputstatemachine
Fucntie toegevoegd waarbij relaties op true en false gezet kunnen worden als de conditie integers en true/false waarde bevat.

Echter moet er nog een stuk code worden toegevoegd waarbij er relaties op true en false gezet kunnen worden als er meerdere integers values in de conditie zitten.

Momenteel is dit buggy.


                    //Hierbij zijn er integers, true values aanwezig in de conditie
                    else if (intConditions.Count != 0 && TrueConditions.Count != 0 && falseConditions.Count == 0)
                    {
                        if (TrueItems.Count != 0)
                        {
                            foreach (var item in TrueItems)
                            {
                                if (TrueConditions.Contains(item.name))
                                {
                                    IntegerCheck(intConditions, ListView1, ListView1.Items[i]);
                                }
                                else if (!TrueConditions.Contains(item.name))
                                {
                                    ListView1.Items[i].BackColor = Color.Red;
                                }
                            }
                        }
                        else
                        {
                            ListView1.Items[i].BackColor = Color.Red;
                        }
                    }
                    //hierbij zijn er integers en false values aanwezig in de conditie
                    else if (intConditions.Count != 0 && falseConditions.Count != 0 && TrueConditions.Count == 0)
                    {
                        if (FalseItems.Count != 0)
                        {
                            foreach (var item in FalseItems)
                            {
                                if (falseConditions.Contains("!" + item.name))
                                {
                                    IntegerCheck(intConditions, ListView1, ListView1.Items[i]);
                                }
                            }
                        }
                        else
                        {
                            ListView1.Items[i].BackColor = Color.Red;
                        }
                    }
                    //Hierbij zijn er alleen integer values in de conditie
                    else if (intConditions.Count != 0 && TrueConditions.Count == 0 && falseConditions.Count == 0)
                    {
                        IntegerCheck(intConditions, ListView1, ListView1.Items[i]);
                    }
                    //Hierbij zijn er alleen true values aanwezig in de conditie
                    else if (intConditions.Count == 0 && falseConditions.Count == 0 && TrueConditions.Count != 0)
                    {
                        foreach (var item in TrueItems)
                        {
                            if (TrueConditions.Contains(item.name))
                            {
                                ListView1.Items[i].BackColor = Color.Green;
                            }
                            else
                            {
                                ListView1.Items[i].BackColor = Color.Red;
                            }
                        }
                    }
                    //Hierbij zijn er allen false values aanwezig in de conditie
                    else if (intConditions.Count == 0 && TrueConditions.Count == 0 && falseConditions.Count != 0)
                    {
                        foreach (var item in FalseItems)
                        {
                            if (falseConditions.Contains("!" + item.name))
                            {
                                ListView1.Items[i].BackColor = Color.Green;
                            }
                        }
                    }                    
                }
            }
            catch
            {
            }
        }
        
## v1.16.4
### Outputstatemachine
Deze functie kan nu het verschil lezen tussen bools en integers.

Hij kan de volgende relaties herkennen.

* conditie met 1 of meer true values
* conditie met 1 of meer false values
* conditie met 1 of meer integers values
* conditie met 1 of meer integers values, 1 of meer true values en 1 of meer false values


                    //Hierbij zijn er integers, true values aanwezig in de conditie
                    else if (intConditions.Count != 0 && TrueConditions.Count != 0 && falseConditions.Count == 0)
                    {
                        if (TrueItems.Count != 0)
                        {
                            foreach (var item in TrueItems)
                            {
                                if (TrueConditions.Contains(item.name))
                                {
                                    IntegerCheck(intConditions, ListView1.Items[i], currentposition.relations[i]);
                                }
                                else if (!TrueConditions.Contains(item.name))
                                {
                                    currentposition.relations[i].valid = false;
                                    ListView1.Items[i].BackColor = Color.Red;
                                }
                            }
                        }
                        else
                        {
                            currentposition.relations[i].valid = false;
                            ListView1.Items[i].BackColor = Color.Red;
                        }
                    }
                    //hierbij zijn er integers en false values aanwezig in de conditie
                    else if (intConditions.Count != 0 && falseConditions.Count != 0 && TrueConditions.Count == 0)
                    {
                        if (FalseItems.Count != 0)
                        {
                            foreach (var item in FalseItems)
                            {
                                if (falseConditions.Contains("!" + item.name))
                                {
                                    IntegerCheck(intConditions, ListView1.Items[i], currentposition.relations[i]);
                                }
                                else
                                {
                                    currentposition.relations[i].valid = false;
                                    ListView1.Items[i].BackColor = Color.Red;
                                }
                            }
                        }
                        else
                        {
                            currentposition.relations[i].valid = false;
                            ListView1.Items[i].BackColor = Color.Red;
                        }
                    }
                    //Hierbij zijn er alleen integer values in de conditie
                    else if (intConditions.Count != 0 && TrueConditions.Count == 0 && falseConditions.Count == 0)
                    {
                        IntegerCheck(intConditions, ListView1.Items[i], currentposition.relations[i]);
                    }
                    //Hierbij zijn er alleen true values aanwezig in de conditie
                    else if (intConditions.Count == 0 && falseConditions.Count == 0 && TrueConditions.Count != 0)
                    {
                        if (TrueItems.Count != 0)
                        {
                            foreach (var item in TrueItems)
                            {
                                if (TrueConditions.Contains(item.name))
                                {
                                    currentposition.relations[i].valid = true;
                                    ListView1.Items[i].BackColor = Color.Green;
                                }
                                else
                                {
                                    currentposition.relations[i].valid = false;
                                    ListView1.Items[i].BackColor = Color.Red;
                                }
                            }
                        }
                        else
                        {
                            ListView1.Items[i].BackColor = Color.Red;
                        }
                    }
                    //Hierbij zijn er allen false values aanwezig in de conditie
                    else if (intConditions.Count == 0 && TrueConditions.Count == 0 && falseConditions.Count != 0)
                    {
                        if (FalseItems.Count != 0)
                        {
                            foreach (var item in FalseItems)
                            {
                                if (falseConditions.Contains("!" + item.name))
                                {
                                    currentposition.relations[i].valid = true;
                                    ListView1.Items[i].BackColor = Color.Green;
                                }
                            }
                        }
                        else
                        {
                            currentposition.relations[i].valid = false;
                            ListView1.Items[i].BackColor = Color.Red;
                        }
                    } 
                    
### Fixed Bugs
#### Stepper bug
Er is een tijdje een bug geweest waarbij de stepper niet correct functioneerde.

De stepper kon dan maar via een relatie de statemachine laten verlopen.

Ik heb de stepper functie aangepast en vernieuwd. Eerst werdt een relatie op true gezet,

Als de listview item de kleur groen had.

Nu wordt de relatie al op true gezet in de Outputstatemachine functie. 

Zo wordt er gecontroleerd op attributes ipv listview backcolors, wat een stuk stabiler is.

                for (int i = 0; i < Rxml.CurrentPosition.relations.Count; i++)
                {
                    if (Rxml.CurrentPosition.relations[i].valid)
                    {
                        Rxml.CurrentPosition = Ostm.Stepper(Rxml.CurrentPosition.relations[i], Rxml.CurrentPosition.relations[i].next.id, Rxml.CurrentPosition);
                        UpdateStatusView(listView1.Items[i].Text);
                        i = Rxml.CurrentPosition.relations.Count;
                        Ostm.UpdateDataGrid(dataGridView1, Rxml.CurrentPosition);
                    }
                }

#### Doublevalue bug
Ook was er een bug rondom dubbel voorkomende values.

Over het algemeen functioneerde deze functie perfect met een value die maar 2 keer voor kwam.

Maar als deze value meer dan 2 keer voor kwam registreerde hij deze niet meer.

Dit kwam omdat ik in de doublechecklist zogt met firstordefault ipv where.

Bij firstor default wordt er geen lijst aangemaakt maar maar één value.

Bij where gebeurt dit wel en kunnen er dus meer dan 2 dubbele values gedetecteerd worden.

                        List<InputValue> DoubleValue = DoubleCheckList.Where(x => x.name.Equals(value.name) && x.currentnodeid != value.currentnodeid).ToList();
                        if (DoubleValue.Count != 0)
                        {
                            foreach (InputValue dvalue in DoubleValue)
                            {
                                dvalue.currentnodeids.Add(value.currentnodeid);
                                value.currentnodeids.Add(value.currentnodeid);
                                DoubleCheckList.Remove(value);
                            }
                        }
                        else if (DoubleValue.Count == 0)
                        {
                            value.currentnodeids.Add(value.currentnodeid);
                        }
                        


## v.1.17.1
### Output Statemachine
Statemachine output verwijderd, werdt te lang en te ingewikkeld.

In gesprek met Stefan, een Expression evaluator gebruikt als lib.

Deze is geimplementeerd in deze versie.

UseCase11: outputsStatemachine is afgerond.

Deze UC moet alleen nog getest worden.

Deze UC wordt in de nabije toekomst ook veranderd naar "ExpressionEvaluator"

public void OutputStatemachine(ListView ListView1, Node currentposition)
        {

            try
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
                        if (evaluator.Evaluate(expression).Equals(true))
                        {
                            currentposition.relations[i].valid = true;
                            ListView1.Items[i].BackColor = Color.Green;
                        }
                        else
                        {
                            currentposition.relations[i].valid = false;
                            ListView1.Items[i].BackColor = Color.Red;
                        }
                    }
                    else
                    {
                        currentposition.relations[i].valid = true;
                        ListView1.Items[i].BackColor = Color.Green;
                    }
                }
            }
            catch 
            {

            }
        }

