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

//#define DEBUG_SAVING
//#define DEBUG_LOADING

namespace DOTA2WikiParser
{
    public partial class ParserForm : MetroForm
    {
        private static HtmlWeb _webGet;

        //Info 
        private static HtmlAgilityPack.HtmlDocument _doc;
        private static string _wikiSiteURL = "http://dota2.gamepedia.com";

        //Requests
        private int _requestsNum = 20;
        private int _nextRequest = 0;

        //Treasures
        private static string _treasuresPageURL = "http://dota2.gamepedia.com/Treasure";
        private static List<Treasure> _treasures;
        private static int _numOfTreasures = 126;
        private string _lastTreasureURL;
        private string _curTreasureURL;
        private int _tresuresTempNum = /*0*/125;
        private string _treasuresJSONFilePath = @"H:\GIT\Treasure Simulator DOTA 2\ParsedFiles\raw\treasures.json";
        private string _treasuresImagesPath = @"H:\GIT\Treasure Simulator DOTA 2\ParsedFiles\raw\treasuresImg";
        private string _treasuresImagesDiv2Path = @"H:\GIT\Treasure Simulator DOTA 2\ParsedFiles\raw\treasuresImgDiv2";
        private string _treasuresJSONString;
        private Hashtable _treasuresJSONRoot;
        private ArrayList _treasuresJSONArray;
        private int setsId = 1;
        private int itemsId = 1;

        //Sets
        private static List<Set> _sets;

        //Items
        private static List<Item> _items;


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
            treasuresGrid.Rows.Clear();
            treasuresGrid.Rows.Add(_treasures.Count - 1);

            for (int i = 0; i < _treasures.Count; i++)
            {
                treasuresGrid.Rows[i].Height = 100;

                treasuresGrid.Rows[i].Cells[0].Value = _treasures[i].id;
                treasuresGrid.Rows[i].Cells[1].Value = _treasures[i].name;
                treasuresGrid.Rows[i].Cells[2].Value = _treasures[i].rare;
                treasuresGrid.Rows[i].Cells[3].Value = _treasures[i].cost;
                treasuresGrid.Rows[i].Cells[4].Value = _treasures[i].imgDiv2;
                treasuresGrid.Rows[i].Cells[5].Value = _treasures[i].url;

                if (_treasures[i].regularSetOrItem != null)
                {
                    string s = _treasures[i].regularSetOrItem.Count + Environment.NewLine;

                    if (_treasures[i].giftsRegular != null)
                    {
                        for (int j = 0; j < _treasures[i].regularSetOrItem.Count-1; j++)
                        {
                            Hashtable regHash = _treasures[i].giftsRegular[j] as Hashtable;
                            s += "  " + _treasures[i].regularSetOrItem[j]  + " - " + regHash["type"] + ":" + regHash["id"] + Environment.NewLine;
                        }
                    }
                    else
                    {
                        foreach (string s1 in _treasures[i].regularSetOrItem)
                        {
                            s += "  " + s1 + Environment.NewLine;
                        }
                    }
                    
                    //                    Console.WriteLine(s);
                    treasuresGrid.Rows[_treasures[i].id - 1].Cells[6].Value = s;
                }
                else
                    treasuresGrid.Rows[_treasures[i].id - 1].Cells[6].Value = "NULL";

                if (_treasures[i].veryRareSetOrItem != null)
                {
                    string s = _treasures[i].veryRareSetOrItem.Count + Environment.NewLine;
                    foreach (string s1 in _treasures[i].veryRareSetOrItem)
                    {
                        s += "  " + s1 + Environment.NewLine;
                    }
                    //                    Console.WriteLine(s);
                    treasuresGrid.Rows[_treasures[i].id - 1].Cells[7].Value = s;
                }
                else
                    treasuresGrid.Rows[_treasures[i].id - 1].Cells[7].Value = "NULL";

                if (_treasures[i].extremelyRareSetOrItem != null)
                {
                    string s = _treasures[i].extremelyRareSetOrItem.Count + Environment.NewLine;
                    foreach (string s1 in _treasures[i].extremelyRareSetOrItem)
                    {
                        s += "  " + s1 + Environment.NewLine;
                    }
                    //                    Console.WriteLine(s);
                    treasuresGrid.Rows[_treasures[i].id - 1].Cells[8].Value = s;
                }
                else
                    treasuresGrid.Rows[_treasures[i].id - 1].Cells[8].Value = "NULL";

                if (_treasures[i].ultraRareSetOrItem != null)
                {
                    string s = _treasures[i].ultraRareSetOrItem.Count + Environment.NewLine;
                    foreach (string s1 in _treasures[i].ultraRareSetOrItem)
                    {
                        s += "  " + s1 + Environment.NewLine;
                    }
                    //                    Console.WriteLine(s);
                    treasuresGrid.Rows[_treasures[i].id - 1].Cells[9].Value = s;
                }
                else
                    treasuresGrid.Rows[_treasures[i].id - 1].Cells[9].Value = "NULL";
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
            var _webGet = new HtmlWeb();
            var _doc = _webGet.Load(_wikiSiteURL + treasure.url);

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

            /*  Cost */
            treasure.cost = "0";

            /* Load sprite  */
            var request = WebRequest.Create(treasure.imgUrl);
            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                treasure.img = Image.FromStream(stream);
            }

            /* Img div 2   */
            treasure.imgDiv2 = ResizeImage(treasure.img, 128, 85);

            /* Regular */
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
                        //                        treasure.regularSetOrItem = new List<string>();
                        treasure.regularSetOrItem = new ArrayList();

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
                        //                        treasure.veryRareSetOrItem = new List<string>();
                        treasure.veryRareSetOrItem = new ArrayList();

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
                        //                        treasure.extremelyRareSetOrItem = new List<string>();
                        treasure.extremelyRareSetOrItem = new ArrayList();

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
                        //                        treasure.ultraRareSetOrItem = new List<string>();
                        treasure.ultraRareSetOrItem = new ArrayList();


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

            // Tell the UI we are done.
            treasuresGrid.Rows[treasure.id - 1].Cells[0].Value = treasure.id;
            treasuresGrid.Rows[treasure.id - 1].Cells[1].Value = treasure.name;
            treasuresGrid.Rows[treasure.id - 1].Cells[2].Value = treasure.rare;
            treasuresGrid.Rows[treasure.id - 1].Cells[3].Value = treasure.cost;
            treasuresGrid.Rows[treasure.id - 1].Cells[4].Value = treasure.imgDiv2;
            treasuresGrid.Rows[treasure.id - 1].Cells[5].Value = treasure.url;

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
            //                        ParseTreasure(_treasures[45]);

            _treasures.Sort(new TreasureByID());
        }

        private void ParseAllSetsOrItemsToArrays()
        {
            for (int i = 0; i < _treasures.Count; i++)
            {
                if (_treasures[i].regularSetOrItem != null)
                {
                    _treasures[i].giftsRegular = new ArrayList();

                    for (int j = 0; j < _treasures[i].regularSetOrItem.Count; j++)
                    {
                        Hashtable regularSetOrItemInfo = new Hashtable();
                        regularSetOrItemInfo.Add("type", "null");
                        regularSetOrItemInfo.Add("id", "null");

                        _treasures[i].giftsRegular.Add(regularSetOrItemInfo);

                        SetOrItemInfo setOrItemInfo = new SetOrItemInfo();
                        setOrItemInfo.treasure = _treasures[i];
                        setOrItemInfo.url = _treasures[i].regularSetOrItem[j].ToString();
                        setOrItemInfo.type = "Regular";
                        setOrItemInfo.i = j;

                        ThreadPool.QueueUserWorkItem(new WaitCallback(FullFiilTreasureOrSetInfo), setOrItemInfo);

                        //                        FullFiilTreasureOrSetInfo(_treasures[i].regularSetOrItem[j].ToString());
                    }
                }
            }
        }

        private void FullFiilTreasureOrSetInfo(object setOrItemInfo)
        {
            SetOrItemInfo info = setOrItemInfo as SetOrItemInfo;
            var _webGet = new HtmlWeb();
            var _doc = _webGet.Load(_wikiSiteURL + info.url);

            HtmlNode type = _doc.DocumentNode.SelectSingleNode("//table[@class]//tr[4]/td");

            //            Console.WriteLine(type.InnerText);

            if (_sets == null)
                _sets = new List<Set>();
            if (_items == null)
                _items = new List<Item>();

            if (type != null)
            {
                Console.WriteLine(type.InnerText);
                if (type.InnerText.Contains("Bundle"))
                {
                    bool hasAlready = false;
                    foreach (Set s in _sets)
                    {
                        if (s.url == info.url)
                        {
                            Hashtable itemInfo = new Hashtable();
                            itemInfo.Add("type", "set");
                            itemInfo.Add("id", s.id);
                            info.treasure.giftsRegular.Insert(info.i, itemInfo);

                            hasAlready = true;
                        }
                    }

                    if (!hasAlready)
                    {
                        Set set = new Set();

                        set.url = info.url;

                        set.id = setsId;
                        setsId++;

                        set.name = _doc.DocumentNode.SelectSingleNode("//table[@class]//tr[1]/td").InnerText;

                        set.cost = "0";

                        HtmlNode imgUrl = _doc.DocumentNode.SelectSingleNode("//table//tr[2]/td/a/img");
                        set.imgUrl = imgUrl.Attributes["src"].Value;

                        /* Load sprite  */
                        var request = WebRequest.Create(set.imgUrl);
                        var response = request.GetResponse();

                        using (var stream = response.GetResponseStream())
                        {
                            set.img = Image.FromStream(stream);
                        }

                        /* Img div 2   */
                        set.imgDiv2 = ResizeImage(set.img, 64, 43);

                        Hashtable setInfo = new Hashtable();
                        setInfo.Add("type", "set");
                        setInfo.Add("id", set.id);
                        info.treasure.giftsRegular.Insert(info.i, setInfo);

                        _sets.Add(set);
                        //                Console.WriteLine("Set: " + info.treasure.id);
                    }
                }
                else
                {
                    bool hasAlready = false;
                    foreach (Item i in _items)
                    {
                        if (i.url == info.url)
                        {
                            Hashtable itemInfo = new Hashtable();
                            itemInfo.Add("type", "item");
                            itemInfo.Add("id", i.id);
                            info.treasure.giftsRegular.Insert(info.i, itemInfo);

                            hasAlready = true;
                        }
                    }

                    if (!hasAlready)
                    {
                        Item item = new Item();

                        item.url = info.url;

                        item.id = itemsId;
                        itemsId++;

                        item.name = _doc.DocumentNode.SelectSingleNode("//table[@class]//tr[1]/td").InnerText;

                        item.cost = "0";

                        HtmlNode imgUrl = _doc.DocumentNode.SelectSingleNode("//table//tr[2]/td/a/img");
                        item.imgUrl = imgUrl.Attributes["src"].Value;

                        /* Load sprite  */
                        var request = WebRequest.Create(item.imgUrl);
                        var response = request.GetResponse();

                        using (var stream = response.GetResponseStream())
                        {
                            item.img = Image.FromStream(stream);
                        }

                        /* Img div 2   */
                        item.imgDiv2 = ResizeImage(item.img, 64, 43);

                        item.slot = type.InnerText.After(": ");

                        Hashtable itemInfo = new Hashtable();
                        itemInfo.Add("type", "item");
                        itemInfo.Add("id", item.id);
                        info.treasure.giftsRegular.Insert(info.i, itemInfo);

                        _items.Add(item);
                        //                Console.WriteLine("Item: " + info.treasure.id);
                    }

                }
            }
            else
            {
                Console.WriteLine(info.treasure.id);
            }
        }

        private void ParseItem(Treasure treasure, Item item)
        {
            
        }

        private void FullFillItemsGrid()
        {
            itemsGrid.Rows.Clear();
            itemsGrid.Rows.Add(_items.Count);

            for (int i = 0; i < _items.Count; i++)
            {
//                _items[i].id = i + 1;
                itemsGrid.Rows[i].Height = 45;
                itemsGrid.Rows[i].Cells[0].Value = _items[i].id;
                itemsGrid.Rows[i].Cells[1].Value = _items[i].name;
                itemsGrid.Rows[i].Cells[2].Value = _items[i].rare;
                itemsGrid.Rows[i].Cells[3].Value = _items[i].cost;
                itemsGrid.Rows[i].Cells[4].Value = _items[i].imgDiv2;
                itemsGrid.Rows[i].Cells[5].Value = _items[i].url;
            }
        }

        private void FullFillSetsGrid()
        {
            setsGrid.Rows.Clear();
            setsGrid.Rows.Add(_sets.Count );

            for (int i = 0; i < _sets.Count; i++)
            {
                setsGrid.Rows[i].Cells[0].Value = _sets[i].id;
                setsGrid.Rows[i].Cells[1].Value = _sets[i].name;
                setsGrid.Rows[i].Cells[2].Value = _sets[i].rare;
                setsGrid.Rows[i].Cells[3].Value = _sets[i].cost;
                setsGrid.Rows[i].Cells[4].Value = _sets[i].imgDiv2;
                setsGrid.Rows[i].Cells[5].Value = _sets[i].url;
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

        private void metroButton7_Click(object sender, EventArgs e)
        {
            SaveTreasuresParsingInfoToFile();
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            LoadTreasuresParsingInfoFromFile();
            FullFIllTreasuresGrid();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            ParseAllSetsOrItemsToArrays();
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

        public void SaveTreasuresParsingInfoToFile()
        {
            _treasuresJSONString = System.IO.File.ReadAllText(@_treasuresJSONFilePath);
            _treasuresJSONRoot = MiniJSON.JsonDecode(_treasuresJSONString) as Hashtable;
            _treasuresJSONArray = _treasuresJSONRoot["treasures"] as ArrayList;

#if DEBUG_SAVING
            Console.WriteLine("Parsed: " + _treasuresJSONString);
#endif
            /* Fullfill info */
            _treasuresJSONArray.Clear();

            if (_treasuresJSONArray == null)
                _treasuresJSONArray = new ArrayList();

            foreach (Treasure treasure in _treasures)
            {
                Hashtable treasureHashtable = new Hashtable();
                treasureHashtable.Add("url", treasure.url);
                treasureHashtable.Add("id", treasure.id.ToString());
                treasureHashtable.Add("name", treasure.name);
                treasureHashtable.Add("rare", treasure.rare);
                treasureHashtable.Add("cost", treasure.cost);
                treasureHashtable.Add("sprite", treasure.imgUrl);

                treasure.img.Save(@_treasuresImagesPath + @"\" + treasure.id + "_" + treasure.name + ".png");
                treasure.imgDiv2.Save(@_treasuresImagesDiv2Path + @"\" + treasure.id + "_" + treasure.name + ".png");

                if (treasure.regularSetOrItem != null)
                {
                    treasureHashtable.Add("Regular", treasure.regularSetOrItem);
                }

                if (treasure.veryRareSetOrItem != null)
                {
                    treasureHashtable.Add("VeryRare", treasure.veryRareSetOrItem);
                }

                if (treasure.extremelyRareSetOrItem != null)
                {
                    treasureHashtable.Add("ExtremelyRare", treasure.extremelyRareSetOrItem);
                }

                if (treasure.ultraRareSetOrItem != null)
                {
                    treasureHashtable.Add("UltraRare", treasure.ultraRareSetOrItem);
                }

                _treasuresJSONArray.Add(treasureHashtable);
            }

            string json = MiniJSON.JsonEncode(_treasuresJSONRoot);
            System.IO.File.WriteAllText(@_treasuresJSONFilePath, json);
#if DEBUG_SAVING
            Console.WriteLine("Saved: " + json);
#endif
        }

        public void LoadTreasuresParsingInfoFromFile()
        {
            _treasuresJSONString = System.IO.File.ReadAllText(@_treasuresJSONFilePath);
            _treasuresJSONRoot = MiniJSON.JsonDecode(_treasuresJSONString) as Hashtable;
            _treasuresJSONArray = _treasuresJSONRoot["treasures"] as ArrayList;

#if DEBUG_LOADING
            Console.WriteLine("Parsed: " + _treasuresJSONString);
#endif 

            /* Fullfill info */
            if (_treasuresJSONArray != null)
            {
                if (_treasures != null)
                    _treasures.Clear();
                else
                    _treasures = new List<Treasure>();

                foreach (Hashtable hashtable in _treasuresJSONArray)
                {
                    Treasure treasure = new Treasure();

                    treasure.url = hashtable["url"].ToString();
                    treasure.id = int.Parse(hashtable["id"].ToString());
                    treasure.name = hashtable["name"].ToString();
                    treasure.rare = hashtable["rare"].ToString();
                    treasure.cost = hashtable["cost"].ToString();
                    treasure.imgUrl = hashtable["sprite"].ToString();

                    treasure.img = Image.FromFile(@_treasuresImagesPath + @"\" + treasure.id + "_" + treasure.name + ".png");
                    treasure.imgDiv2 = Image.FromFile(@_treasuresImagesDiv2Path + @"\" + treasure.id + "_" + treasure.name + ".png");

                    if (hashtable.ContainsKey("Regular"))
                        treasure.regularSetOrItem = hashtable["Regular"] as ArrayList;
                    if (hashtable.ContainsKey("VeryRare"))
                        treasure.veryRareSetOrItem = hashtable["VeryRare"] as ArrayList;
                    if (hashtable.ContainsKey("ExtremelyRare"))
                        treasure.extremelyRareSetOrItem = hashtable["ExtremelyRare"] as ArrayList;
                    if (hashtable.ContainsKey("UltraRare"))
                        treasure.ultraRareSetOrItem = hashtable["UltraRare"] as ArrayList;

                    _treasures.Add(treasure);
                }
            }
        }

        public class MiniJSON
        {
            public const int TOKEN_NONE = 0;
            public const int TOKEN_CURLY_OPEN = 1;
            public const int TOKEN_CURLY_CLOSE = 2;
            public const int TOKEN_SQUARED_OPEN = 3;
            public const int TOKEN_SQUARED_CLOSE = 4;
            public const int TOKEN_COLON = 5;
            public const int TOKEN_COMMA = 6;
            public const int TOKEN_STRING = 7;
            public const int TOKEN_NUMBER = 8;
            public const int TOKEN_TRUE = 9;
            public const int TOKEN_FALSE = 10;
            public const int TOKEN_NULL = 11;

            private const int BUILDER_CAPACITY = 2000;

            protected static MiniJSON instance = new MiniJSON();

            /// <summary>
            /// On decoding, this value holds the position at which the parse failed (-1 = no error).
            /// </summary>
            protected int lastErrorIndex = -1;
            protected string lastDecode = "";

            /// <summary>
            /// Parses the string json into a value
            /// </summary>
            /// <param name="json">A JSON string.</param>
            /// <returns>An ArrayList, a Hashtable, a float, a string, null, true, or false</returns>
            public static object JsonDecode(string json)
            {
                // save the string for debug information
                MiniJSON.instance.lastDecode = json;

                if (json != null)
                {
                    char[] charArray = json.ToCharArray();

                    int index = 0;
                    bool success = true;
                    object value = MiniJSON.instance.ParseValue(charArray, ref index, ref success);
                    if (success)
                    {
                        MiniJSON.instance.lastErrorIndex = -1;
                    }
                    else
                    {
                        MiniJSON.instance.lastErrorIndex = index;
                    }
                    return value;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Converts a Hashtable / ArrayList object into a JSON string
            /// </summary>
            /// <param name="json">A Hashtable / ArrayList</param>
            /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
            public static string JsonEncode(object json)
            {
                StringBuilder builder = new StringBuilder(BUILDER_CAPACITY);
                bool success = MiniJSON.instance.SerializeValue(json, builder);
                return (success ? builder.ToString() : null);
            }

            /// <summary>
            /// On decoding, this function returns the position at which the parse failed (-1 = no error).
            /// </summary>
            /// <returns></returns>
            public static bool LastDecodeSuccessful()
            {
                return (MiniJSON.instance.lastErrorIndex == -1);
            }

            /// <summary>
            /// On decoding, this function returns the position at which the parse failed (-1 = no error).
            /// </summary>
            /// <returns></returns>
            public static int GetLastErrorIndex()
            {
                return MiniJSON.instance.lastErrorIndex;
            }

            /// <summary>
            /// If a decoding error occurred, this function returns a piece of the JSON string 
            /// at which the error took place. To ease debugging.
            /// </summary>
            /// <returns></returns>
            public static string GetLastErrorSnippet()
            {
                if (MiniJSON.instance.lastErrorIndex == -1)
                {
                    return "";
                }
                else
                {
                    int startIndex = MiniJSON.instance.lastErrorIndex - 5;
                    int endIndex = MiniJSON.instance.lastErrorIndex + 15;
                    if (startIndex < 0)
                    {
                        startIndex = 0;
                    }
                    if (endIndex >= MiniJSON.instance.lastDecode.Length)
                    {
                        endIndex = MiniJSON.instance.lastDecode.Length - 1;
                    }

                    return MiniJSON.instance.lastDecode.Substring(startIndex, endIndex - startIndex + 1);
                }
            }

            protected Hashtable ParseObject(char[] json, ref int index)
            {
                Hashtable table = new Hashtable();
                int token;

                // {
                NextToken(json, ref index);

                bool done = false;
                while (!done)
                {
                    token = LookAhead(json, index);
                    if (token == MiniJSON.TOKEN_NONE)
                    {
                        return null;
                    }
                    else if (token == MiniJSON.TOKEN_COMMA)
                    {
                        NextToken(json, ref index);
                    }
                    else if (token == MiniJSON.TOKEN_CURLY_CLOSE)
                    {
                        NextToken(json, ref index);
                        return table;
                    }
                    else
                    {

                        // name
                        string name = ParseString(json, ref index);
                        if (name == null)
                        {
                            return null;
                        }

                        // :
                        token = NextToken(json, ref index);
                        if (token != MiniJSON.TOKEN_COLON)
                        {
                            return null;
                        }

                        // value
                        bool success = true;
                        object value = ParseValue(json, ref index, ref success);
                        if (!success)
                        {
                            return null;
                        }

                        table[name] = value;
                    }
                }

                return table;
            }

            protected ArrayList ParseArray(char[] json, ref int index)
            {
                ArrayList array = new ArrayList();

                // [
                NextToken(json, ref index);

                bool done = false;
                while (!done)
                {
                    int token = LookAhead(json, index);
                    if (token == MiniJSON.TOKEN_NONE)
                    {
                        return null;
                    }
                    else if (token == MiniJSON.TOKEN_COMMA)
                    {
                        NextToken(json, ref index);
                    }
                    else if (token == MiniJSON.TOKEN_SQUARED_CLOSE)
                    {
                        NextToken(json, ref index);
                        break;
                    }
                    else
                    {
                        bool success = true;
                        object value = ParseValue(json, ref index, ref success);
                        if (!success)
                        {
                            return null;
                        }

                        array.Add(value);
                    }
                }

                return array;
            }

            protected object ParseValue(char[] json, ref int index, ref bool success)
            {
                switch (LookAhead(json, index))
                {
                    case MiniJSON.TOKEN_STRING:
                        return ParseString(json, ref index);
                    case MiniJSON.TOKEN_NUMBER:
                        return ParseNumber(json, ref index);
                    case MiniJSON.TOKEN_CURLY_OPEN:
                        return ParseObject(json, ref index);
                    case MiniJSON.TOKEN_SQUARED_OPEN:
                        return ParseArray(json, ref index);
                    //			case MiniJSON.TOKEN_TRUE:
                    //				NextToken(json, ref index);
                    //				return Boolean.Parse("TRUE");
                    //			case MiniJSON.TOKEN_FALSE:
                    //				NextToken(json, ref index);
                    //				return Boolean.Parse("FALSE");
                    case MiniJSON.TOKEN_NULL:
                        NextToken(json, ref index);
                        return null;
                    case MiniJSON.TOKEN_NONE:
                        break;
                }

                success = false;
                return null;
            }

            protected string ParseString(char[] json, ref int index)
            {
                string s = "";
                char c;

                EatWhitespace(json, ref index);

                // "
                c = json[index];
                index++;

                bool complete = false;
                while (!complete)
                {

                    if (index == json.Length)
                    {
                        break;
                    }

                    c = json[index];
                    index++;
                    if (c == '"')
                    {
                        complete = true;
                        break;
                    }
                    else if (c == '\\')
                    {

                        if (index == json.Length)
                        {
                            break;
                        }
                        c = json[index];
                        index++;
                        if (c == '"')
                        {
                            s += '"';
                        }
                        else if (c == '\\')
                        {
                            s += '\\';
                        }
                        else if (c == '/')
                        {
                            s += '/';
                        }
                        else if (c == 'b')
                        {
                            s += '\b';
                        }
                        else if (c == 'f')
                        {
                            s += '\f';
                        }
                        else if (c == 'n')
                        {
                            s += '\n';
                        }
                        else if (c == 'r')
                        {
                            s += '\r';
                        }
                        else if (c == 't')
                        {
                            s += '\t';
                        }
                        else if (c == 'u')
                        {
                            int remainingLength = json.Length - index;
                            if (remainingLength >= 4)
                            {
                                char[] unicodeCharArray = new char[4];
                                //						Array.Copy(json, index, unicodeCharArray, 0, 4);
                                for (int i = 0; i < 4; i++)
                                {
                                    unicodeCharArray[i] = json[index + i];
                                }

                                // Drop in the HTML markup for the unicode character
                                s += "&#x" + new string(unicodeCharArray) + ";";

                                /*
                                uint codePoint = UInt32.Parse(new string(unicodeCharArray), NumberStyles.HexNumber);
                                // convert the integer codepoint to a unicode char and add to string
                                s += Char.ConvertFromUtf32((int)codePoint);
                                */

                                // skip 4 chars
                                index += 4;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        s += c.ToString();
                    }

                }

                if (!complete)
                {
                    return null;
                }

                return s;
            }

            protected float ParseNumber(char[] json, ref int index)
            {
                EatWhitespace(json, ref index);

                int lastIndex = GetLastIndexOfNumber(json, index);
                int charLength = (lastIndex - index) + 1;
                char[] numberCharArray = new char[charLength];

                //		Array.Copy(json, index, numberCharArray, 0, charLength);
                for (int i = 0; i < charLength; i++)
                {
                    numberCharArray[i] = json[index + i];
                }

                index = lastIndex + 1;
                return Single.Parse(new string(numberCharArray)); // , CultureInfo.InvariantCulture);
            }

            protected int GetLastIndexOfNumber(char[] json, int index)
            {
                int lastIndex;
                for (lastIndex = index; lastIndex < json.Length; lastIndex++)
                {
                    if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
                    {
                        break;
                    }
                }
                return lastIndex - 1;
            }

            protected void EatWhitespace(char[] json, ref int index)
            {
                for (; index < json.Length; index++)
                {
                    if (" \t\n\r".IndexOf(json[index]) == -1)
                    {
                        break;
                    }
                }
            }

            protected int LookAhead(char[] json, int index)
            {
                int saveIndex = index;
                return NextToken(json, ref saveIndex);
            }

            protected int NextToken(char[] json, ref int index)
            {
                EatWhitespace(json, ref index);

                if (index == json.Length)
                {
                    return MiniJSON.TOKEN_NONE;
                }

                char c = json[index];
                index++;
                switch (c)
                {
                    case '{':
                        return MiniJSON.TOKEN_CURLY_OPEN;
                    case '}':
                        return MiniJSON.TOKEN_CURLY_CLOSE;
                    case '[':
                        return MiniJSON.TOKEN_SQUARED_OPEN;
                    case ']':
                        return MiniJSON.TOKEN_SQUARED_CLOSE;
                    case ',':
                        return MiniJSON.TOKEN_COMMA;
                    case '"':
                        return MiniJSON.TOKEN_STRING;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                        return MiniJSON.TOKEN_NUMBER;
                    case ':':
                        return MiniJSON.TOKEN_COLON;
                }
                index--;

                int remainingLength = json.Length - index;

                // false
                if (remainingLength >= 5)
                {
                    if (json[index] == 'f' &&
                        json[index + 1] == 'a' &&
                        json[index + 2] == 'l' &&
                        json[index + 3] == 's' &&
                        json[index + 4] == 'e')
                    {
                        index += 5;
                        return MiniJSON.TOKEN_FALSE;
                    }
                }

                // true
                if (remainingLength >= 4)
                {
                    if (json[index] == 't' &&
                        json[index + 1] == 'r' &&
                        json[index + 2] == 'u' &&
                        json[index + 3] == 'e')
                    {
                        index += 4;
                        return MiniJSON.TOKEN_TRUE;
                    }
                }

                // null
                if (remainingLength >= 4)
                {
                    if (json[index] == 'n' &&
                        json[index + 1] == 'u' &&
                        json[index + 2] == 'l' &&
                        json[index + 3] == 'l')
                    {
                        index += 4;
                        return MiniJSON.TOKEN_NULL;
                    }
                }

                return MiniJSON.TOKEN_NONE;
            }

            protected bool SerializeObjectOrArray(object objectOrArray, StringBuilder builder)
            {
                if (objectOrArray is Hashtable)
                {
                    return SerializeObject((Hashtable)objectOrArray, builder);
                }
                else if (objectOrArray is ArrayList)
                {
                    return SerializeArray((ArrayList)objectOrArray, builder);
                }
                else
                {
                    return false;
                }
            }

            protected bool SerializeObject(Hashtable anObject, StringBuilder builder)
            {
                builder.Append("{");

                IDictionaryEnumerator e = anObject.GetEnumerator();
                bool first = true;
                while (e.MoveNext())
                {
                    string key = e.Key.ToString();
                    object value = e.Value;

                    if (!first)
                    {
                        builder.Append(", ");
                    }

                    SerializeString(key, builder);
                    builder.Append(":");
                    if (!SerializeValue(value, builder))
                    {
                        return false;
                    }

                    first = false;
                }

                builder.Append("}");
                return true;
            }

            protected bool SerializeArray(ArrayList anArray, StringBuilder builder)
            {
                builder.Append("[");

                bool first = true;
                for (int i = 0; i < anArray.Count; i++)
                {
                    object value = anArray[i];

                    if (!first)
                    {
                        builder.Append(", ");
                    }

                    if (!SerializeValue(value, builder))
                    {
                        return false;
                    }

                    first = false;
                }

                builder.Append("]");
                return true;
            }

            protected bool SerializeValue(object value, StringBuilder builder)
            {

                // Type t = value.GetType();

                // Debug.Log("type: " + t.ToString() + " isArray: " + t.IsArray);

                if (value.GetType().IsArray)
                {
                    SerializeArray(new ArrayList((ICollection)value), builder);
                }
                else if (value is string)
                {
                    SerializeString((string)value, builder);
                }
                else if (value is Char)
                {
                    SerializeString(value.ToString(), builder);
                }
                else if (value is Hashtable)
                {
                    SerializeObject((Hashtable)value, builder);
                }
                else if (value is ArrayList)
                {
                    SerializeArray((ArrayList)value, builder);
                }
                else if ((value is Boolean) && ((Boolean)value == true))
                {
                    builder.Append("true");
                }
                else if ((value is Boolean) && ((Boolean)value == false))
                {
                    builder.Append("false");
                }
                //		else if (value.GetType().IsPrimitive) {
                else if (value is Single)
                {
                    SerializeNumber((Single)value, builder);
                }
                else if (value == null)
                {
                    builder.Append("null");
                }
                else
                {
                    return false;
                }
                return true;
            }

            protected void SerializeString(string aString, StringBuilder builder)
            {
                builder.Append("\"");

                char[] charArray = aString.ToCharArray();
                for (int i = 0; i < charArray.Length; i++)
                {
                    char c = charArray[i];
                    if (c == '"')
                    {
                        builder.Append("\\\"");
                    }
                    else if (c == '\\')
                    {
                        builder.Append("\\\\");
                    }
                    else if (c == '\b')
                    {
                        builder.Append("\\b");
                    }
                    else if (c == '\f')
                    {
                        builder.Append("\\f");
                    }
                    else if (c == '\n')
                    {
                        builder.Append("\\n");
                    }
                    else if (c == '\r')
                    {
                        builder.Append("\\r");
                    }
                    else if (c == '\t')
                    {
                        builder.Append("\\t");
                    }
                    else
                    {
                        int codepoint = (Int32)c;
                        if ((codepoint >= 32) && (codepoint <= 126))
                        {
                            builder.Append(c);
                        }
                        //                else
                        //                {
                        //					builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                        //				}
                    }
                }

                builder.Append("\"");
            }

            protected void SerializeNumber(float number, StringBuilder builder)
            {
                builder.Append(number.ToString()); // , CultureInfo.InvariantCulture));
            }

            /*
            /// <summary>
            /// Determines if a given object is numeric in any way
            /// (can be integer, float, etc). C# has no pretty way to do this.
            /// </summary>
            protected bool IsNumeric(object o)
            {
                try {
                    Single.Parse(o.ToString());
                } catch (Exception) {
                    return false;
                }
                return true;
            }
            */
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            FullFillItemsGrid();
        }

        private void metroButton9_Click(object sender, EventArgs e)
        {
            FullFillSetsGrid();
        }
    }

    public class Entity
    {
        public string url;

        public int id;
        public string name;
        public string rare;
        public string cost;
        public string sprite;

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
        public string sprite;
        */

        /*public List<string> regularSetOrItem;
        public List<string> veryRareSetOrItem;
        public List<string> extremelyRareSetOrItem;
        public List<string> ultraRareSetOrItem;*/
        public ArrayList regularSetOrItem;
        public ArrayList veryRareSetOrItem;
        public ArrayList extremelyRareSetOrItem;
        public ArrayList ultraRareSetOrItem;

        /*public List<Hashtable> giftsRegular;
        public List<Hashtable> giftsVeryRare;
        public List<Hashtable> giftsExtremelyRare;
        public List<Hashtable> giftsUltraRare;*/
        public ArrayList giftsRegular;
        public ArrayList giftsVeryRare;
        public ArrayList giftsExtremelyRare;
        public ArrayList giftsUltraRare;
    }

    public class Set : Entity
    {

    }

    public class Item : Entity
    {
        public string slot;
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

    public class SetOrItemInfo
    {
        public Treasure treasure;
        public string url;
        public string type;
        public int i;
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

    /* Based on the JSON parser from 
     * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
     * 
     * I simplified it so that it doesn't throw exceptions
     * and can be used in Unity iPhone with maximum code stripping.
     */
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    /// 
    /// JSON uses Arrays and Objects. These correspond here to the datatypes ArrayList and Hashtable.
    /// All numbers are parsed to floats.
    /// </summary>
}
