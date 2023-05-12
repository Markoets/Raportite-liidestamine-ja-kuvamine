using Microsoft.AspNetCore.Mvc;
using SiseveebRaptortid.Models;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using SiseveebRaptortid.Services;
using Microsoft.Extensions.Caching.Memory;

namespace SiseveebRaptortid.Controllers
{

    public class HomeController : Controller
    {
        private ApiService _apiService;
        private readonly IMemoryCache _memoryCache;

        public HomeController(ApiService powerBiServiceApi, IMemoryCache memoryCache)
        {
            _apiService = powerBiServiceApi;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        public async Task<IActionResult> Raportid(string username, string password)
        {
            try
            {
                                                                                                                  //DC=test,DC=name
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, "Domeeni IP", "Active Directory domain distinguished name (DN)", "Kasutajanimi", "Kasutaja parool"))
                {
                    // Check if the user is authenticated
                    if (_memoryCache.Get("AuthToken") == "sees")
                    {

                        // Set initial values for the permissions

                        // Get the username from memory cache
                        username = _memoryCache.Get("UserName").ToString();

           

                        // Get the AD groups for the user
                        List<string> groups = await GetADGroups(username);

                        foreach (var i in groups)
                        {
                            _memoryCache.Set(i, i, TimeSpan.FromMinutes(30));
                        }
                        // Call the Power BI API service to get the reports
                        var viewModel = await _apiService.GetReportsEmbeddingData();
                                     // Add the permissions to the ViewBag
                        ViewBag.groups = string.Join(", ", groups);
                        return View(viewModel);
                    }
                    else
                    {
                        //Validate the user credentials

                        bool isAuthenticated = context.ValidateCredentials(username, password);
                        if (isAuthenticated)
                        {
                            // Get the AD groups for the user
                            List<string> groups = await GetADGroups(username);

                            foreach (var i in groups)
                            {
                                _memoryCache.Set(i, i, TimeSpan.FromMinutes(30));
                            }
                            // Call the Power BI API service to get the reports
                            var viewModel = await _apiService.GetReportsEmbeddingData();
                            UserPrincipal user = UserPrincipal.FindByIdentity(context, username);

                            // Add the user to the memory cache
                            _memoryCache.Set("AuthToken", "sees", TimeSpan.FromMinutes(30));
                            _memoryCache.Set("UserName", username, TimeSpan.FromMinutes(30));
                            // Add the permissions to the ViewBag
                            ViewBag.groups = string.Join(", ", groups);
                            return View(viewModel);
                        }
                        else
                        {

                            return View("login");
                        }
                    }
                }
            }
            catch (System.Exception)
            {

                ViewBag.fail = ("Vale kasutajanimi või parool");
                return View("login");
            }

        }

        public IActionResult Logout()
        {
            // Clear the memory cache
            _memoryCache.Remove("AuthToken");
            _memoryCache.Remove("UserName");
            return RedirectToAction("Index", "Home"); 

        }
        public async Task<List<string>> GetADGroups(string username)
        {
            List<string> groups = new List<string>();
            // Create a DirectoryEntry object for the LDAP server.
            var domain = new DirectoryEntry("LDAP://IP-aadress", "Kasutajanimi", "Kasutaja parool");
            var searcher = new DirectorySearcher(domain);
            // Set the LDAP search filter and properties to return
            searcher.Filter = "((displayName=*))";
            searcher.PropertiesToLoad.Add("samAccountName");
            searcher.PropertiesToLoad.Add("memberOf");
            var results = searcher.FindAll();
            // Loop through the results
            foreach (SearchResult result in searcher.FindAll())
            {
                var resultGroups = result.Properties["memberOf"];
                foreach (var group in resultGroups)
                {
                    username = username.ToLower();
                    string AccountName = result.Properties["samAccountName"][0].ToString().ToLower();
                    // Check if the username matches the result from the LDAP query
                    if (AccountName == username)
                    {
                        var samAccountName = result.Properties["samAccountName"][0].ToString();
                        var groupname = group.ToString().Split(',')[0].Split('=')[1];
                        var user = new User
                        {
                            SamAccountName = username,
                            Group = groupname,
                        };
                        // Add the group to the list
                        groups.Add(user.Group);

                    }

                }
            }


            return groups;

        }

        public IActionResult login()
        {
            return View();
        }

    }
}