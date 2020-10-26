using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCISHealthCheck
{
    class Search_Data
    {
        #region Declarations
        private string province;
        private string parcelType;
        private string administrativeDistrict;
        private int farmNumber;
        private int provinceIndex;
        private string provinceValue;
        #endregion

        #region Properties
        public string Province
        {
            get { return province; }
            set { province = value; }
        }
        public string ParcelType
        {
            get { return parcelType; }
            set { parcelType = value; }
        }
        public string AdministrativeDistrict
        {
            get { return administrativeDistrict; }
            set { administrativeDistrict = value; }
        }
        public int FarmNumber
        {
            get { return farmNumber; }
            set { farmNumber = value; }
        }
        public int ProvinceIndex
        {
            get { return provinceIndex; }
            set { provinceIndex = value; }
        }
        public string ProvinceValue
        {
            get { return provinceValue; }
            set { provinceValue = value; }
        }
        #endregion
    }
}
