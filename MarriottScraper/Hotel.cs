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
        public bool marriottMatch = false;

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
