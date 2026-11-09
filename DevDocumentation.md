# Bible Link Finder program
Je implementován jako Window Forms aplikace, která používá projekt v Avalonia na zobrazování vizualizací. 

Má následující Model-View-ViewModel architekturu:
1.	Model – knihovny DataStructures, FindLinksForRequirements, Preprocessing
2.	View – WindowForms projekt UserInterface, Avalonia projekt Vizualizace
3.	ViewModel – Knihovna ViewModel

## Model
###	A. DataStructures knihovna
-	obsahuje struktury a třídy, pomocí kterých můžeme reprezentovat linky 

#### Link 
-	reprezentuje odkaz z nějaké části Bible na druhou pomocí:
    -  Reference source – zdrojová část
    - Reference target – cílová část
    - int Occurance – četnost v datových souborech
-	metody pracující s occurance – zvýšení, getter
-	metody rovnosti – Equals, GetHashCode, operátory ==,!=
-	metody, zda link splňuje podmínku pro daný list požadovaných referencí
    - **To**(List<Reference>):bool – patří target linku do listu
    - **From**(List<Reference>):bool – patří source linku do listu
    - **Inside**(List<Reference>):bool – patří source i target do listu
    - **All**(List<Reference>):bool – patří source nebo alespoň target do listu
-	Porovnávání linků pomocí třech Comparerů – podle source, occurance, target

#### Reference
-	reprezentuje rozsah části Bible pomocí následujících fieldů:
    - int book
    - int chapterStart
    - int verseStart
    - int chapterEnd 
    - int verseEnd
-	metody rovnosti – Equals, GetHashCode, a operátory ==,!=, 
-	obsahuje metody:
    - **FitsInto**(Reference requirement) : bool  – je aktuální reference v rozsahu requirement? ano/ne
    - **CompareTo**():bool – porovnání dvou referencí, v jakém pořadí se nacházejí v Bibli
-	ReferenceExtentions 
    - metoda **ToReference**(this string text): List<Reference>
    - převod textové reprezentace do struktury Reference

#### Verse
-	reprezentuje konkrétní verš pomocí následujících fieldů:
    - int book – číslo knihy, které je určeno v datových souborech
    - string bookShort – zkratka názvu knihy, vzato přímo z NumberToBook třídy
    - int chapter 
    - int verse 
    - string text – text verše z právě používaného překladu Bible
### B. FindLinksForRequirements knihovna
-	obsahuje třídy na hledání linků, které splňují požadavky
-	používá DataStructures

#### InputParser
-	třída k převedení textového zadání veršů do Referencí, které můžeme použít na vyhledání linků
-	obsahuje slovník na převod jména knihy na číslo použitelné v datových souborech
-	bezparametrický konstruktor, nebo konstruktor s předáním slovníku na jména knih
-	metoda **Parse**(string):List<Reference> 
#### LinkLoader
-	nalezení všech linků k dispozici, vyhledání s konkrétními požadavky
##### ReferenceLinkLoader 
-	konstruktor s třemi povinnými parametry – cesta na složku s crossreference daty, na složku s daty z komentářů a složku se slovníkovými daty, a s třemi nepovinnými parametry – soubory, které se mají použít pro cross., komentáře a slovníky
-	konstruktor načte všechny linky z databází, souborů jak pro linky mezi verši, tak pro témata
-	metoda **FindLinks**(List<Reference> requierements, OneTextBoxState state):List<Link> 
nalezne Linky, které odpovídají požadavku typu linků a)
-	metoda **FindLinksTwo**(List<Reference> requirementsFrom, List<Reference> requirementsTo):List<Link> nalezne linky pro typ linků b)
-	metoda **FindThemeLinks**(string theme):List<Link> nalezne linky pro téma theme
-	metoda **GetThemes**():List<string> vrátí všechny témata, která je možné vyhledat

#### BibleTextFinder
-	správné zobrazení veršů podle konkrétního překladu Bible
##### BibleText
-	konstruktor má parametr cestu k souboru s překladem Bible
-	v konstruktoru se Bible načte jako list veršů do fieldu verses
-	implementuje IEnumerable -  má funkci **GetEnumerator**(Reference reference)  : VerseEnumerator, vyčtení veršů v dané části Bible
##### VerseEnumerator
-	enumerátor, který umožňuje procházení kolekce v obou směrech, a i kontrola, zda je možný krok bez provedení kroku
-	metody Current(), MoveNext(), CheckNext(), MoveBack(), CheckBack()

### C. Preprocessing
-	vytažení dat z textu v komentářích a slovnících a vytvoření databází (crossreference souborů)
#### Preprocessor
-	konstruktor s parametry – složka se soubory ke zpracování, složka k uložení zpracovaných dat
-	používá TargetParser na zpracování jednotlivých textů
-	metoda **ProcessAll**():void – zpracování všech souborů ve složce
-	metoda **Process**(string fileName):void – zpracování specifického souboru
-	pokud už v cílové složce je soubor zpracovaný, nezpracuje se znovu, proto pokud se jedná o updatování souboru, je nutné staré zpracování předem smazat a poté nově vytvořit
#### TextTargetParsing
-	TargetParser, jediná metoda: **ParseTextForTargetsString**(string text): List<Reference>
-	text projde a extrahuje odkazy, pomocí string.ToReference() převede na Reference
## View
### UserInterface
-	WindowForms projekt
-	jeho součástí je config soubor
-	stará se o zobrazení dat, které bere z ViewModelu pomocí Data Bindingu nebo voláním metod
-	spuštění bez parametrů – spuštění aplikace a otevření Form1, kde uživatel může zadat parametry
-	spuštění s parametrem – parametr je textová reprezentace skupiny veršů, aplikace se spustí a vyhledají se linky s defaultně nastaveným překladem Bible, s tříděním podle occurance a s typem linků a) – all. 

#### Form1
-	field ModelViewRequirementBase, přes který se pracuje s daty, data binding
-	tabulka s linky je tvořena s inicializací Form1 kvůli napojení na data
-	 text veršů je při změnách prohledán a html tagy (např. <J>…<\J>) jsou v textu nahrazeny za změnu barvy, nový odstavec atd.
#### VisualizationForm
-	slouží pouze jako host pro MainView z Avalonia projektu Visualization
-	má dva konstruktory – pro vytvoření linků pro témata a mezi verši

### Visualization 
-	má interní architekturu Model-View-ModelView, která rozděluje project následovně:
1.	Model – třídy v namespacu Diagrams a Util
2.	View – MainView
3.	ModelView- MainModelView
-	Při spuštění VisualizationFormu se inicializuje MainModelView a dosadí se jako data context pro MainView

#### Modely Diagramů
-	Je definovanán interface IDiagramBase s metodami pro získání dat linků, knížek, updatování pozice diagramu a počitání geometrických tvarů pro objekty
-	IDiagramBase je implementován třídou ChordDiagramModel pro ChordDiagram a TwoLinesDiagramModel pro diagram dvou přímek.
#### Util
-	Třída **BookManager** slouží pro načtení listu vizuální reprezentace knížek a vypočítání pozic jednotlivých veršů
-	Třída **ConfigManager** má sadu statických funkcí, které vrací konfiguraci pro vizualizaci
-	**HistogramManager** slouží pro konfiguraci a načtení histogramu, spolu s vypočítáním pozic, histogram má logaritmickou škálu
-	**VisualLink** je třída která reprezentuje link, obsahuje informace o barvě a umístění linku
-	**Book** je třída která reprezentuje knihu, obsahuje informace o knížce, kapitolách, barvě a umístění knihy
-	**HistogramItem** je třída, která reprezentuje část histogramu, obsahuje informace o barvě a umístění tvaru
#### MainViewModel
-	Má dva konstruktory, jeden pro tvoření diagramu mezi verši a jeden pro tvoření diagramu s tématem
-	Obsahuje field IDiagramBase Diagram, přes který získává a updatuje knihy, linky a histogram
#### MainView
-	Váže se na data v MainViewModelu a vykresluje podle nich knihy, linky a histogram
## ViewModel
### LinkBox
-	udržuje data pro box, ve kterém jsou zobrazená data o linku např. Visible, Occurance, Popisky pro source a target, konkrétní verš, listovací tlačítka
### SearchBoxRequirement
-	udržuje stav pro box, kde uživatel nastavuje požadavek např. textová reprezentace první /druhé skupiny veršů, konkrétní překlad Bible, typ linků
### RequirementBox
-	drží informace o požadovaných verších – jejich text
### ViewModelRequirementBase
-	udržuje celkový stav dat, mění je podle akcí uživatele
-	obsahuje list LinkBoxů, SearchBoxRequirement, RequirementBox, InputParser, BibleText, RefererenceLinkLoader
-	při inicializaci provede preprocessing, pokud má nastat
-	stav Visible pro tlačítka More/Previous při listování linky
-	List<Link> _links – všechny nalezené linky, které se dosazují do linkBoxů
-	metoda **Search**():void – pomocí ReferenceLinkLoaderu nalezne linky, dosadí první skupinu do LinkBoxů, updatuje RequirementBox, při chybě vypíše hlášku
-	tlačítka Source, Target – metody ShowSources/Targets – všem LinkBoxům změní verš, který se zobrazuje 
-	Změna řazení linků – metoda **ChangeSortingWay** – index trackBaru v UserInterface navázaný na enum s možnostmi
-	**SortLinks**():void – seřadí _links s odpovídajícím comparerem
