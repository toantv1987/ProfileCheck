using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AutoDoDuongKinh
{
 public  class ClsConfig
    {
        private string _name;
        private int _age;
        private DateTime _dateOfBirth;
        private string _SSN;
        private string _address;
        private string _email;
        private bool _frequentBuyer;
        // Name property with category attribute and   
        // description attribute added   
        [CategoryAttribute("ID Settings"), DescriptionAttribute("Name of the customer")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        [CategoryAttribute("ID Settings"),
            DescriptionAttribute("Social Security Number of the customer")
        ]
        public string SSN
        {
            get
            {
                return _SSN;
            }
            set
            {
                _SSN = value;
            }
        }
        [CategoryAttribute("ID Settings"),
            DescriptionAttribute("Address of the customer")
        ]
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }
        [CategoryAttribute("ID Settings"),
            DescriptionAttribute("Date of Birth of the Customer (optional)")
        ]
        public DateTime DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }
            set
            {
                _dateOfBirth = value;
            }
        }
        [CategoryAttribute("ID Settings"), DescriptionAttribute("Age of the customer")]
        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
            }
        }
        [CategoryAttribute("Marketting Settings"), DescriptionAttribute("If the customer as bought more than 10 times, thisisset to true")]
        public bool FrequentBuyer
        {
            get
            {
                return _frequentBuyer;
            }
            set
            {
                _frequentBuyer = value;
            }
        }
        [CategoryAttribute("Marketting Settings"), DescriptionAttribute("Most current e-mail of the customer")]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }
    }
}
