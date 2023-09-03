using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public static class NameToBook
    {
        public static Dictionary<int, string> bookNumberToName = new Dictionary<int, string> //names t
        {
           {10,"Gen"},{ 20,"Exo"},{ 30,"Lev"},{ 40,"Num"},
           { 50,"Deu"},{ 60,"Joz"},{ 70,"Sou"},{ 80,"Rut"},
           { 90,"1Sa"},{ 100,"2Sa"},{ 110,"1Krá"},{ 120,"2Krá"},
           { 130,"1Par"},{ 140,"2Par"},{ 150,"Ezd"},{ 160,"Neh"},
           { 190,"Est"},{ 220,"Job"},{ 230,"Žalm"},{ 240,"Přís"},
           { 250,"Kaz"},{ 260,"Pís"},{ 290,"Iz"},{ 300,"Jer"},
           { 310,"Pláč"},{ 330,"Ez"},{ 340,"Dan"},{ 350,"Oze"},
           { 360,"Joel"},{ 370,"Amos"},{ 380,"Abd"},{ 390,"Jon"},
           { 400,"Mic"},{ 410,"Nah"},{ 420,"Aba"},{ 430,"Sof"},
           { 442,"Agg"},{ 450,"Zac"},{ 460,"Mal"},{ 470,"Mat"},
           { 480,"Mar"},{ 490,"Luk"},{ 500,"Jan"},{ 510,"Skut"},
           { 520,"Řím"},{ 530,"1Kor"},{ 540,"2Kor"},{ 550,"Gal"},
           { 560,"Ef"},{ 570,"Fil"},{ 580,"Kol"},{ 590,"1Tes"},
           { 600,"2Tes"},{ 610,"1Timo"},{ 620,"2Tim"},{ 630,"Tit"},
           { 640,"Flm"},{ 650,"Žid"},{ 660,"Jak"},{ 670,"1Pe"},
           { 680,"2Pe"},{ 690,"1Jan"},{ 700,"2Jan"},{ 710,"3Jan"},
           { 720,"Jud"},{ 730,"Zjev"}
        };
    }
    public struct Verse
    {
        public readonly int book;
        public readonly string bookShort;
        public readonly int chapter;
        public readonly int verse;
        public readonly string text;

        public Verse(int book, int chapter, int verse, string text)
        {
            this.book = book;
            this.bookShort = NameToBook.bookNumberToName[book];
            this.chapter = chapter;
            this.verse = verse;
            this.text = text;
        }
    }
}
