using AwosFramework.Scraping.Html.Css;
using AwosFramework.Scraping.Html.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditCrawler
{
	public class Post
	{
		[XPath(".", Attribute = "data-timestamp")]
		public ulong TimeStamp { get; set; }

		[XPath(".", Attribute = "data-subreddit")]
		public string Subreddit { get; set; }

		[Css(".title a")]
		public string Title { get; set; }
		
		[XPath(".", Attribute = "data-fullname")]
		public string PostId { get; set; }
	}
}
