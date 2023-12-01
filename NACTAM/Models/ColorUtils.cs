
using SkiaSharp;

using Svg.Skia;

namespace NACTAM.Models;

/// <summary>
/// Color utility class for generating Profile picture
///
/// author: Tuan Bui
/// </summary>
public class ColorUtils {
	/// <summary>
	/// generates random color with the saturation of the original image
	///
	/// author: Tuan Bui
	/// </summary>
	public static string RandomProfileColor() {
		Random random = new Random();
		return SKColor.FromHsv((float)random.NextDouble() * 360.0f, 100.0f, (float)random.NextDouble() * 20.0f + 30.0f).ToString();
	}

	/// <summary>
	/// generates a random profile picture
	///
	/// author: Tuan Bui
	/// </summary>
	public static byte[] GenerateRandomProfilePicture() {
		var svgString = File.ReadAllText("./wwwroot/images/profilbildDefault.svg");
		var replacedColor = svgString.Replace("#009a95", RandomProfileColor().Remove(1, 2));
		SKSvg img = new SKSvg();
		img.FromSvg(replacedColor);
		using (var ms = new MemoryStream()) {
			img.Save(ms, new SKColor(210, 215, 215));
			ms.Seek(0, SeekOrigin.Begin);
			return ms.ToArray();
		}
	}
}
