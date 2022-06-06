namespace Changey.Options;

internal static class CompareOptionTexts
{
	public const string BaseUrl =
		"Base url for generated links. When using github.com or gitlab.com release and compare templates are automatically determined.";

	public const string CompareTemplate =
		"Template for an URL that compares two tags. Only needed when using an unknown base url.";

	public const string ReleaseTemplate =
		"Template for an URL that directly links to a release. Only needed when using an unknown base url.";
}