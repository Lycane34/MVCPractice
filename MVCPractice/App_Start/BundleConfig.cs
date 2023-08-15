using System.Web;
using System.Web.Optimization;

namespace MVCPractice
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            StyleBundle(bundles);
            ScriptBundle(bundles);
        }
    
        public static void StyleBundle(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css").Include("~/Content/bootstrap.css"));
        }

        public static void ScriptBundle(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js").Include("~/Scripts/jquery-{version}.js").Include("~/Scripts/bootstrap.js"));
        }
    
    
    
    }

}
