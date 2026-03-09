namespace Shoplio.ConsoleApp.Utils;

/// <summary>
/// Utility class for rendering ASCII charts and progress bars in the console.
/// </summary>
public static class ConsoleChart
{
    /// <summary>
    /// Renders a horizontal bar chart for the given data.
    /// </summary>
    /// <param name="items">Collection of items with labels and values.</param>
    /// <param name="maxBarWidth">Maximum width of the bar in characters (default: 40).</param>
    public static void RenderBarChart(IEnumerable<(string Label, decimal Value)> items, int maxBarWidth = 40)
    {
        var data = items.ToList();
        if (!data.Any())
        {
            Console.WriteLine("No data to display.");
            return;
        }

        var maxValue = data.Max(x => x.Value);
        if (maxValue == 0)
        {
            Console.WriteLine("All values are zero.");
            return;
        }

        var maxLabelLength = data.Max(x => x.Label.Length);

        foreach (var item in data)
        {
            var barLength = (int)((item.Value / maxValue) * maxBarWidth);
            var bar = new string('█', barLength);
            var padding = new string(' ', maxLabelLength - item.Label.Length);
            
            Console.WriteLine($"{item.Label}{padding} | {bar} ${item.Value:F2}");
        }
    }

    /// <summary>
    /// Renders a horizontal bar chart with count values (no dollar signs).
    /// </summary>
    /// <param name="items">Collection of items with labels and counts.</param>
    /// <param name="maxBarWidth">Maximum width of the bar in characters (default: 40).</param>
    public static void RenderCountChart(IEnumerable<(string Label, int Count)> items, int maxBarWidth = 40)
    {
        var data = items.ToList();
        if (!data.Any())
        {
            Console.WriteLine("No data to display.");
            return;
        }

        var maxValue = data.Max(x => x.Count);
        if (maxValue == 0)
        {
            Console.WriteLine("All values are zero.");
            return;
        }

        var maxLabelLength = data.Max(x => x.Label.Length);

        foreach (var item in data)
        {
            var barLength = (int)((double)item.Count / maxValue * maxBarWidth);
            var bar = new string('█', barLength);
            var padding = new string(' ', maxLabelLength - item.Label.Length);
            
            Console.WriteLine($"{item.Label}{padding} | {bar} {item.Count}");
        }
    }

    /// <summary>
    /// Renders a progress bar showing percentage completion.
    /// </summary>
    /// <param name="current">Current value.</param>
    /// <param name="maximum">Maximum value.</param>
    /// <param name="width">Width of the progress bar (default: 20).</param>
    /// <returns>A string representing the progress bar.</returns>
    public static string RenderProgressBar(int current, int maximum, int width = 20)
    {
        if (maximum == 0) return $"[{new string('░', width)}] 0%";

        var percentage = (double)current / maximum;
        var filled = (int)(percentage * width);
        var empty = width - filled;

        var bar = new string('█', filled) + new string('░', empty);
        return $"[{bar}] {percentage * 100:F0}%";
    }

    /// <summary>
    /// Renders a stock level indicator with color-coded status.
    /// </summary>
    /// <param name="stockLevel">Current stock level.</param>
    /// <param name="threshold">Low stock threshold.</param>
    /// <param name="width">Width of the indicator bar (default: 15).</param>
    /// <returns>A string representing the stock indicator.</returns>
    public static string RenderStockIndicator(int stockLevel, int threshold, int width = 15)
    {
        if (stockLevel <= 0)
        {
            return $"[{new string('░', width)}] OUT OF STOCK";
        }

        if (stockLevel <= threshold)
        {
            var filled = Math.Min(stockLevel, width);
            var bar = new string('▓', filled) + new string('░', width - filled);
            return $"[{bar}] LOW ({stockLevel})";
        }

        var normalFilled = Math.Min(stockLevel / 10, width);
        var normalBar = new string('█', normalFilled) + new string('░', width - normalFilled);
        return $"[{normalBar}] OK ({stockLevel})";
    }
}
