using System;
using System.Collections;
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
using System.Threading;

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
        private static ManualResetEvent[] manualEvents;
        private static int _gabenOffset = 0;
        private string _lastTreasureURL;
        private string _curTreasureURL;

        public delegate void BarDelegate();
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

//            _gabenOffset = 0;
            GetAllTreasuresPagesURLs();

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
            treasuresUrlGrid.Rows.Clear();
            treasuresUrlGrid.Rows.Add(_treasures.Count - 1);

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

            _lastTreasureURL = "";
            _treasures = new List<Treasure>(capacity: 150);

            //            int id = _numOfTreasures;
            int i = 0;
            foreach (HtmlNode link in _treasuresTable.ChildNodes.Descendants("a"))
            {
                HtmlAttribute att = link.Attributes["href"];
                _curTreasureURL = att.Value;

                if (_lastTreasureURL != _curTreasureURL && !_curTreasureURL.Contains("Bonus") && att.Value.Contains("_-_"))
                {
                    _treasures.Add(new Treasure());
                    _treasures[i].url = att.Value;
                    _lastTreasureURL = att.Value;
                    i++;
                }
            }

            _treasures.Sort(new TreasureByURL());
        }

        /*static void ParseTreasure(object state)
        {
            Treasure treasure = state as Treasure;
            _webGet = new HtmlWeb();
            _doc = _webGet.Load(_wikiSiteURL + treasure.url);

            /*      ID      #1#
            HtmlNode idAndName = _doc.DocumentNode.SelectSingleNode("//*[@id='firstHeading']/span");
            string name = idAndName.InnerText.After(" - ");
            int id = int.Parse(new string(idAndName.InnerText.TakeWhile(char.IsDigit).ToArray()));
            if (name.Contains("Bonus"))
            {
                treasure.id = id + 1;
            }
            else
            {
                treasure.id = id;
            }
            MessageBox.Show(treasure.id.ToString());
        }*/

        /*static void ParseTreasure(Treasure treasure)
        {
            _webGet = new HtmlWeb();
            _doc = _webGet.Load(_wikiSiteURL + treasure.url);

            /*      ID      #1#
            HtmlNode idAndName = _doc.DocumentNode.SelectSingleNode("//*[@id='firstHeading']/span");
            string name = idAndName.InnerText.After(" - ");
//            int id = int.Parse(new string(idAndName.InnerText.TakeWhile(char.IsDigit).ToArray()));

            /*if (name.Contains("Bonus"))
            {
                _gabenOffset += 1;
                treasure.id = id + _gabenOffset;
            }
            else
            {
                treasure.id = id + _gabenOffset;
            }#1#
            //            MessageBox.Show(treasure.id.ToString());
        }*/

        void ParseTreasure(object treasureInfo)
        {
            Treasure treasure = treasureInfo as Treasure;
            _webGet = new HtmlWeb();
            _doc = _webGet.Load(_wikiSiteURL + treasure.url);

            // Tell the UI we are done.
            try
            {
                // Invoke the delegate on the form.
                this.Invoke(new BarDelegate(UpdateBar));
            }
            catch
            {
                // Some problem occurred but we can recover.
            }

            /*      ID      */
            HtmlNode idAndName = _doc.DocumentNode.SelectSingleNode("//*[@id='firstHeading']/span");
            string name = idAndName.InnerText.After(" - ");
            
        }

        private void ParseAllTreasures()
        {
            metroProgressBar1.Maximum = 4/*_treasures.Count-1*/;
            metroProgressBar1.Minimum = 0;

            for (int i = 0; i < 5 /*_treasures.Count*/; i++)
            {
                _treasures[i].id = i + 1;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ParseTreasure), _treasures[i]);
            }
            /*using (var finished = new CountdownEvent(1))
            {
                for (int i = 0; i < 5 /*_treasures.Count#1#; i++)
                {
                    _treasures[i].id = i + 1;
                    var capture = _treasures[i];
                    finished.AddCount();
                    ThreadPool.QueueUserWorkItem(
                        (state) =>
                        {
                            try
                            {
                                ParseTreasure(capture);
                            }
                            finally
                            {
                                finished.Signal();
                            }

                        }, null);
                }

                finished.Signal(); // Signal that queueing is complete.
                finished.Wait(); // Wait for all work items to complete.
            }*/

            /*for (int i = 0; i < _treasures.Count; i++)
            {
                //ThreadPool.QueueUserWorkItem(ParseTreasure(_treasures[i]));
                //                ThreadPool.QueueUserWorkItem(new WaitCallback(ParseTreasure), _treasures[i]);
            }*/

            /*ThreadPool.QueueUserWorkItem(new WaitCallback(ParseTreasure), _treasures[0]);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ParseTreasure), _treasures[125]);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ParseTreasure), _treasures[88]);*/

            /*ParseTreasure(_treasures[0]);
            ParseTreasure(_treasures[125]);
            ParseTreasure(_treasures[88]);*/

            _treasures.Sort(new TreasureByID());
        }

        // Update the graphical bar.
        private void UpdateBar()
        {
            metroProgressBar1.Value++;
            if (metroProgressBar1.Value == metroProgressBar1.Maximum)
            {
                // We are finished and the progress bar is full.
                FullFIllTreasuresGrid();
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            ParseAllTreasures();
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
     }
 }*/

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

        public List<Hashtable> giftsRegular;
        public List<Hashtable> giftsVeryRare;
        public List<Hashtable> giftsExtremelyRare;
        public List<Hashtable> giftsUltraRare;
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
        }
    }

    public class TreasureByURL : Comparer<Treasure>
    {
        // Compares by Length, Height, and Width.
        public override int Compare(Treasure x, Treasure y)
        {
            if (x.url.CompareTo(y.url) != 0)
            {
                return int.Parse(x.url.Between("/", "_")).CompareTo((int.Parse(y.url.Between("/", "_"))));
            }
            else
            {
                return 0;
            }
        }
    }

    static class SubstringExtensions
    {
        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            //            int posB = value.LastIndexOf(b);
            int posB = value.IndexOf(b);

            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }
    }
}
