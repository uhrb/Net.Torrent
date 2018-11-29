# Net.Torrent
Net.Torrent is torrent files (and any b-encoded files) parsing library for .NET Core
Features:
  - Serialize/Deserialize b-encoded entities (including torrent files)
  - Manipulate torrent data (including creation of new torrents from scratch)

### Examples
Read torrent file
```csharp
using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
{
    var parser = new TorrentSerializer();
    var trt = parser.Deserialize(file);
}
```
Create new torrent file
```csharp
var builder = new TorrentBuilder(Encoding.UTF8);
builder.AddFile("directory/file1.txt", 20L);
builder.AddFile("directory/file2.txt", 40L);
builder.AddFile("directory/file3.txt", 50L);
builder.SetPieceLength(16384);
builder.SetAnnounce(new Uri("http://something"));
builder.CalculatePieces(new FileStreamProvider());
var torrent = builder.Build();
```
Write torrent file
```csharp
Torrent torrent = CreateTorrent();
using(var file = File.Create("something.torrent"))
{
    new TorrentSerializer().Serialize(file, torrent);
}
```
### Development
Feel free to fork and improve. I will be very pleased by pull requests.
### License
Copyright 2018 Uladzimir Harabtsou

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.