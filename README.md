# 1. Statemachine Generator
### 1.1 context
Dit project is gebaseerd op een stageopdracht voor PCiD. PCiD is een klein bedrijf dat software en hardware gerelateerde opdrachten aanneemt.   
PCiD heeft veel opdrachten van verschillende bedrijven. Deze bedrijven hebben allemaal verschillende bedrijfsprocessen.   
Om deze bedrijfsprocess in kaart te brengen gebruikt men UML diagrams. Deze UML diagrams zijn vooral flowcharts ook wel statemachines.   
Een statemachine representeert een bedrijfsproces, en runned als het ware de flowchart als een programma om de eindgebruiker up-to-date te houden.
### 1.2 probleem
Het probleem is echter dat ieder bedrijf andere teken standaarden heeft voor het visualiseren van hun bedrijfsprocessen.   
Dit houdt in dat PCiD, voor ieder bedrijf handmatig een statemachine moet gaan programmeren, wat tijd inefficient is.
### 1.3 oplossing
Dus in dit project heb ik een oplossing ontwikkeld die dit probleem oplost.   
Samen met PCiD heb ik een statemachine generator ontwikkeld, die in staat is om een flowchart uit te lezen   
en deze automatisch om te zetten in een statemachine.

## 2. Getting Started

Onderstaande instructies zullen een werkende versie van Statemachine Generator x.x.x. creëeren op u lokale desktop.

### 2.1 Installatie voorwaardes

Voor het tekenen van flowcharts wordt yEd Graph Editor gebruikt.   
```
yEd Graph Editor 3.19.1.1
https://www.yworks.com/products/yed
```   
   
Voor de statemchine generator is alleen een IDE nodig, mits je wilt runnen in een environment.   

```
Visual Studio Enterprise 16.4.1
https://visualstudio.microsoft.com/downloads/
```   

Voor de output van de gegenereerde statemachines is het nodig om een log framework te downloaden.   
Hier onder is te zien waar deze frameworks te downloaden zijn voor, C#, C++ en Java.   
Het is mogelijk om andere frameworks te gebruiken, echter zal dan een gedeelte van de gegenereerde code handmatig aangepast moeten worden.   


```
C#: Nlog 
downloadable as a nuget package inside the visual studio env.
Install-Package NLog -Version 4.6.8
Install-Package Nlog.config -Version 4.6.8

C++: spdlog
downloadable as a zip through github. 
https://github.com/gabime/spdlog

Java: log4j
Downloadable as a package.
https://logging.apache.org/log4j/2.x/
```


### 2.2 Installeren

#### 2.2.1 Installeren in de env
Volg onderstaande stappen als je de statemachine generator in de visual studio omgeving wilt runnen:   

```
1. Clone of download de repository.
2. Run de statemachine.sln file.
3. Zet de sln config op "debug".
4. Klik op start.
```

#### 2.2.2 Installeren van de exe
Volg onderstaande stappen als je de statemachine generator wilt runnen als programma:   

```
1. Clone of download de repository.
2. open de release map -> /statemachine/bin/release.
3. run de Statemachine.GeneratorX.X.X.exe file.
```

## 3. Instructies

### 3.1 Hoe teken je een compatible flowchart
#### 3.1.1 Een flowchart aanmaken
```
1. Open yEd Graph Editor.
2. Maak een nieuw document aan.
3. Rechts onder het kopje "Palette" selecteer "Flowchart".
```
#### 3.1.2 Flowchart node types
De flowchart die compatible is met deze statemachine generator moet voldoen aan onderstaande voorwaardes.
```
1. De flowchart bestaat uit 3 vershillende elementen. start-, process- en een decisionnode.
2. De flowchart begint met één start node.
3. Een process node kan meerdere binnenkomende relaties hebben maar kan maar één uitgaande relatie hebben.
3. Een Decision node kan meerdere binnenkomende relaties hebben maar kan niet minder dan twee uitgaande relaties hebben.
```
#### 3.1.3 Flowchart naamgeving
De elementen van de flowchart hebben alle vier een andere manier van naamgeving.
```
Process-, Decision- en Startnodes.
1. De naam van deze drie elementen is een werkwoord.
2. De naam is volledig aan elkaar geschreven en bevat alleen letters en hoofdletters.

Uitgaande relaties vanaf decisionnodes.
1. Relaties die van een decisionnode afkomen worden genoteerd als een conditie.
2. Deze conditie is te vergelijken met een if statement.
3. Voorbeeld: Varx=0 & Vary
3.1 - Varx wordt hier vergleken met een integer. De generator leest Varx dan ook als een integer.
    - Tussen de variabele, de comparison symbol en de integer staan GEEN spaties.
    - Tussen voorwaarde 1: Varx=0, de comparison symbol en voorwaarde 2: VarY staan WEL spaties.
3.2 - Vary wordt hier met niks vergelijken. De generator leest Vary dan als een boolean. In dit geval is Vary waar.
    - Als Vary als "!Vary" genoteerd is dan is Vary onwaar.
3.3 - De conditie namen: Varx en Vary, kunnen alleen bestaan uit letters en Hoofdletter.
    - Er kan ook maar een conditie voor komen. Voorbeeld: "Varx=0" of alleen "Vary".
    - Een conditie kan verder uit oneindige voorwaardes bestaan. Voorbeeld: Varx=0 & Vary & Varq & etc.
    - Een comparison symbol in de conditie kan alleen maar AND en OR zijn. "&" of "|".
    
Uitgaande relaties vanaf process- en startnodes
1. De naam van uitgaande relaties vanaf process- en startnodes krijgen ook een werkwoord als naam.
2. Deze naam bestaat uit letters en Hoofdletters.
```

#### 3.1.4 Voorbeeld
![picture](/img/Voorbeeldstm.PNG)
### 3.2 Hoe gebruik je de generator

#### Een flowchart uploaden
Door in het eerste scherm van de generator op de browse knop te klikken kun je een XML file uploaden.   
Vervolgens kun je op Run state machine duwen om de statemachine te simuleren.   
![picture](/img/uploadenxml.PNG)
#### De statemachine simuleren
In het tweede scherm kun je de statemachine simuleren.   
In dit scherm kun je aan de linker kant de currentstates aflezen.   
![picture](/img/currentstatesaflezen.PNG)   
Aan de rechter kan van het scherm kun je de inputvalues aflezen en aanpassen.   
Doormiddel van de Changevalue knop kun je input values van true naar false en van false naar true veranderen.   
Door een integer van een input value te selecteren en deze direct in het datagrid aan te passen kun je de waarde van de integer value aanpassen.   
![picture](/img/inputvaluesaflezenaanpassen.PNG)   
Doormiddel van de Step knop en de Auto step checkbox kan je door de statemachine heenlopen.   
![picture](/img/stepperautostepper.PNG)   
Doormiddel van de export logs knop kun je de logs van de gesimuleerde statemachine exporteren en opslaan op je computer.   
Zodoende kun je deze terug zien om te controleren op fouten.   
![picture](/img/logsaflezenexporteren.PNG)

#### De statemachine exporteren
Met behulp van de Export statemachine knop kan je de statemachine code genereren in drie talen naar keuze, C#, C++ en Java.
![picture](/img/stmexporterengenereren.PNG)

### 3.3 de flowchart foutherkenning foutnummers
In scherm een komen fouten te voor schijn als de getekende flowchart niet klopt.   
Benader de instructies om een correcte flowchart te tekenen.   
Om de fout errors, zo begrijpelijk mogelijk te maken heb ik deze op een rijtje gezet en omschreven in een tabel.   
![picure](/img/foutherkenningnummers.PNG =10x)


## 4. Statemachine code integreren
Dit zijn voorbeelden van het integreren. Er zijn meer mogelijkheden om de code te integreren.

### 4.1 Csharp
1. Maak een Console appliction aan in visual studio aan.
2. Importeer de gegenereerde files doormiddel van de add existing items optie.   
![picture](/img/importcsharp.PNG)
3. Verander de projectname van de override class.   
![picture](/img/changeprojnamecsharp.PNG)
4. Voeg onderstaande regels toe in de program.cs om de statemachine te starten.   
![picture](/img/Addprogramlinescsharp.PNG)
5. Installeer onderstaande nuget packages. Deze staan ook weergegeven in de installatie voorwaardes.   
![picture](/img/nugetcsharp.PNG)   
6. Run het programma.

### 4.2 C++
1. Maak een Console application voor C++ in visual studio aan.   
2. Importeer de gegenereerde files handmatig in de repos map, daarna voeg je ze in de ide toe via de add existing items optie.   
![picture](/img/Importcplus.PNG)![picture](/img/Importcplus2.PNG)   
3. Importeer vervolgens de spdlog map voor het logging framework. Dit mapje hoef je alleen handmatig toe te voegen aan de repos map.   
   Dit framework is te downloaden in het kopje installatie voorwaardes.   
4. Voeg de onderstaande regels toe aan je main.h om de statemachine te laten starten.   
![picture](/img/Addprogramlinescplus.PNG)   
5. Voeg de benodigde includes toe en controleer of de includes valide zijn.   
![picture](/img/checkincludescplus.PNG)
6. Run het programma.

### 4.3 Java
1. Maak een nieuw java project aan in een willekeurige IDE. In dit voorbeeld wordt eclipse gebruikt.
2. Sleep de gegenereerde files in de src folder.   
![picture](/img/importjava.PNG)
3. Voeg doormiddel van een buidlpath de external jars van het log framework toe. Het framework is te installeren bij de installatie voorwaardes.   
![picture](/img/addlogjava.PNG)
4. Voeg een main toe met onderstaande code.   
![picture](/img/addprogramlinesjava.PNG)
5. Run het programma.

## 5. Ontwikkeld met

* [Visual Studio](https://visualstudio.microsoft.com/downloads/)
* [yEd Graph Editor](https://www.yworks.com/products/yed)


## 7. Autheur

* **R.F. de Groen** - [rutgerfrans](https://rutgerfrans.com/)


## 8. Erkenning
Dit project is zeer zeker niet gemaakt door alleen mijzelf.
In onderstaande lijst wil ik graag erkenning geven aan de mensen die mij doorgaands geholpen hebben tijdens dit project.

* PCiD voor het mogelijk maken van een stage plek.
* Nick Rademakers voor het fungeren als stagebegeleider en het begeleiden van het project.
* Stefan Artz voor het fungeren als product-owner en bij het ondersteunen van de ontwikkeling van het PoC.
