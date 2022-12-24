namespace Gudgeon.Modules.Fun.Tiles;

class BoardConverter
{
    public string ConvertToString(TilesCore core, string? footer = null)
    {
        string text = Emojis.BlackLargeSquare;

        for (int letter = 0; letter < core.BoardSize; letter++)
        {
            text += string.Format("{0} ", Emojis.Letters[letter]);
        }

        text += "\n";

        for (int row = 0; row < core.BoardSize; row++)
        {
            text += Emojis.Numbers[row];

            for (int column = 0; column < core.BoardSize; column++)
            {
                text += string.Format("{0} ", Emojis.Tiles[core.Board[row, column]]);
            }

            text += "\n";
        }

        if (footer != null)
        {
            text += footer;
        }

        return text;
    }
}