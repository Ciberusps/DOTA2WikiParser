using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using MetroFramework.Forms;
using System.Threading;
using EnvDTE;
using EnvDTE80;



namespace DOTA2WikiParser
{
    public partial class ParserForm : MetroForm
    {
        private static HtmlWeb _webGet;
        private static HtmlAgilityPack.HtmlDocument _doc;
        private static string _wikiSiteURL = "http://dota2.gamepedia.com";
        private static string _treasuresPageURL = "http://dota2.gamepedia.com/Treasure";
        private static List<Treasure> _treasures;
        private static List<Set> _sets;
        private static List<Item> _items;
        private static int _numOfTreasures = 126;
        private static ManualResetEvent[] manualEvents;
        private static int _gabenOffset = 0;
        private string _lastTreasureURL;
        private string _curTreasureURL;
        private int _tresuresTempNum = /*0*/125;
        private int _requestsNum = 20;
        private int _nextRequest = 0;
        //        private string _lastSetOrItemURL;


        public delegate void BarDelegate();

        public ParserForm()
        {
            InitializeComponent();
        }

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

        private void Form1_Load(object sender, EventArgs e)
        {
            ThreadPool.SetMinThreads(_requestsNum + 10, _requestsNum + 10);

            /*GetAllTreasuresPagesURLs();
            FullFIllTreasuresGrid();*/
        }

        private void FullFIllTreasuresGrid()
        {
            treasuresUrlGrid.Rows.Clear();
            treasuresUrlGrid.Rows.Add(_treasures.Count - 1);

            for (int i = 0; i < _treasures.Count; i++)
            {
                treasuresUrlGrid.Rows[i].Height = 100;

                treasuresUrlGrid.Rows[i].Cells[0].Value = _treasures[i].id;
                treasuresUrlGrid.Rows[i].Cells[1].Value = _treasures[i].name;
                treasuresUrlGrid.Rows[i].Cells[2].Value = _treasures[i].rare;
                treasuresUrlGrid.Rows[i].Cells[3].Value = _treasures[i].cost;
                treasuresUrlGrid.Rows[i].Cells[4].Value = _treasures[i].imgDiv2;
                treasuresUrlGrid.Rows[i].Cells[5].Value = _treasures[i].url;

                if (_treasures[i].regularSetOrItem != null)
                {
                    string s = _treasures[i].regularSetOrItem.Count + Environment.NewLine;
                    foreach (string s1 in _treasures[i].regularSetOrItem)
                    {
                        s += "  " + s1 + Environment.NewLine;
                    }
                    //                    Console.WriteLine(s);
                    treasuresUrlGrid.Rows[_treasures[i].id - 1].Cells[6].Value = s;
                }
                else
                    treasuresUrlGrid.Rows[_treasures[i].id - 1].Cells[6].Value = "NULL";

                if (_treasures[i].veryRareSetOrItem != null)
                {
                    string s = _treasures[i].veryRareSetOrItem.Count + Environment.NewLine;
                    foreach (string s1 in _treasures[i].veryRareSetOrItem)
                    {
                        s += "  " + s1 + Environment.NewLine;
                    }
                    //                    Console.WriteLine(s);
                    treasuresUrlGrid.Rows[_treasures[i].id - 1].Cells[7].Value = s;
                }
                else
                    treasuresUrlGrid.Rows[_treasures[i].id - 1].Cells[7].Value = "NULL";

                if (_treasures[i].extremelyRareSetOrItem != null)
                {
                    string s = _treasures[i].extremelyRareSetOrItem.Count + Environment.NewLine;
                    foreach (string s1 in _treasures[i].extremelyRareSetOrItem)
                    {
                        s += "  " + s1 + Environment.NewLine;
                    }
                    //                    Console.WriteLine(s);
                    treasuresUrlGrid.Rows[_treasures[i].id - 1].Cells[8].Value = s;
                }
                else
                    treasuresUrlGrid.Rows[_treasures[i].id - 1].Cells[8].Value = "NULL";

                if (_treasures[i].ultraRareSetOrItem != null)
                {
                    string s = _treasures[i].ultraRareSetOrItem.Count + Environment.NewLine;
                    foreach (string s1 in _treasures[i].ultraRareSetOrItem)
                    {
                        s += "  " + s1 + Environment.NewLine;
                    }
                    //                    Console.WriteLine(s);
                    treasuresUrlGrid.Rows[_treasures[i].id - 1].Cells[9].Value = s;
                }
                else
                    treasuresUrlGrid.Rows[_treasures[i].id - 1].Cells[9].Value = "NULL";
            }
        }

        private void GetAllTreasuresPagesURLs()
        {
            _webGet = new HtmlWeb();
            _doc = _webGet.Load(_treasuresPageURL);

            HtmlNode _treasuresTable = _doc.DocumentNode.SelectSingleNode("//table[@class='navbox']");

            _lastTreasureURL = "";
            _treasures = new List<Treasure>(capacity: 150);

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

            for (int j = 0; j < _treasures.Count; j++)
            {
                _treasures[j].id = j + 1;
            }
        }

        void ParseTreasure(object treasureInfo)
        {
            Treasure treasure = treasureInfo as Treasure;
            _webGet = new HtmlWeb();
            _doc = _webGet.Load(_wikiSiteURL + treasure.url);

            /*     Name    */
            HtmlNode idAndName = _doc.DocumentNode.SelectSingleNode("//*[@id='firstHeading']/span");
            string name = idAndName.InnerText.After(" - ");
            treasure.name = name;

            /*     Rare    */
            HtmlNode rare =
                _doc.DocumentNode.SelectSingleNode("//tr[3]/td/div/a/span/b");
            treasure.rare = rare.InnerText;

            /*    Img url  */
            HtmlNode imgUrl = _doc.DocumentNode.SelectSingleNode("//tr[2]/td/a/img");
            treasure.imgUrl = imgUrl.Attributes["src"].Value;

            /* Load image  */
            var request = WebRequest.Create(treasure.imgUrl);
            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                treasure.img = Image.FromStream(stream);
            }

            /* Img div 2   */
            treasure.imgDiv2 = ResizeImage(treasure.img, 128, 85);

            /* Regular */
            //            HtmlNodeCollection allContent = _doc.DocumentNode.SelectNodes("//h3//span[@id]");
            //            HtmlNodeCollection allContent = new HtmlNodeCollection(_doc.DocumentNode.SelectSingleNode("//h2//span[@id]"));
            //            allContent.Add(_doc.DocumentNode.SelectSingleNode("//h2//span[@id]"));


            /*if (_doc.DocumentNode.SelectNodes("//h3//span[@id]") != null)
                foreach (HtmlNode selectNode in _doc.DocumentNode.SelectNodes("//h3//span[@id]"))
                {
                    allContent.Add(selectNode);
                }*/

            //            Console.WriteLine(treasure.id);
            /*
                    РАБОЧИЙ ВАРИАНТ
            HtmlNodeCollection allContent = _doc.DocumentNode.SelectNodes("//h3//span[@id]");

            if (allContent == null)
                allContent = new HtmlNodeCollection(_doc.DocumentNode.SelectSingleNode("//h2//span[@id]"));

            allContent.Add(_doc.DocumentNode.SelectSingleNode("//table[@class='navbox']"));

            HtmlNodeCollection allLinks = _doc.DocumentNode.SelectNodes("//div//a");
*/

            HtmlNodeCollection h2 = _doc.DocumentNode.SelectNodes("//h2//span[@id]");
            HtmlNodeCollection h3 = _doc.DocumentNode.SelectNodes("//h3//span[@id]");
            HtmlNodeCollection h4 = _doc.DocumentNode.SelectNodes("//h4//span[@id]");

            HtmlNodeCollection allContent = new HtmlNodeCollection(h2[0]);

            if (h2 != null)
                for (int i = 0; i < h2.Count; i++)
                {
                    allContent.Add(h2[i]);
                }

            if (h3 != null)
                for (int i = 0; i < h3.Count; i++)
                {
                    allContent.Add(h3[i]);
                }

            if (h4 != null)
                for (int i = 0; i < h4.Count; i++)
                {
                    allContent.Add(h4[i]);
                }

            /*Sorting allContent*/
            for (int i = 0; i < allContent.Count; i++)
            {
                for (int j = i + 1; j < allContent.Count; j++)
                {
                    if (allContent[j].Line < allContent[i].Line)
                    {
                        var temp = allContent[i];
                        allContent[i] = allContent[j];
                        allContent[j] = temp;
                    }
                }
            }

            foreach (HtmlNode htmlNode in allContent)
            {
                Console.WriteLine(htmlNode.Line + " " + htmlNode.InnerText);
            }

            allContent.Add(_doc.DocumentNode.SelectSingleNode("//table[@class='navbox']"));

            HtmlNodeCollection allLinks = _doc.DocumentNode.SelectNodes("//div//a");

            string lastSetOrItemURL = "";
            Console.WriteLine("Загаловков (h3, h2): " + allContent.Count);

            for (int i = 0; i < allContent.Count; i++)
            {
                if (i != allContent.Count - 1)
                    Console.WriteLine(allContent[i].InnerText + " : " + allContent[i].Line);
                else
                    Console.WriteLine(allContent[i].Line);

                if (allContent[i].InnerText == "Regular"
                 || allContent[i].InnerText == "Equipment"
                 || allContent[i].InnerText == "Sets"
                 || allContent[i].InnerText == "Bronze Tier"
                 || allContent[i].InnerText == "Silver Tier"
                 || allContent[i].InnerText == "Gold Tier"
                 || allContent[i].InnerText == "Contents")
                {
                    if (treasure.regularSetOrItem == null)
                        treasure.regularSetOrItem = new List<string>();

                    foreach (var link in allLinks)
                    {
                        if (link.Line > allContent[i].Line
                            && link.Line < allContent[i + 1].Line
                            && link.Attributes["href"].Value != lastSetOrItemURL
                            && !link.Attributes["href"].Value.Contains("?version=")
                            && !link.Attributes["href"].Value.Contains("index.php"))
                        {
                            Console.WriteLine("\v" + link.Attributes["href"].Value + " " + link.Line);
                            treasure.regularSetOrItem.Add(link.Attributes["href"].Value);
                            //                            Console.WriteLine(treasure.regularSetOrItem[treasure.regularSetOrItem.Count - 1]);

                            lastSetOrItemURL = link.Attributes["href"].Value;
                        }
                    }
                }

                if (allContent[i].InnerText == "Very Rare"
                 || allContent[i].InnerText == "Rare"
                 || allContent[i].InnerText == "Very Rare Bonus"
                 || allContent[i].InnerText == "Rare Bonus")
                {
                    if (treasure.veryRareSetOrItem == null)
                        treasure.veryRareSetOrItem = new List<string>();

                    foreach (var link in allLinks)
                    {
                        if (link.Line > allContent[i].Line
                            && link.Line < allContent[i + 1].Line
                            && link.Attributes["href"].Value != lastSetOrItemURL
                            && !link.Attributes["href"].Value.Contains("?version=")
                            && !link.Attributes["href"].Value.Contains("index.php"))
                        {
                            Console.WriteLine("\v" + link.Attributes["href"].Value + " " + link.Line);
                            treasure.veryRareSetOrItem.Add(link.Attributes["href"].Value);
                            //                            Console.WriteLine(treasure.regularSetOrItem[treasure.regularSetOrItem.Count - 1]);

                            lastSetOrItemURL = link.Attributes["href"].Value;
                        }
                    }
                }

                if (allContent[i].InnerText == "Extremely Rare"
                 || allContent[i].InnerText == "Extremely Rare Bonus"
                 || allContent[i].InnerText == "Extremely Bonus")
                {
                    if (treasure.extremelyRareSetOrItem == null)
                        treasure.extremelyRareSetOrItem = new List<string>();

                    foreach (var link in allLinks)
                    {
                        if (link.Line > allContent[i].Line
                            && link.Line < allContent[i + 1].Line
                            && link.Attributes["href"].Value != lastSetOrItemURL
                            && !link.Attributes["href"].Value.Contains("?version=")
                            && !link.Attributes["href"].Value.Contains("index.php"))
                        {
                            Console.WriteLine("\v" + link.Attributes["href"].Value + " " + link.Line);
                            treasure.extremelyRareSetOrItem.Add(link.Attributes["href"].Value);
                            //                            Console.WriteLine(treasure.regularSetOrItem[treasure.regularSetOrItem.Count - 1]);

                            lastSetOrItemURL = link.Attributes["href"].Value;
                        }
                    }
                }

                if (allContent[i].InnerText == "Ultra Rare"
                 || allContent[i].InnerText == "Couriers"
                 || allContent[i].InnerText == "Super Very Rare Bonus")
                {
                    if (treasure.ultraRareSetOrItem == null)
                        treasure.ultraRareSetOrItem = new List<string>();

                    foreach (var link in allLinks)
                    {
                        if (link.Line > allContent[i].Line
                            && link.Line < allContent[i + 1].Line
                            && link.Attributes["href"].Value != lastSetOrItemURL
                            && !link.Attributes["href"].Value.Contains("?version=")
                            && !link.Attributes["href"].Value.Contains("index.php"))
                        {
                            Console.WriteLine("\v" + link.Attributes["href"].Value + " " + link.Line);
                            treasure.ultraRareSetOrItem.Add(link.Attributes["href"].Value);
                            //                            Console.WriteLine(treasure.regularSetOrItem[treasure.regularSetOrItem.Count - 1]);

                            lastSetOrItemURL = link.Attributes["href"].Value;
                        }
                    }
                }

            }

            //            MessageBox.Show(allContent[0].InnerText);
            //            MessageBox.Show(allContent[1].InnerText);
            //            MessageBox.Show(allContent[2].InnerText);
            //            MessageBox.Show(allContent[3].InnerText);

            // Tell the UI we are done.
            treasuresUrlGrid.Rows[treasure.id - 1].Cells[0].Value = treasure.id;
            treasuresUrlGrid.Rows[treasure.id - 1].Cells[1].Value = treasure.name;
            treasuresUrlGrid.Rows[treasure.id - 1].Cells[2].Value = treasure.rare;
            treasuresUrlGrid.Rows[treasure.id - 1].Cells[3].Value = treasure.cost;
            treasuresUrlGrid.Rows[treasure.id - 1].Cells[4].Value = treasure.imgDiv2;
            treasuresUrlGrid.Rows[treasure.id - 1].Cells[5].Value = treasure.url;
            //            Console.WriteLine(treasure.regularSetOrItem);

            /*if (treasure.regularSetOrItem != null)
            {
                string s = "";
                foreach (string s1 in treasure.regularSetOrItem)
                {
                    Console.WriteLine(s1);

                    s += s1 + Environment.NewLine;
                }
                Console.WriteLine(s);
                treasuresUrlGrid.Rows[treasure.id - 1].Cells[6].Value = s;
            }
            else
                treasuresUrlGrid.Rows[treasure.id - 1].Cells[6].Value = "NULL";*/

            try
            {
                // Invoke the delegate on the form.
                this.Invoke(new BarDelegate(UpdateBar));
            }
            catch
            {
                // Some problem occurred but we can recover.
            }
        }

        private void ParseAllTreasures()
        {
            GetAllTreasuresPagesURLs();
            FullFIllTreasuresGrid();

            ClearOutput();
            metroProgressBar1.Maximum = _tresuresTempNum/*_treasures.Count-1*/;
            metroProgressBar1.Minimum = 0;

                        ParseTreasuresInRange(0, _requestsNum);
            /*ParseTreasure(_treasures[0]);
            ParseTreasure(_treasures[66]);
            ParseTreasure(_treasures[125]);*/
//            ParseTreasure(_treasures[45]);

            _treasures.Sort(new TreasureByID());
        }

        private void ParseTreasuresInRange(int start, int count)
        {
            _nextRequest += _requestsNum;
            for (int i = start; i < start + count; i++)
            {
                if (i > _tresuresTempNum/*_treasures.Count - 1*/)
                    return;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ParseTreasure), _treasures[i]);
            }
        }

        // Update the graphical bar.
        private void UpdateBar()
        {
            metroProgressBar1.Value++;
            if (metroProgressBar1.Value >= _nextRequest)
            {
                ParseTreasuresInRange(_nextRequest, _requestsNum);
            }

            if (metroProgressBar1.Value == metroProgressBar1.Maximum)
            {
                // We are finished and the progress bar is full.
                FullFIllTreasuresGrid();
            }
        }

        protected void ClearOutput()
        {
            DTE2 ide = (DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
            ide.ToolWindows.OutputWindow.OutputWindowPanes.Item("Debug").Clear();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ide);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            ParseAllTreasures();
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            FullFIllTreasuresGrid();
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }

    public class Entity
    {
        public string url;

        public int id;
        public string name;
        public string rare;
        public string cost;
        public string image;

        public Image img;
        public Image imgDiv2;
        public string imgUrl;
    }


    public class Treasure : Entity
    {
        /*public string url;

        public int id;
        public string name;
        public string rare;
        public string cost;
        public string image;
        */

        public List<string> regularSetOrItem;
        public List<string> veryRareSetOrItem;
        public List<string> extremelyRareSetOrItem;
        public List<string> ultraRareSetOrItem;

        public List<Hashtable> giftsRegular;
        public List<Hashtable> giftsVeryRare;
        public List<Hashtable> giftsExtremelyRare;
        public List<Hashtable> giftsUltraRare;
    }

    public class Set : Entity
    {

    }

    public class Item : Entity
    {

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
