using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HotelBargainHunter.DataSources
{
    class Marriott
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

        public Marriott(DateTime fromDate, DateTime toDate, String city, String state, String marriottMemberNumber)
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

        private void GetHotelsList()
        {
            Dictionary<String, String> reqparm = new Dictionary<string, string>();
            reqparm.Add("searchType", "InCity");
            reqparm.Add("groupCode", "");
            reqparm.Add("searchRadius", this.searchRadius);
            reqparm.Add("poiName", "");
            reqparm.Add("recordsPerPage", this.recordsPerPage);
            reqparm.Add("vsMarriottBrands", "");
            reqparm.Add("singleSearchAutoSuggest", "");
            reqparm.Add("destinationAddress.latitude", "");
            reqparm.Add("destinationAddress.longitude", "");
            reqparm.Add("destinationAddress.city", this.city);
            reqparm.Add("destinationAddress.stateProvince", this.state);
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
            reqparm.Add("toDate", toDate.AddDays(1).ToString("MM/dd/yyyy"));
            reqparm.Add("populateTodateFromFromDate", "true");
            reqparm.Add("defaultToDateDays", "1");
            reqparm.Add("roomCount", "1");
            reqparm.Add("guestCount", "1");
            reqparm.Add("marriottRewardsNumber", this.marriottMemberNumber);
            reqparm.Add("useRewardsPoints", "true");
            reqparm.Add("clusterCode", "none");
            reqparm.Add("corporateCode", "");
            reqparm.Add("displayableIncentiveType_Number", "");
            reqparm.Add("marriottBrands", "all");

            int count = 0;
            foreach (KeyValuePair<String, String> pair in reqparm)
            {
                //if this isn't the first parameter, add the & symbol
                if (count != 0)
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

                hotel.price = CleanString(price);
                hotel.name = CleanString(title);
                hotel.address = CleanString(addr);
                hotel.distance = CleanString(distance);

                if (hotels.FindAll(h => h.name == hotel.name).Count == 0)
                {
                    hotels.Add(hotel);
                }
            }
        }

        static private String CleanString(String toClean)
        {
            return toClean.Replace("\r", "").Replace("\t", "").Replace("\n", "");
        }
    }
}
