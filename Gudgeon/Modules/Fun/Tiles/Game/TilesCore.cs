﻿using System.Text;
using Discord;

namespace Gudgeon.Modules.Fun.Tiles;

internal class TilesCore
{
    private int[,] _board;
    private readonly Random _random = new();
    public const int MaxTile = 2048;
    public Player Player { get; init; }
    public int MaxBoardSize { get; init; }
    public TilesCore(Player player, int maxBoardSize)
    {
        Player = player;
        MaxBoardSize = maxBoardSize;
        _board = new int[MaxBoardSize, MaxBoardSize];
        TryGenerateTile();
    }

    public bool HasMaxTile()
    {
        foreach (int tile in _board)
        {
            if (tile == MaxTile)
                return true;
        }

        return false;
    }

    public bool CanShiftBoard()
    {
        int score = 0;
        if (!TryMoveBoard((int[,])_board.Clone(), ref score, Direction.Up) &&
            !TryMoveBoard((int[,])_board.Clone(), ref score, Direction.Down) &&
            !TryMoveBoard((int[,])_board.Clone(), ref score, Direction.Left) &&
            !TryMoveBoard((int[,])_board.Clone(), ref score, Direction.Right))
            return false;
        return true;
    }

    public bool TryGenerateTile()
    {
        List<Tuple<int, int>> emptySlots = new();
        for (int row = 0; row < MaxBoardSize; row++)
        {
            for (int column = 0; column < MaxBoardSize; column++)
            {
                if (_board[row, column] == 0)
                {
                    emptySlots.Add(new Tuple<int, int>(row, column));
                }
            }
        }

        if (emptySlots.Count != 0)
        {
            int tile = _random.Next(0, emptySlots.Count);
            int value = _random.Next(0, 100) < 95 ? 2 : 4;
            Player.Score += value;
            _board[emptySlots[tile].Item1, emptySlots[tile].Item2] = value;
            return true;
        }
        return false;
    }

    public bool TryMoveBoard(Direction direction)
    {
        int score = 0;

        if (TryMoveBoard(_board, ref score, direction))
        {
            TryGenerateTile();
            Player.Score += score;
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
                Direction.Down => (MaxBoardSize - x - 1, y),
                Direction.Left => (x, y),
                Direction.Right => (x, MaxBoardSize - y - 1),
                _ => throw new NotImplementedException(),
            };

        bool[,] locked = new bool[MaxBoardSize, MaxBoardSize];
        bool update = false;

        for (int row = 0; row < MaxBoardSize; row++)
        {
            for (int column = 0; column < MaxBoardSize; column++)
            {
                var (tempi, tempj) = Map(row, column);
                if (board[tempi, tempj] == 0)
                {
                    continue;
                }
            KeepChecking:
                var (adji, adjj) = Adjacent(tempi, tempj);
                if (adji < 0 || adji >= MaxBoardSize ||
                    adjj < 0 || adjj >= MaxBoardSize ||
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

    public string ToDiscordMessageContent()
    {
        StringBuilder builder = new(":black_large_square:", DiscordConfig.MaxMessageSize);

        for (int letter = 0; letter < MaxBoardSize; letter++)
        {
            builder.AppendFormat("{0} ", Emojis.Letters[letter]);
        }

        builder.AppendLine();

        for (int row = 0; row < MaxBoardSize; row++)
        {
            builder.Append(Emojis.Numbers[row]);
            
            for (int column = 0; column < MaxBoardSize; column++)
            {
                builder.AppendFormat("{0} ", Emojis.Tiles[_board[row, column]]);
            }

            builder.AppendLine();
        }
        
        builder.Append($"{Player.User.Mention} | score: **{Player.Score}**");

        return builder.ToString();
    }
}