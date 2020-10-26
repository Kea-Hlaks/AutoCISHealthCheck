using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCISHealthCheck
{
    class Creds
    {
        #region Declarations
        private string username;
        private string password;
        private int userId;
        private int roleId;
        private string usertype;
        private string roletype;
        #endregion

        #region Properties
        protected internal string Username
        {
            get { return username; }
            set { username = value; }
        }
        protected internal string Password
        {
            get { return password; }
            set { password = value; }
        }
        protected internal int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        protected internal int RoleId
        {
            get { return roleId; }
            set { roleId = value; }
        }
        protected internal string RoleType
        {
            get { return roletype; }
            set { roletype = value; }
        }
        protected internal string UserType
        {
            get { return usertype; }
            set { usertype = value; }
        }

        #endregion
    }
}
