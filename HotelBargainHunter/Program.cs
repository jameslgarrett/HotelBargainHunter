using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Xml;
using System.Web;
using HtmlAgilityPack;
using HotelBargainHunter.DataSources;

namespace HotelBargainHunter
{

    class Program
    {
        static void Main(string[] args)
        {
            DateTime fromDate = DateTime.Today.AddDays(1); //replace with your check in date
            DateTime toDate = fromDate.AddDays(7); // replace with your check out date
            String city = "Orlando"; // replace with your city
            String state = "FL"; // replace with your state
            String marriottMemberNo = ""; // Marriott membership # can go here.

            HotelsCom hotelsComList = new HotelsCom(fromDate, toDate, city, state);
            Marriott marriottList = new Marriott(fromDate, toDate, city, state, marriottMemberNo);
            
            Console.WriteLine("Done...");
        }

        
    }

}
