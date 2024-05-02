using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditCrawler
{
	public class RedditConfig
	{
		public string Url { get; set; }
		public string SubReddit { get; set; }
		public string Sort { get; set; }
		public int PostHistoryLength { get; set; } = 10;
		public bool DistinctPostSubreddits { get; set; } = true;
		public int UniqueUserCount { get; set; } = 50000;
	}
}
