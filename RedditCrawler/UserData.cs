using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditCrawler
{
	public class UserData
	{
		public string UserId { get; set; }
		public List<Post> PostHistory { get; set; } = new List<Post>();
	}
}
