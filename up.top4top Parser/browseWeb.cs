using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace up.top4top_Parser
{
    class browseWeb
    {
        public delegate void EventHandler();
        public static event EventHandler _callback;

        // Making them public, cuz maybe we will need them in Main class. And static to make them easy to use.
        public static List<string> webElement = new List<string>(); 
        public static List<string> webTitle   = new List<string>();

        private static bool searchEnded = false;

        private int _exactList = 0;
        private int _listIndex = 0;
        private int _count = 0;

        private readonly string regexPattern = @"https:\/\/up.top4top(.*?)html";
        private readonly string[] countryDomains =
        {
            "com", "ac", "fr", "tn"
        };

        struct webCollectInfo
        {
            public static RichTextBox consoleBox;
            public static EventHandler processEnd;
            public static ListView listHold;
            public static ProgressBar prgVal;

            public static string protocol;
            public static string domaine;
            public static string keyword;
            public static string page;
        }


        public browseWeb(EventHandler endExecution, RichTextBox _csl ,ListView _lw, string _pt, string _key, string _pg, ProgressBar _prg, string _dm = "Random")
        {
            webCollectInfo.processEnd = endExecution;
            webCollectInfo.consoleBox = _csl;
            webCollectInfo.listHold   = _lw;

            webCollectInfo.protocol   = _pt;
            webCollectInfo.domaine    = _dm;
            webCollectInfo.keyword    = _key;
            webCollectInfo.page       = _pg;
            webCollectInfo.prgVal     = _prg;
        }

        public void NavigateSite()
        {

            if (!searchEnded)
            {

                webCollectInfo.listHold.Enabled = false;

                string search = Uri.EscapeDataString(webCollectInfo.keyword + " site:up.top4top.net");
                string requestURI = webCollectInfo.protocol + "://www.google." +
                                    ((webCollectInfo.domaine == "Random") ?
                                    countryDomains[new Random().Next(countryDomains.Length)] : webCollectInfo.domaine) +
                                    "/search?q=" + search + "&hl=fr&start=" +
                                    _count.ToString() + "0&filter=0";

                WebClient wb = new WebClient {Proxy = null};
                
                wb.DownloadStringCompleted += Download_Stringevent;
                wb.DownloadStringAsync(new Uri(requestURI));

            }

        }

        private void Download_Stringevent(object sender, DownloadStringCompletedEventArgs e)
        {
            
            if (e.Result == null || !e.Result.Contains("https://up.top4top"))
            {
                callOnProcessFinished();
                return;
            }
            
            Regex rg = new Regex(regexPattern, RegexOptions.Multiline);
            foreach (Match matchString in rg.Matches(e.Result))
            {
                if (!matchString.Value.Contains("fileuser"))
                {
                    if (!checkDuplicate(webElement, matchString.Value))
                    {
                        consoleLogs.Add(webCollectInfo.consoleBox, "Added : {matchString.Value}", Color.LightCoral);
                        webElement.Add(matchString.Value);
                    }
                }
            }

            webCollectInfo.prgVal.Maximum = webElement.Count;

            if (++_count == int.Parse(webCollectInfo.page))
            {

                searchEnded = true;
                downloadTitles();

            }
            else
            {
                NavigateSite();
            }

        }


        private void downloadTitles()
        {
            
            if (webTitle.Count != webElement.Count)
            {

                _exactList++;

                WebClient wb = new WebClient {Proxy = null};
                wb.Encoding = Encoding.UTF8; // For Arabic letters.

                wb.DownloadStringCompleted += Download_TitleEvent;
                wb.DownloadStringAsync(new Uri(webElement[_exactList - 1]));

                webCollectInfo.prgVal.Value += 1;
            }


        }

        private void Download_TitleEvent(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null) return;

                Regex rg = new Regex(@"<title>(.*?)<\/title>", RegexOptions.Multiline);
                foreach (Match matchString in rg.Matches(e.Result))
                {
                    if (!checkDuplicate(webTitle, matchString.Value))
                    {
                        _listIndex++;

                        var Extract_Info = new
                        {
                            _title      = matchString.Value.Replace("<title>", "").Replace("</title>", ""),
                            _downloads  = extractInfo(e.Result, "عدد التحميلات", "شاركها معهم").
                                Trim(),
                            _size       = extractInfo(e.Result, "حجم الملف", "تاريخ الملف").
                                Trim()
                        };

                        ListViewItem item1 = new ListViewItem();

                        item1.Text = (browseWeb.webElement[_listIndex - 1]);

                        item1.SubItems.Add(Extract_Info._title);     // Title Output.
                        item1.SubItems.Add(Extract_Info._downloads); // Downloads Output.
                        item1.SubItems.Add(Extract_Info._size);      // Size Output.

                        item1.SubItems[0].ForeColor = Color.Blue;
                        item1.SubItems[2].ForeColor = Color.Red;
                        item1.SubItems[3].ForeColor = Color.DarkCyan;
                        
                        item1.UseItemStyleForSubItems = false;

                        webCollectInfo.listHold.Items.Add(item1);

                        consoleLogs.Add(webCollectInfo.consoleBox, "Found : {Extract_Info._title}", Color.DeepPink);
                    }
                }
                downloadTitles();
            }
            catch(ArgumentOutOfRangeException) // BUG : a weird error that I can't fix it. It's probably caused by add items or Substring. 
            {
 
                callOnProcessFinished();
            }

        }

        private void callOnProcessFinished()
        {

            _callback = new EventHandler(webCollectInfo.processEnd);
            _callback.Invoke();

            webCollectInfo.listHold.Enabled = true;

            webTitle.Clear();
            webElement.Clear();
            searchEnded = false;

            _exactList = 0;
            _listIndex = 0;
            _count = 0;

        }

        private string extractInfo(string text, string start, string end)
        {

            int iStart = text.IndexOf(start);
            iStart = (iStart == -1) ? 0 : iStart + start.Length;

            int iEnd = text.LastIndexOf(end);

            if (iEnd == -1)
                iEnd = text.Length;

            string _Finalresult = Regex.Replace(text.Substring(iStart, iEnd - iStart),
                @"<[^>]*>", String.Empty);

            return _Finalresult;

        }
     
        private bool checkDuplicate( List<string> checkStr, string val )
        {
            bool localCheck = false;
            for (int i = 0; i < checkStr.Count; i++)
            {
                localCheck = checkStr[i].Equals(val);
            }
            return localCheck;
        }
    }
}