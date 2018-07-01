using System;
using Newtonsoft.Json;

namespace RestClient.Alfresco
{
    public class Entry
    {
        virtual public string id { get; set;} //" "4a2919e1-d82d-428b-86f7-dfb32ca2c413",
        virtual public bool isFile { get; set;} //": true,;//
        
        virtual public DateTime modifiedAt { get; set;} //": "2018-06-08T08:58:31.225+0000",
        virtual public string name { get; set;} //: "928_4.PDF",
    }
}
