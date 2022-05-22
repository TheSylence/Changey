namespace Changey.Services;

internal record TemplateSet(string Release, string Compare, string Base)
{
	public bool Empty => string.IsNullOrEmpty(Release) || string.IsNullOrEmpty(Compare) || string.IsNullOrEmpty(Base);
}