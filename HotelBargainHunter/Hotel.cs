using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBargainHunter
{
    class Hotel
    {
        public String name;
        public String address;
        public String distance;
        public String price;
        public String hotelsComPrice;
        public String streetAddress;
        public String city;
        public String zipCode;
        public String country;

        // Match info
        public bool marriottMatch = false;
        public bool hotelsComMatch = false;

        //Hotels.com specific variables
        public String hotelsComUserRating;
        public String hotelsComStarRating;
        public String hotelsComGuestRating;

        public Hotel()
        {

        }

        public Hotel(String name, String address, String distance, String price)
        {
            this.name = name;
            this.address = address;
            this.distance = distance;
            this.price = price;
        }
    }
}
