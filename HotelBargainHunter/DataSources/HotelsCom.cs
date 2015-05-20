using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace HotelBargainHunter.DataSources
{

    class HotelsCom
    {
        private String postUrl = "http://www.marriott.com/search/submitSearch.mi?";
        private DateTime fromDate;
        private DateTime toDate;
        private String city;
        private String state;
        private String searchRadius = "50"; // number of miles to search around city
        private String recordsPerPage = "200"; // number of records to return, leave this 200 unless you know more will work
        private String marriottMemberNumber;
        private List<Hotel> hotels = new List<Hotel>();

        public HotelsCom(DateTime fromDate, DateTime toDate, String city, String state)
        {
            this.fromDate = fromDate;
            this.toDate = toDate;
            this.city = city;
            this.state = state;
            this.marriottMemberNumber = marriottMemberNumber;
            GetHotelsList();
        }
        
        public List<Hotel> GetHotels()
        {
            return this.hotels;
        }

        /// <summary>
        /// This gets the hotels list from hotels.com json API.
        /// Known Bugs: Only gets 1 page of data.
        /// </summary>
        private void GetHotelsList()
        {
            String hotelsComUrl = "http://www.hotels.com/search/listings.json?";

            Dictionary<String, String> hotelsComReqParm = new Dictionary<string, string>();
            hotelsComReqParm.Add("callback", "jQuery1102007159304269589484_1432086390798");
            hotelsComReqParm.Add("q-destination", String.Format("{0}, {1}", this.city, this.state));
            hotelsComReqParm.Add("q-localised-check-in", fromDate.ToString());
            hotelsComReqParm.Add("q-localised-check-out", toDate.ToString());
            hotelsComReqParm.Add("q-rooms", "1");
            hotelsComReqParm.Add("q-room-0-adults", "2");
            hotelsComReqParm.Add("q-room-0-children", "0");
            hotelsComReqParm.Add("sort-order", "BEST_SELLER");

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

            

            WebClient client = new WebClient();
            Stream stream = client.OpenRead(hotelsComUrl);
            StreamReader fReader = new StreamReader(stream);

            String json = fReader.ReadToEnd().Replace("jQuery1102007159304269589484_1432086390798(", "").Replace(");", "");

            XmlNode node = JsonConvert.DeserializeXmlNode(json, "Root");

            foreach (XmlNode n in node.SelectNodes("//results"))
            {
                String name = n.SelectSingleNode(".//name").InnerText.ToString();
                String streetAddress = n.SelectSingleNode(".//address/streetAddress").InnerText.ToString();
                String locality = n.SelectSingleNode(".//address/locality").InnerText.ToString();
                String postalCode = n.SelectSingleNode(".//address/postalCode").InnerText.ToString();
                String region = n.SelectSingleNode(".//address/region").InnerText.ToString();
                String country = n.SelectSingleNode(".//address/countryName").InnerText.ToString();
                String starRating = n.SelectSingleNode(".//starRating").InnerText.ToString();
                String guestRating = "";
                try
                {
                    n.SelectSingleNode(".//guestReviews/rating").InnerText.ToString();
                }
                catch
                {
                    // do nothing, may throw error if no guest rating have been recorded.
                }
                try
                {
                    String totalguestReviews = n.SelectSingleNode(".//guestReviews/total").InnerText.ToString();
                }
                catch
                {
                    // do nothing, may throw error if no guest reviews have been recorded.
                }
                
                String price = n.SelectSingleNode(".//price/current").InnerText.ToString();

                Hotel hotel = new Hotel();
                hotel.name = name;
                hotel.hotelsComPrice = price;
                hotel.city = city;
                hotel.streetAddress = streetAddress;
                hotel.hotelsComStarRating = starRating;
                hotel.hotelsComUserRating = guestRating;
                hotel.hotelsComMatch = true;

                hotels.Add(hotel);
            }

            // dispose to save memory
            fReader.Dispose();

        }
    }
}
