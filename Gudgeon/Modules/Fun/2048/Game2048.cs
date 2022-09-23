using Discord;

namespace Gudgeon.Modules.Fun._2048;

internal class Game2048
{
    public IUser Player { get; init; }
    public int Score { get; private set; }
    public int[,] Board { get; private set; }
    public Game2048(IUser player)
    {
        Player = player;
        Board = new int[4, 4];
        TryGenerateTile();
    }

    public static readonly Dictionary<int, string> Emojis = new()
    {
        [0] = "<:tile_empty:971319911131533342>",
        [2] = "<:tile_2:971319143464849419>",
        [4] = "<:tile_4:971323661644603402>",
        [8] = "<:tile_8:971344853688520774>",
        [16] = "<:tile_16:971346172629372928>",
        [32] = "<:tile_32:971346568219340830>",
        [64] = "<:tile_64:971348814357209108>",
        [128] = "<:tile_128:971334224953094164>",
        [256] = "<:tile_256:971334918116343848>",
        [512] = "<:tile_512:971334880191455263>",
        [1024] = "<:tile_1024:971335974640558131>",
        [2048] = "<:tile_2048:971336371899883560>",
    };
    private readonly Random Random = new();
    private const int MaxBoardSize = 4;

    public bool HasMaxTile()
    {
        for (int row = 0; row < MaxBoardSize; row++)
        {
            for (int column = 0; column < MaxBoardSize; column++)
            {
                if (Board[row, column] == 2048)
                    return true;
            }
        }
        return false;
    }
    public bool TryGenerateTile()
    {
        List<Tuple<int, int>> emptySlots = new();
        for (int row = 0; row < MaxBoardSize; row++)
        {
            for (int column = 0; column < MaxBoardSize; column++)
            {
                if (Board[row, column] == 0)
                {
                    emptySlots.Add(new Tuple<int, int>(row, column));
                }
            }
        }

        if (emptySlots.Count != 0)
        {
            int tile = Random.Next(0, emptySlots.Count);
            int value = Random.Next(0, 100) < 95 ? 2 : 4;
            Score += value;
            Board[emptySlots[tile].Item1, emptySlots[tile].Item2] = value;
        }

        int score = 0;
        if (!TryMoveBoard((int[,])Board.Clone(), ref score, Direction.Up) &&
            !TryMoveBoard((int[,])Board.Clone(), ref score, Direction.Down) &&
            !TryMoveBoard((int[,])Board.Clone(), ref score, Direction.Left) &&
            !TryMoveBoard((int[,])Board.Clone(), ref score, Direction.Right))
            return false;
        return true;
    }

    public bool TryMoveBoard(Direction direction)
    {
        int score = 0;

        if (TryMoveBoard(Board, ref score, direction))
        {
            Score += score;
            return true;
        }
        return false;
    }
    private bool TryMoveBoard(int[,] board, ref int score, Direction direction)
    {
        (int X, int Y) Adjacent(int x, int y) =>
            direction switch
            {
                Direction.Up => (x - 1, y),
                Direction.Down => (x + 1, y),
                Direction.Left => (x, y - 1),
                Direction.Right => (x, y + 1),
                _ => throw new NotImplementedException(),
            };

        (int X, int Y) Map(int x, int y) =>
            direction switch
            {
                Direction.Up => (x, y),
                Direction.Down => (board.GetLength(0) - x - 1, y),
                Direction.Left => (x, y),
                Direction.Right => (x, board.GetLength(1) - y - 1),
                _ => throw new NotImplementedException(),
            };

        bool[,] locked = new bool[board.GetLength(0), board.GetLength(1)];
        bool update = false;

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                var (tempi, tempj) = Map(i, j);
                if (board[tempi, tempj] == 0)
                {
                    continue;
                }
                KeepChecking:
                var (adji, adjj) = Adjacent(tempi, tempj);
                if (adji < 0 || adji >= board.GetLength(0) ||
                    adjj < 0 || adjj >= board.GetLength(1) ||
                    locked[adji, adjj])
                {
                    continue;
                }
                else if (board[adji, adjj] == 0)
                {
                    board[adji, adjj] = board[tempi, tempj];
                    board[tempi, tempj] = 0;
                    update = true;
                    tempi = adji;
                    tempj = adjj;
                    goto KeepChecking;
                }
                else if (board[adji, adjj] == board[tempi, tempj])
                {
                    board[adji, adjj] += board[tempi, tempj];
                    score += board[adji, adjj]!;
                    board[tempi, tempj] = 0;
                    update = true;
                    locked[adji, adjj] = true;
                }
            }
        }
        return update;
    }

    private readonly string[] RowNumbers =
    {
        ":one:",
        ":two:",
        ":three:",
        ":four:"
    };
    public string GetDisplayBoard()
    {
        string text = ":black_large_square::regional_indicator_a: :regional_indicator_b: :regional_indicator_c: :regional_indicator_d:\n";
        for (int i = 0; i < MaxBoardSize; i++)
        {
            text += RowNumbers[i];
            for (int j = 0; j < MaxBoardSize; j++)
            {
                text += string.Format("{0, 4}" + " ", Emojis[Board[i, j]]);
            }
            text += "\n";
        }
        return text += $"{Player.Mention} | score: {Score}";
    }
}