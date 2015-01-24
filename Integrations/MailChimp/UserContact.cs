using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLib.Integrations.MailChimpAndConstantContact.MailChimp
{
   public class UserContact
    {

        public UserContact(string frstname, string lstname, string emailAddr)
        {
            this.fname = frstname;
            this.lname = lstname;
            this.email = emailAddr;

        }

        private string fname;
        private string lname;
        private string email;


        public string FirstName
        {
            get { return fname; }
            set { fname = value; }
        }

        public string Lastname
        {
            get { return lname; }
            set { lname = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}
