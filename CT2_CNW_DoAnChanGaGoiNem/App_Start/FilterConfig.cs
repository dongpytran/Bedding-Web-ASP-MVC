using System.Web;
using System.Web.Mvc;

namespace CT2_CNW_DoAnChanGaGoiNem
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
