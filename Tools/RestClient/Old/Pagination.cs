using Newtonsoft.Json;

namespace RestClient.Alfresco
{
    public class Pagination
    {
        public int count{ get; set;} //:1,
        public bool hasMoreItems{ get; set;} //:false,
        public int totalItems{ get; set;} //:1,
        public int skipCount{ get; set;} //:0,
        public int maxItems{ get; set;} //:10
    }
}
