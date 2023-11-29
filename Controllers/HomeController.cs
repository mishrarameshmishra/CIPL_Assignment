using CIPL_Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;


namespace CIPL_Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            string url = "https://dghindia.gov.in/index.php/all_story?type=1&page=notice";
            string htmlContent = await GetHtmlContentAsync(url);
            var ulData = ParseHtml(htmlContent);
            return View(ulData);
        }
        private async Task<string> GetHtmlContentAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        private List<NoticeModel> ParseHtml(string htmlContent)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            var newsItems = new List<NoticeModel>();
            var ulNode = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@style, 'padding:20px;')]/ul");

            if (ulNode != null)
            {
                var liNodes = ulNode.SelectNodes(".//li");

                if (liNodes != null)
                {
                    foreach (var liNode in liNodes)
                    {
                        var aNode = liNode.SelectSingleNode(".//a");
                        if (aNode != null)
                        {
                            var href = aNode.GetAttributeValue("href", "");
                            var title = aNode.InnerText.Trim();
                            if (href.Contains("dghindia.gov.in"))
                            {
                                var newsItem = new NoticeModel { Url = href, Title = title, UrlType = "Internal" };
                                newsItems.Add(newsItem);
                            }
                            else
                            {
                                var newsItem = new NoticeModel { Url = href, Title = title, UrlType = "External" };
                                newsItems.Add(newsItem);
                            }
                        }
                    }
                }
            }

            return newsItems;
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
    }

}
