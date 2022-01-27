using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Timestream
{
    public class Compression
    {
        private async Task<string> ToCompressedStringAsync(string value, CompressionLevel level, Func<Stream, Stream> createCompressionStream)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            await using var input = new MemoryStream(bytes);
            await using var output = new MemoryStream();
            await using var stream = createCompressionStream(output);

            await input.CopyToAsync(stream);
            await stream.FlushAsync();

            var result = output.ToArray();
            return Convert.ToBase64String(result);
        }

        private async Task<string> FromCompressedStringAsync(string value, Func<Stream, Stream> createDecompressionStream)
        {
            var bytes = Convert.FromBase64String(value);
            await using var input = new MemoryStream(bytes);
            await using var output = new MemoryStream();
            await using var stream = createDecompressionStream(input);

            await stream.CopyToAsync(output);
            await output.FlushAsync();

            var result = output.ToArray();
            return Encoding.Unicode.GetString(result);
        }

        public async Task<string> ToBrotliAsync(string value, CompressionLevel level = CompressionLevel.Optimal)
            => await ToCompressedStringAsync(value, level, s => new BrotliStream(s, level));

        public async Task<string> FromBrotliAsync(string value)
            => await FromCompressedStringAsync(value, s => new BrotliStream(s, CompressionMode.Decompress));
    }
}