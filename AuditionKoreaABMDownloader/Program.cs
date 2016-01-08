using System.Net.Http;
using System.Threading.Tasks;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace AuditionKoreaABMDownloader
{


    class Program
    {
        const string abmInfoUri = "http://hbs.au.xdn.kinxcdn.com/adtpatch/audition/package/AbmInfo.txt";
      static  StringBuilder songUriDownloadLink = new StringBuilder("http://hbs.au.xdn.kinxcdn.com/adtpatch/audition/package/ABM/");
        static string songList;

        static HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
        {
             AsyncContext.Run(() => MainAsync(args));
           

        }

        //Shouldn't use void in async
        static async void MainAsync(string[] args)
        {


            var response = await httpClient.GetAsync(abmInfoUri);
           
            if (response.StatusCode == HttpStatusCode.OK)
            {

                songList = await response.Content.ReadAsStringAsync();

            }
            MatchCollection mc = Regex.Matches(songList, @"k\w*.tbm");
            IEnumerable <string> files = Directory.EnumerateFiles(@"C:\Program Files (x86)\Audition\ABM").Select(Path.GetFileName);
            downloadSongs(mc,files);



        }

        static async void downloadSongs(MatchCollection songList, IEnumerable<string> files)
        {
            
            foreach (var songFileName in songList)
            {
                songUriDownloadLink.Append(songFileName);
                Console.WriteLine(songUriDownloadLink);
                var responseSong = await httpClient.GetAsync(songUriDownloadLink.ToString());
                if (responseSong.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Response succeeded");
                    var stream = await responseSong.Content.ReadAsStreamAsync();
                    using (FileStream fileStream = new FileStream(songFileName.ToString(), FileMode.Create))
                    {
                        stream.CopyTo(fileStream);

                    }
                }
                songUriDownloadLink.Remove(songUriDownloadLink.Length - songFileName.ToString().Length, songFileName.ToString().Length);
            }

            Console.ReadKey();
        }


     

    }//class


 }//namespace
