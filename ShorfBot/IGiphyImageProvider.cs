namespace ShorfBot
{
    public interface IGiphyImageProvider
    {
        GiphyData GetRandomGiphyImageData(string tags);
    }
}
