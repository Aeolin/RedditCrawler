using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RedditCrawler
{
	public class OutputConfig
	{
		public string Path { get; set; } = "./output";
		public int SaveBatchSize { get; set; } = 1000;
	}
}
