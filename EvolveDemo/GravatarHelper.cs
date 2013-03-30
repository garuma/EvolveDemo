using System;

namespace EvolveDemo
{
	public static class GravatarHelper
	{
		public static string MakeUrl (string id, int size = 48)
		{
			return "http://www.gravatar.com/avatar/" + id + "?s=" + size + "&d=404";
		}
	}
}

