// See https://aka.ms/new-console-template for more information

using CommunityToolkit.HighPerformance;
using SkiaSharp;

Console.WriteLine("Texture Stitcher Proof of Concept");

// Read Data

Console.Write("File Name: ");
var file = Console.ReadLine();
// Load Texture Set File
var imagePath = AppDomain.CurrentDomain.BaseDirectory + $"/{file}";
var image = SKImage.FromEncodedData(imagePath);

Console.Write("Size X: ");
var sizeX = Convert.ToInt32(Console.ReadLine());

Console.Write("Size Y: ");
var sizeY = Convert.ToInt32(Console.ReadLine());

Console.Write("Resolution: ");
var resolution = Convert.ToInt32(Console.ReadLine());

Console.Write("Seed: ");
var seed = Convert.ToInt32(Console.ReadLine());
// Create Random
var random = new Random(seed);

// Load Texture Set
var textureSet = SKBitmap.FromImage(image) ?? new SKBitmap(resolution * 4, resolution * 4);
// Create Result Bitmap
var result = new SKBitmap(resolution * sizeX, resolution * sizeY);

// Create Canvas for Result
using (var resultCanvas = new SKCanvas(result))
{
    // Create Map of placed Tiles
    var textures = new int[sizeX, sizeY];
    // Set unplaced Tiles to -1
    textures.AsSpan2D().Fill(-1);

    // Left to Right, Top to Bottom
    for (var x = 0; x < sizeX; x++)
    for (var y = 0; y < sizeY; y++)
    {
        // Get a Tile that fits
        var imageId = GetRandomImage(x, y, textures, sizeX, sizeY, random);
        // Write placed Tile to Map
        textures[x, y] = imageId;
        // Create Source Map Coordinates
        var left = imageId % 4 * resolution;
        var top = imageId / 4 * resolution;
        var source = new SKRect(left, top, left + resolution, top + resolution);
        // Create Result Coordinates
        var target = new SKRect(x * resolution, y * resolution, (x + 1) * resolution, (y + 1) * resolution);
        // Place Tile
        resultCanvas.DrawBitmap(textureSet, source, target);
    }

    // Draw Result
    resultCanvas.Flush();
}

// Write Result to out.png
var data = result.Encode(SKEncodedImageFormat.Png, 100);
var outPath = AppDomain.CurrentDomain.BaseDirectory + "out.png";
File.WriteAllBytes(outPath, data.ToArray());

// Gets a random tile, that fits in the given location based on a map
int GetRandomImage(int x, int y, int[,] tx, int width, int height, Random rng)
{
    // If out of bounds or not set generate random, if not check opposite side and match it
    var left = x <= 0 ? rng.Next(0, 2) : tx[x - 1, y] == -1 ? rng.Next(0, 2) : TestBit(tx[x - 1, y], 2);
    var bottom = y >= height - 1 ? rng.Next(0, 2) : tx[x, y + 1] == -1 ? rng.Next(0, 2) : TestBit(tx[x, y + 1], 3);
    var right = x >= width - 1 ? rng.Next(0, 2) : tx[x + 1, y] == -1 ? rng.Next(0, 2) : TestBit(tx[x + 1, y], 0);
    var top = y <= 0 ? rng.Next(0, 2) : tx[x, y - 1] == -1 ? rng.Next(0, 2) : TestBit(tx[x, y - 1], 1);
    // Create Tile ID
    return left | (bottom << 1) | (right << 2) | (top << 3);
}

// Returns a given bit from an int
int TestBit(int n, int pos)
{
    return (n >> pos) & 1;
}