using System;
using ServiceStack.Text;

namespace EvolveDemo
{
	public enum GitHubEventType {
		CommitCommentEvent,
		CreateEvent,
		DeleteEvent,
		DownloadEvent,
		FollowEvent,
		ForkEvent,
		GistEvent,
		IssueCommentEvent,
		IssuesEvent,
		MemberEvent,
		PublicEvent,
		PullRequestEvent,
		PullRequestReviewCommentEvent,
		PushEvent,
		TeamAddEvent,
		WatchEvent
	}
	
	public class GitHubEvent
	{
		public string Id { get; set; }
		public GitHubEventType Type { get; set; }
		public GitHubRepo Repo { get; set; }
		public GitHubActor Actor { get; set; }
		public JsonObject Payload { get; set; }
		public bool Consummed { get; set; }
	}
	
	public class GitHubRepo
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	
	public class GitHubActor
	{
		public string Id { get; set; }
		public string Login { get; set; }
		public string Url { get; set; }
		public string Avatar_Url { get; set; }
		public string Gravatar_Id { get; set; }
	}

	static class GitHubIconsUtils
	{
		public static char GetIconCharForEventType (GitHubEventType type)
		{
			switch (type) {
			case GitHubEventType.CommitCommentEvent:
				return '\uf24f';
			case GitHubEventType.CreateEvent:
				return '\uf203';
			case GitHubEventType.DeleteEvent:
				return '\uf204';
			case GitHubEventType.DownloadEvent:
				return '\uf20b';
			case GitHubEventType.FollowEvent:
				return '\uf21c';
			case GitHubEventType.ForkEvent:
				return '\uf220';
			case GitHubEventType.GistEvent:
				return '\uf27a';
			case GitHubEventType.IssueCommentEvent:
				return '\uf029';
			case GitHubEventType.IssuesEvent:
				return '\uf026';
			case GitHubEventType.MemberEvent:
				return '\uf21a';
			case GitHubEventType.PublicEvent:
				return '\uf024';
			case GitHubEventType.PullRequestEvent:
				return '\uf022';
			case GitHubEventType.PullRequestReviewCommentEvent:
				return '\uf02b';
			case GitHubEventType.PushEvent:
				return '\uf205';
			case GitHubEventType.TeamAddEvent:
				return '\uf019';
			case GitHubEventType.WatchEvent:
				return '\uf01d';
			default:
				return '\uf050';
			}
		}
	}
}

