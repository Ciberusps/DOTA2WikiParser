using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using MetroFramework.Forms;

namespace DOTA2WikiParser
{
    public partial class ParserForm : MetroForm
    {
        private static HtmlWeb _webGet;
        private static HtmlAgilityPack.HtmlDocument _doc;
        private static string _wikiSiteURL = "http://dota2.gamepedia.com";
        private static string _treasuresPageURL = "http://dota2.gamepedia.com/Treasure";
        private static List<Treasure> _treasures;
        private static int _numOfTreasures = 126;

        /*  "id": "1",
            "name": "Lost Treasure of the Ivory Isles",
            "rare": "common",
            "cost": "$2.49",
            "sprite": "1_treasure_chest_juggernaut",
            "gifts": {
                "Regular": [
                    {
                        "id": "8",
                        "type": "set"
                    },
                    {
                        "id": "2",
                        "type": "set"
                    },
                    {
                        "id": "3",
                        "type": "set"
                    }
                ],
                "VeryRare": [
                    {
                        "id": "3",
                        "type": "set"
                    }
                ],
                "ExtremelyRare": [
                    {
                        "id": "4",
                        "type": "set"
                    }
                ],
                "UltraRare": [
                    {
                        "id": "3",
                        "type": "item"
                    }
                ]
            }*/

        public ParserForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetAllTreasuresPagesURLs();

            ParseAllTreasures();

            FullFIllTreasuresGrid();
//            ParseTreasure();
//            GetAllTreasuresInfo();

            /*//HtmlNode title = _doc.DocumentNode.SelectSingleNode("//span[@itemprop='name'");
            //HtmlNode title = _doc.DocumentNode.SelectSingleNode("//title");
            //HtmlNode title = _doc.DocumentNode.SelectSingleNode("//span[@itemprop]");
            HtmlNodeCollection title = _doc.DocumentNode.SelectNodes("//span");
            // / root / actors / actor[@id = 1]

            for (int i = 0; i < title.Count; i++)
            {
                Console.Write(title[i].InnerText);
            }

            Console.ReadKey();*/
        }

        private void FullFIllTreasuresGrid()
        {
            for (int i = 0; i < _treasures.Count; i++)
            {
                treasuresUrlGrid.Rows[i].Cells[0].Value = _treasures[i].id;
                treasuresUrlGrid.Rows[i].Cells[1].Value = _treasures[i].name;
                treasuresUrlGrid.Rows[i].Cells[2].Value = _treasures[i].rare;
                treasuresUrlGrid.Rows[i].Cells[3].Value = _treasures[i].cost;
                treasuresUrlGrid.Rows[i].Cells[4].Value = _treasures[i].img;
                treasuresUrlGrid.Rows[i].Cells[5].Value = _treasures[i].url;
            }
        }

        private void GetAllTreasuresPagesURLs()
        {
            _webGet = new HtmlWeb();
            _doc = _webGet.Load(_treasuresPageURL);

            HtmlNode _treasuresTable = _doc.DocumentNode.SelectSingleNode("//table[@class='navbox']");

            string _lastValue = "";
//            _tURLs = new List<string>();
            _treasures = new List<Treasure>(capacity:150);

            int id = _numOfTreasures;
            int i = 0;
            foreach (HtmlNode link in _treasuresTable.ChildNodes.Descendants("a"))
            {
                HtmlAttribute att = link.Attributes["href"];
                if (_lastValue != att.Value && att.Value.Contains("_-_"))
                {
                    //_tURLs.Add(att.Value);
                    _treasures.Add(new Treasure());
//                    _treasures[id].id = int.Parse(Regex.Match(att.Value, @"\d+").Value); //Временно нужно парсить с сайта
                    _treasures[i].id = id;
                    _treasures[i].url = att.Value;
                    _lastValue = att.Value;
                    id--;
                    i++;
                }
            }
            //            treasuresUrlGrid.Rows.Add(_tURLs.Count - 1);
            treasuresUrlGrid.Rows.Add(_treasures.Count - 1);

            _treasures.Sort(new TreasureByID());

            /* Ссылки на сокровищницы */
            /*for (int i = 0; i < _tURLs.Count; i++)
            {
                Console.WriteLine(_tURLs[i]);
            }*/
            /*                        */
        }

        private void ParseTreasure(Treasure treasure)
        {
            _webGet = new HtmlWeb();
            _doc = _webGet.Load(_wikiSiteURL + treasure.url);

            /*      ID      */
            HtmlNode id = _doc.DocumentNode.SelectSingleNode("//");
        }

        private void ParseAllTreasures()
        {
            /*for (int i = 0; i < _treasures.Count; i++)
            {
                ParseTreasure(_treasures[i]);
            }*/
            ParseTreasure(_treasures[0]);
            ParseTreasure(_treasures[125]);
            ParseTreasure(_treasures[88]);
        }
        /* static void GetAllTreasuresInfo()
         {
             _webGet = new HtmlWeb();
             _doc = _webGet.Load(_wikiSiteURL + _tURLs[0]);

             /*var a = doc.DocumentNode.SelectSingleNode("//a[@name=""a""]")
             Console.WriteLine(a.NextSibling.InnerText)#1#

             HtmlNode regular = _doc.DocumentNode.SelectSingleNode("//h3//span[@id]");
             //            if (regular.Attributes[""])

             /*HtmlNodeCollection regular =
                 _doc.DocumentNode.SelectNodes(
                     "//div[@style]//a[@href]");

             foreach (HtmlNode htmlNode in regular.Nodes())
             {
                 /*foreach (HtmlNode node in htmlNode.SelectNodes("//a[@href]"))
                 {

                 }#2#

                 Console.WriteLine(htmlNode.InnerText);

                 //                Console.WriteLine(htmlNode.SelectSingleNode("a[@href]").InnerText); 
                 /*foreach (HtmlNode node in htmlNode.ChildNodes.Descendants("a"))
                 {
                     Console.WriteLine(node.InnerText);
                 }#2#
             }#1#

             /*foreach (var tURL in _tURLs)
             {

             }#1#
         }

         public class Treasure
         {
             public List<Hashtable> giftsRegular;
             public List<Hashtable> giftsVeryRare;
             public List<Hashtable> giftsExtremelyRare;
             public List<Hashtable> giftsUltraRare;

             public int id;
             public string name;
             public string rare;
             public string cost;
             public string image;
         }
     }
 }*/
        private void treasuresUrlGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    public class Treasure
    {
        public string url;

        public int id;
        public string name;
        public string rare;
        public string cost;
        public string image;

        public Image img;
        public string imgUrl;
    }

    public class TreasureByID : Comparer<Treasure>
    {
        // Compares by Length, Height, and Width.
        public override int Compare(Treasure x, Treasure y)
        {
            if (x.id.CompareTo(y.id) != 0)
            {
                return x.id.CompareTo(y.id);
            }
            else
            {
                return 0;
            }
/*
            if (x.Length.CompareTo(y.Length) != 0)
            {
                return x.Length.CompareTo(y.Length);
            }
            else if (x.Height.CompareTo(y.Height) != 0)
            {
                return x.Height.CompareTo(y.Height);
            }
            else if (x.Width.CompareTo(y.Width) != 0)
            {
                return x.Width.CompareTo(y.Width);
            }
            else
            {
                return 0;
            }*/
        }

    }

}
