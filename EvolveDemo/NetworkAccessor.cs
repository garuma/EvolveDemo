using System;
using System.Net;

namespace EvolveDemo
{
	public static class NetworkAccessor
	{
		static NetworkAccessor ()
		{
			//ProxyUrl = 
		}

		public static string ProxyUrl {
			get;
			set;
		}

		public static WebClient Setup (this WebClient client, ref string url)
		{
			if (string.IsNullOrEmpty (ProxyUrl))
				return client;
			var uri = new Uri (url);
			client.Headers.Add ("X-Forward-To", uri.Scheme + "://" + uri.Host);
			url = ProxyUrl + uri.PathAndQuery;
			return client;
		}
	}
}

