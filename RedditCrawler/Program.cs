using AwosFramework.Scraping;
using AwosFramework.Scraping.Binding;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Extensions.ReInject;
using AwosFramework.Scraping.Html;
using AwosFramework.Scraping.Html.Handler;
using AwosFramework.Scraping.ResultHandling.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RedditCrawler;
using ReInject;
using ReInject.Interfaces;
using System.Diagnostics;
using System.Net.Http.Headers;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


var container = Injector.GetContainer();

var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole().AddConfiguration(config.GetSection("Logging")); });
container.AddSingleton<IConfiguration>(config);
container.AddSingleton<IDependencyContainer>(container);
container.AddSingleton<System.IServiceProvider>(container);
container.AddSingleton<ILoggerFactory>(loggerFactory);
container.AddSingleton<ScraperConfiguration>(config.GetSection("ScrapingSettings").Get<ScraperConfiguration>());
container.AddSingleton<RedditConfig>(config.GetSection("RedditConfig").Get<RedditConfig>());
container.AddTransient<HttpClient>(() =>
{
	var client = new HttpClient();
	client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:125.0) Gecko/20100101 Firefox/125.0");
	client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
	client.DefaultRequestHeaders.Add("Connection", "keep-alive");
	return client;
});

container.AddDefaultBinders();
container.MapScrapeControllers();

var output = config.GetSection("Output").Get<OutputConfig>();
var results = new JsonResultHandler<UserData>(output.Path, output.SaveBatchSize);

using var scraper = new Scraper(loggerFactory, container.GetInstance<ScraperConfiguration>(), container)
	.WithResultHandler(results);

var watch = new Stopwatch();
watch.Start();
var cfg = container.GetService<RedditConfig>();
var jobs = cfg.SubReddits.Select(x => ScrapeJob.Get($"{cfg.Url}/r/{x}/{cfg.Sort}")).ToArray();

await scraper.RunAsync(jobs);
Console.WriteLine($"Done, after {watch.Elapsed.TotalSeconds:#.0}s");