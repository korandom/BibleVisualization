> Marie Korandová, III. ročník
>
> letní semestr, 2023/24
>
> Pokročilé programování v C#, NPGR038

# Bible Link Finder

## Popis:

Program na nalezení a zobrazení vizualizace linků mezi v Bibli podle
parametrů, které si uživatel zvolí. Definujeme dva základní typy
vyhledávání: vyhledávání linků mezi verši a vyhledávání linků s určitým
slovním tématem. Typ vyhledávání se přepíná pomocí tlačítka. Link
z nějaké skupiny veršů na jinou může představovat předpověď, zmínku nebo
na příklad spojení díky společnému tématu. Data pro tyto linky jsou
čerpána z cross reference a commentaries souborů. Data pro linky
s tématy jsou čerpány z biblických slovníků -- dictionary souborů.
Mezi parametry, podle kterých se vyhledávají linky mezi verši patří
konkrétní překlad Bible, požadovaná skupina veršů, typ linků a způsob
řazení linků. Konkrétní překlad Bible určí, jaké texty veršů se
uživateli zobrazí.

Typy linků jsou následující:

a)  linky, které odkazují z požadované skupiny veršů

b)  linky, které odkazují do požadované skupiny veršů

c)  linky, které odkazují mezi verši z požadované skupiny

d)  všechny linky, které splňují alespoň jednu z a), b) nebo c) -- tedy
    linky, které nějak obsahují požadovanou skupinu veršů.

Uživatel může také zadat dvě skupiny veršů, pak se vyhledají linky
z první skupiny do druhé. Linky je možné seřadit podle četnosti
(kolikrát byli zmíněny v souborech, může indikovat jejich relevantnost),
podle veršů zdroje linků nebo podle cílových veršů linků (Genesis 1:1
před Genesis 1:2).

Linky s tématy se vyhledávají podle slovního tématu, který se vybírá
z listu možností.
Po vyhledání se zobrazí text požadovaných veršů a list nalezených linků.
Každý link obsahuje čísla zdrojových a cílových veršů, četnost linku a
text. Uživatel může zvolit, zda chce vidět text cílových nebo zdrojových
veršů. Pokud je veršů více, může jimi listovat. Způsob seřazení veršů si
může změnit i po vyhledání požadavku.
Po úspěšném vyhledání si uživatel může zobrazit Chord Diagram nebo
Diagram dvou přímek, které se otevřou v novém okně.
Chord Diagram je možné zobrazit pouze pro vyhledávání mezi verši.
Vizualizace knih je uskutečněna úsečkami na kružnici a bod na přímce
určuje kapitolu a verš. Reference jsou reprezentovány křivkami mezi
těmito body. Vzhled diagramu lze modifikovat nastavením parametrů
v configu.
V Diagramu dvou přímek je vizualizace knih uskutečněna úsečkami na
přímce. V případě vyhledávání témat jsou linky spojením bodů na úsečce a
tématem, v ostatních případech jsou knížky rozděleny do dvou paralelních
úseček podle konfigurace a linky jsou spojením bodů z jedné úsečky do
druhé.

---
## Nastavení před spuštěním

-   před spuštěním je nutné připravit si data, ze kterých chceme čerpat
    a změnit hodnoty v konfiguračním souboru (
    UserInterface.App.config), kde je detailnější popis hodnot.

### Postup připravení dat:

1.  vytvoření složky X, kde si budeme ukládat soubory. Složka musí mít
    čtyři podsložky s následujícími názvy: BibleTranslations,
    CrossReferences, Preprocessed, ToPreprocess

2.  stažení překladů Bible do BibleTranslations, soubory typu
    xxx.SQLite3 (např. BKR.SQLite3)

3.  stažení crossreference souborů do CrossReferences, typu
    xxx.crossreferences.SQLite3

4.  stažení commentaries souborů do ToPreprocess, typu
    xxx.commentaries.SQLite3

5.  stažení documentaries sobourů do ToPreprocess, typu
    xxx.dictionary.SQLite3

6.  povolení preprocesingu v konfiguračním souboru - PreProcessingNeeded
    \-\-- true

7.  preprocesing všech souborů -- nechat value DataToPreprocessList
    prázdnou

8.  nastavení cesty k složce X -- DataSourcePath

-   před dalším spuštěním, vypnout preprocessing

-   Update Dat: vymazat starý soubor z Preprocessed, nastavit
    PreProcessingNeeded \-\-- true a zadat název souboru do
    DataToPreprocessList, pokud více, oddělit středníkem

### Nastavení obecné konfigurace:

1.  FirstPickBible - nastavení preferovaného překladu Bible jako název
    souboru

2.  Pokud chceme omezit zdroje dat, zadat názvy souborů, které chceme
    použít do CommentariesToUse, CrossreferenceToUse a DictionariesToUse

3.  SearchTypeIndex -- určuje defaultní typ linků

### Nastavení konfigurace pro vizualizace:

1.  BooksInDiagram, FirstGroupBooks, SecondGroupBooks - nastavení
    zobrazovaných knih a jejich pořadí

2.  Histogram -- určuje, jestli a jaký typ histogramu se generuje

3.  LinkColor, BackReferenceColor, ForwardColor -- nastavení barvy linků

4.  VisualBookLength -- určuje, jestli mají všechny knihy stejnou
    velikost, nebo jestli je velikost proporcionální k počtu veršů
    v knize

5.  BookGap, LinkThickness, BookThickness, CurvingParameter -- určují
    velikosti přímek

6.  V sekci booksection je možné změnit používané názvy knih, a jejich
    barvu
    
---
## Spuštění 

-   entry point je funkce Main v UserInterface.Program

-   může být spuštěno bez parametrů

-   nebo spuštěno s parametrem se stejným formátem jako zadávání
    požadované skupiny veršů

---

## Způsob zadávání požadavku

-   nastavení parametrů a stisk tlačítka Search nebo enter.

### Překlad Bible

  -   překlad Bible se vybírá pomocí drop down menu

  -   menu obsahuje Bible, které jsou umístěny do složky BibleTranslations

###  Typ vyhledávání

  -   Přepínání pomocí tlačítka „Search Theme" vs „Search Reference"

  -   Pokud se vyhledává téma, místo skupiny veršů se vybírá téma
  z dropdown menu

 ### Požadovaná skupina veršů

  -   verše se zadávají do text boxu v následujících formátech:

    a)  \<zkratka názvu knihy\> \<kapitola\>:\<verš\> \[ -\[\<koncová
        kapitola:\]\<koncový verš\]
    
    b)  \<zkratka názvu knihy\> \<kapitola\>:\<verš 1\>, \<verš 2\> , ... ,
        \<verš n\>
    
    c)  \<zkratka názvu knihy\> \<kapitola\> \[- \<koncová kapitola\>\]
    
    d)  \<zkratka názvu knihy\>
-   kapitoly a verše jsou celá čísla, zadání věcí v hranatých závorkách
    je nepovinné
-   Pro knihy se používají následující zkratky, které jsou také dostupné
    při stisknutí tlačítka Help a jsou stejné jako v aplikaci MyBible
    pro Bibli kralickou (viz. FindLinksForRequirements.InputParser.cs):

> Gen -- Genesis, Exo -- Exodus, Lev -- Leviticus, Num -- Numeri, Deu --
> Deureronomium, Joz -- Jozue, Sou -- Soudců, Rut -- Rút, 1Sa --
> Samuelova 1, 2Sa -- Samuelova 2, 1Krá -- Královská 1, 2Krá --
> Královská 2, 1Par -- Paralipomenon 1, 2Par -- Paralipomenon 2, Ezd --
> Ezdrášova, Neh -- Nehemiášova, Est -- Ester, Job -- Job, Žalm --
> Žalmy, Přís -- Přísloví, Kaz -- Kazatel, Pís -- Píseň Šalamounova, Iz
> -- Izaiáš, Jer -- Jeremiáš, Pláč -- Pláč Jeremiášův, Ez -Ezechiel, Dan
> -- Daniel, Oze -- Ozeáš, Joel- Joel, Amos -- Ámos, Abd -- Abdiáš, Jon
> -- Jonáš, Mic -- Micheáš, Nah -- Nahum, Aba -- Abakuk, Sof --
> Sofoniáš, Agg -- Ageus, Zac -- Zachariáš, Mal -- Malachiáš, Mat --
> evangelia sv. Matouše, Mar -- evangelia sv. Marka, Luk -- evangelia
> sv. Lukáše, Jan -- evangelia sv. Jana, Skut -- Skutky apoštolské,
> epištoly sv. Pavla: Řím -- k Římanův, 1Kor -- ke Korintským 1, 2Kor --
> ke Korintským 2, Gal -- ke Galatským, Ef -- ke Efezským, Fil -- ke
> Filipským, Kol -- ke Koloským, 1Tes -- k Tesalonickým 1, 2Tes --
> k Tesalonickým 2, 1Timo -- k Timoteovi 1, 2Tim -- k Timoteovi 2, Tit
> -- k Titovi, Flm -- k Filemovoni, Žid -- k Židům, Sedm epištol
> různých: Jak -- sv. Jakuba, 1Pe -- sv. Petra 1, 2Pe -- sv. Petra 2,
> 1Jan -- sv. Jana1, 2Jan -- sv. Jana2, 3Jan -- sv. Jana 3, Jud -- sv.
> Judy, Zjev -- Zjevení sv. Jana.

###  Typ linků

a)  s jednou skupinou veršů

- pomocí stisknutí tlačítka Plus přidání druhé skupiny veršů -\> b)

- zvolení jedné z následujících možností pomocí TrackBaru:

    -   from -- linky, kterých zdroj patří do skupiny veršů

    -   to -- linky, kterých cíl patří do skupiny veršů

    -   all -- linky, kterých alespoň cíl nebo zdroj patří do skupiny veršů

    -   inside -- linky, kterých zdroj i cíl patří do skupiny veršů

b)  s dvěma skupinami veršů

- pomocí stisknutí tlačítka Mínus odebrání druhé skupiny veršů -\> a)

- linky, kterých zdroj patří do první skupiny veršů a cíl do druhé skupiny veršů

###  Způsob řazení linků

-   zvolení jedné z možností pomocí Trackbaru, lze změnit i po vyhledání

    -   Occurance -- řazení podle četnosti linku

    -   Source -- řazení podle pořadí zdrojových veršů

    -   Target -- řazení podle pořadí cílových veršů

---
## Zobrazení nalezených linků

-   při nesprávně zadání požadavku zobrazen chybový text a zčervená text
    požadavku

-   listování pomocí tlačítek More a Previous, jsou zobrazena pouze
    pokud je akce možná

-   změna mezi zobrazením zdrojových a cílových veršů pomocí tlačítek
    Source a Target

-   pokud je u linku text více veršový, možnost listovat pomocí šipek
    „\<" a „\>"

---
## Zobrazení vizualizací

-   vizualizace je možno zobrazit pomocí tlačítek "Chord Diagram" a "Two
    Lines Diagram"

-   při přiložení ukazatel myši na knihy je zobrazeno jejich jméno

-   pokud je zapnutý histogram, při přiložení ukazatele myši je
    zobrazena informace o úseku histogramu a četnost
