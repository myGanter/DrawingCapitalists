using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.Game
{
    public enum GameState
    {
        Waiting,
        WordSelection,
        BasicDrawing,
        PostDrawing,
        GuessingWord
    }
}
