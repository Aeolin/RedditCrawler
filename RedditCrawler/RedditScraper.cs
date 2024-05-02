using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Html;
using AwosFramework.Scraping.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditCrawler
{
	public class RedditScraper : ScrapeController
	{
		private static ConcurrentBag<string> _crawledUsers = new ConcurrentBag<string>();
		private readonly RedditConfig _reddit;
		private readonly ILogger _logger;

		public RedditScraper(RedditConfig reddit, ILoggerFactory loggerFactory)
		{
			_reddit = reddit;
			_logger = loggerFactory.CreateLogger<RedditScraper>();
		}

		[Route("https://old.reddit.com/r/{subreddit}/{sort}")]
		public IScrapeResult ScrapeSubRedditAsync(string subreddit, string sort, [FromCss(".self .author", Attribute = "href")] IEnumerable<string> authors, [FromCss(".next-button a", Attribute = "href")] string next)
		{
			var jobs = new List<ScrapeJob>();
			foreach (var user in authors)
			{
				var name = user.Split("/").Last();
				if (_crawledUsers.Contains(name))
					continue;

				if (_crawledUsers.Count >= _reddit.UniqueUserCount)
				{
					_logger.LogInformation("Unique user count reached");
					return Follow(jobs);
				}

				_crawledUsers.Add(name);
				var data = new UserData { UserId = name };
				jobs.Add(ScrapeJob.Get(user + "/submitted?sort=new", 1, data, true));
			}

			if (string.IsNullOrEmpty(next) == false && _crawledUsers.Count < _reddit.UniqueUserCount)
				jobs.Add(ScrapeJob.Get(next, 0));

			return Follow(jobs);
		}

		[Route("https://old.reddit.com/user/{userName}/submitted")]
		public IScrapeResult ScrapeUserPostsAsync(string userName, [FromJob] UserData data, [FromCss("#siteTable > .thing")] IEnumerable<Post> posts, [FromCss(".next-button a", Attribute = "href")] string next)
		{
			if (_reddit.DistinctPostSubreddits)
				posts = posts.DistinctBy(x => x.Subreddit);

			posts = posts.Take(_reddit.PostHistoryLength - data.PostHistory.Count);
			data.PostHistory.AddRange(posts);
			if (data.PostHistory.Count < _reddit.PostHistoryLength && next != null)
				return Follow(ScrapeJob.Get(next, 1, data, true));
			else
				return Ok(data);
		}
	}
}
