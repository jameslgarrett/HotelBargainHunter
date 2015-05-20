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

namespace HotelBargainHunter
{

    class Program
    {
        static void Main(string[] args)
        {
            DateTime fromDate = DateTime.Today.AddDays(1); //replace with your check in date
            DateTime toDate = fromDate.AddDays(7); // replace with your check out date
            String location = "Orlando, FL"; // replace with your city, state
            String city = "Orlando"; // replace with your city
            String state = "FL"; // replace with your state
            String marriottMemberNo = ""; // marriott membership # can go here.

            String hotelsComUrl = "http://www.hotels.com/search.do?";
            Dictionary<String, String> hotelsComReqParm = new Dictionary<string, string>();
            hotelsComReqParm.Add("q-destination", location);
            hotelsComReqParm.Add("q-localised-check-in", fromDate.ToString());
            hotelsComReqParm.Add("q-localised-check-out", toDate.ToString());
            hotelsComReqParm.Add("q-rooms", "1");
            hotelsComReqParm.Add("q-room-0-adults", "2");
            hotelsComReqParm.Add("q-room-0-children", "0");

            String postUrl = "http://www.marriott.com/search/submitSearch.mi?";

            List<Hotel> hotels = new List<Hotel>();

            int hccount = 0;
            foreach (KeyValuePair<String, String> pair in hotelsComReqParm)
            {
                //if this isn't the first parameter, add the & symbol
                if (hccount != 0)
                {
                    hotelsComUrl = hotelsComUrl + "&";
                }

                hotelsComUrl = hotelsComUrl + pair.Key + "=" + pair.Value;
                hccount++;
            }

            HttpWebRequest hcrequest = (HttpWebRequest)WebRequest.Create(hotelsComUrl);
            hcrequest.CookieContainer = new CookieContainer();

            HttpWebResponse hcresponse = (HttpWebResponse)hcrequest.GetResponse();

            //StreamReader reader = new StreamReader(response.GetResponseStream());

            HtmlDocument hcdocument = new HtmlDocument();
            using (Stream stream = hcresponse.GetResponseStream())
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    hcdocument.Load(stream);
                }
            }

            HtmlNodeCollection hotelsNode = hcdocument.DocumentNode.SelectNodes("//*[@id=\"listings\"]/ol/li/article");

            int hotelNum = 1;
            foreach (HtmlNode hotelNode in hotelsNode)
            {

                Hotel hotel = new Hotel();
                try
                {
                    hotel.hotelsComPrice = hotelNode.SelectSingleNode(".//div[@class='price']/b").InnerHtml.ToString().Replace("$", "");
                }
                catch
                {
                    hotel.hotelsComPrice = hotelNode.SelectSingleNode(".//div[@class='price']/span/ins").InnerHtml.ToString().Replace("$", "");
                }
                
                hotel.name = hotelNode.SelectSingleNode(".//h3/a").InnerHtml.ToString();
                hotel.streetAddress = hotelNode.SelectSingleNode(".//span[@class='p-street-address']").InnerHtml.ToString();
                hotel.city = hotelNode.SelectSingleNode(".//span[@class='p-locality']").InnerHtml.ToString();
                hotel.zipCode = hotelNode.SelectSingleNode(".//span[@class='p-postal-code']").InnerHtml.ToString().Replace(", ", "");

                hotels.Add(hotel);
                hotelNum++;
            }

            Dictionary<String, String> reqparm = new Dictionary<string, string>();
            reqparm.Add("searchType", "InCity");
            reqparm.Add("groupCode", "");
            reqparm.Add("searchRadius", "50");
            reqparm.Add("poiName", "");
            reqparm.Add("recordsPerPage", "200");
            reqparm.Add("vsMarriottBrands", "");
            reqparm.Add("singleSearchAutoSuggest", "");
            reqparm.Add("destinationAddress.latitude", "");
            reqparm.Add("destinationAddress.longitude", "");
            reqparm.Add("destinationAddress.city", city);
            reqparm.Add("destinationAddress.stateProvince", state);
            reqparm.Add("destinationAddress.country", "true");
            reqparm.Add("airportCode", "SEA");
            reqparm.Add("destinationAddress.destination", "");
            reqparm.Add("destinationAddress.location", "true");
            reqparm.Add("fromDate", fromDate.ToString("MM/dd/yyyy"));
            reqparm.Add("minDate", "");
            reqparm.Add("maxDate", "");
            reqparm.Add("monthNames", "January%2CFebruary%2CMarch%2CApril%2CMay%2CJune%2CJuly%2CAugust%2CSeptember%2COctober%2CNovember%2CDecember");
            reqparm.Add("weekDays", "S%2CM%2CT%2CW%2CT%2CF%2CS");
            reqparm.Add("dateFormatPattern", "MM%2Fdd%2Fyy");
            reqparm.Add("lengthOfStay", "1");
            reqparm.Add("toDate",toDate.AddDays(1).ToString("MM/dd/yyyy"));
            reqparm.Add("populateTodateFromFromDate", "true");
            reqparm.Add("defaultToDateDays", "1");
            reqparm.Add("roomCount", "1");
            reqparm.Add("guestCount", "1");
            reqparm.Add("marriottRewardsNumber", marriottMemberNo);
            reqparm.Add("useRewardsPoints", "true");
            reqparm.Add("clusterCode", "none");
            reqparm.Add("corporateCode", "");
            reqparm.Add("displayableIncentiveType_Number", "");
            reqparm.Add("marriottBrands", "all");

            int count = 0;
            foreach(KeyValuePair<String, String> pair in reqparm)
            {
                //if this isn't the first parameter, add the & symbol
                if(count != 0)
                {
                    postUrl = postUrl + "&";
                }

                postUrl = postUrl + pair.Key + "=" + pair.Value;
                count++;
            }


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.CookieContainer = new CookieContainer();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //StreamReader reader = new StreamReader(response.GetResponseStream());

            HtmlDocument document = new HtmlDocument();
            using (Stream stream = response.GetResponseStream())
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    document.Load(stream);
                }
            }

            

            foreach (HtmlNode hotelCode in document.DocumentNode.SelectNodes("//div[starts-with(@id,'property-record-')]"))
            {
                Hotel hotel = new Hotel();

                String title = hotelCode.ChildNodes[1].ChildNodes[1].Attributes[0].Value;
                String addr = hotelCode.SelectSingleNode(".//p[@class='m-hotel-address is-hidden-in-gallery-ml ']").InnerHtml.ToString();
                String distance = hotelCode.SelectSingleNode(".//p[@class='m-hotel-distance t-font-sm']/strong").InnerHtml.ToString();
                String price = hotelCode.SelectSingleNode(".//p[@class='t-price']").InnerHtml.ToString();

                Hotel matchHotel = null;
                try
                {
                    matchHotel = hotels.Find(h => addr.Contains(h.streetAddress));
                    if(matchHotel != null)
                    {
                        hotel = matchHotel;
                        hotel.marriottMatch = true;
                        
                    }
                    
                }
                catch
                {
                    //do nothing, no match
                }

                hotel.price = CleanString(price);
                hotel.name = CleanString(title);
                hotel.address = CleanString(addr);
                hotel.distance = CleanString(distance);

                if(hotels.FindAll(h=> h.name == hotel.name).Count == 0)
                {
                    hotels.Add(hotel);
                }
                

                //Console.WriteLine(title + " Price: $" + CleanString(price) + " Distance: " + CleanString(distance) + " Address: " + CleanString(addr));
            }
            List<Hotel> matchHotels = hotels.FindAll(h => h != null && h.marriottMatch == true);
            List<Hotel> priceMatchHotels = hotels.FindAll(h => h != null && h.marriottMatch == true && Convert.ToInt32(h.price) > Convert.ToInt32(h.hotelsComPrice));
            if(priceMatchHotels.Count > 0)
            {
                Console.WriteLine("The following hotels qualify for Marriott pricematch (20% off lower price)");
                foreach(Hotel hotel in priceMatchHotels)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Name: " + hotel.name);
                    Console.WriteLine("Hotels.com price: $" + hotel.hotelsComPrice + "/nt");
                    Console.WriteLine("Marriott.com price: $" + hotel.price + "/nt");
                    Console.WriteLine("Savings off lowest online rate: $" + Convert.ToString(Convert.ToInt32(hotel.hotelsComPrice)*.2));
                }
            }
            else
            {
                Console.WriteLine("Sorry, you cannot get 20% off today with a price match guarantee :(");
            }
            Console.WriteLine("Done...");
        }

        static private String CleanString(String toClean)
        {
            return toClean.Replace("\r", "").Replace("\t", "").Replace("\n", "");
        }
    }

}
