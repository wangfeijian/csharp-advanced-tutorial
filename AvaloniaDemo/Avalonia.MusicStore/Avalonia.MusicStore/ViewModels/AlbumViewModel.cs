using System.IO;
using System.Threading.Tasks;
using Avalonia.MusicStore.Models;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Avalonia.MusicStore.ViewModels;

public class AlbumViewModel :ViewModelBase
{
    private readonly Album _album;

    public AlbumViewModel(Album album)
    {
        _album = album;
    }

    public string Artist => _album.Artist;
    public string Title => _album.Title;

    private Bitmap? _cover;

    public Bitmap? Cover
    {
        get => _cover;
        private set => this.RaiseAndSetIfChanged(ref _cover, value);
    }

    public async Task LoadCover()
    {
        await using var imageStream = await _album.LoadCoverBitmapAsync();
        
        // Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400)); // bug 非法内存
        Cover = await Task.Run(() => new Bitmap(imageStream).CreateScaledBitmap(new PixelSize(400,400)));
    }

    public async Task SaveToDiskAsync()
    {
        await _album.SaveAsync();

        if (Cover != null)
        {
            var bitmap = Cover;

            await Task.Run(() =>
            {
                using var fs = _album.SaveCoverBitmapStream();
                bitmap.Save(fs);
            });
        }
    }
}