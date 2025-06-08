public class ShowOptionsSearch
{
    public bool IsShow { get; }
    public System.Type SearchType { get; }

    public ShowOptionsSearch(bool isShow, System.Type searchType)
    {
        IsShow = isShow;
        SearchType = searchType;
    }
}
