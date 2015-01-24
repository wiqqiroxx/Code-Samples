using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using MailChimp;
using MailChimp.Helper;


namespace CommonClassLib.Integrations
{
    [DataContract]
    public class CustomMergeVars : MailChimp.Lists.MergeVar
    {

        public CustomMergeVars()
        {

            //
            // TODO: Add constructor logic here
            //
        }

        [DataMember(Name = "FNAME")]
        public string FirstName { get; set; }

        [DataMember(Name = "LNAME")]
        public string LastName { get; set; }
    }
}