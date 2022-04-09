using Core.Services.AppState;
using Core.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Services.Game
{
    public class GameLogic
    {
        public GameState CurrentGameState { get; private set; }

        private Dictionary<UserStruct, List<string>> _UsersWords;
        public Dictionary<UserStruct, List<string>> UsersWords 
        {
            get => _UsersWords;
            set
            {
                _UsersWords = value;
                UsersWord = new Dictionary<UserStruct, string>();
            }
        }

        private Dictionary<UserStruct, string> UsersWord;

        private CancellationTokenSource TokenSource;

        private readonly GameRoom RoomObj;

        public GameLogic(GameRoom roomObj)
        {
            RoomObj = roomObj;
            CurrentGameState = GameState.Waiting;
            TokenSource = new CancellationTokenSource();
        }

        public void ChooseWord(UserStruct user, string word)
        {
            var userWords = _UsersWords[user];
            if (userWords.Contains(word))
            {
                lock (UsersWord)
                {
                    UsersWord[user] = word;
                }
            }                
        }

        public bool AllUserChooseWors()
        {
            return _UsersWords.Count == UsersWord.Count;
        }

        public void NormalizeUsersWord()
        {
            foreach (var i in _UsersWords)
            {
                if (!UsersWord.ContainsKey(i.Key))
                {
                    UsersWord.Add(i.Key, i.Value[0]);
                }
            }
        }

        public async Task SetNextState()
        {
            TokenSource.Cancel();
            TokenSource.Dispose();
            TokenSource = new CancellationTokenSource();

            await RunStateTask();

            var nextGameStateIndex = ((int)CurrentGameState + 1) % 5;
            CurrentGameState = (GameState)nextGameStateIndex;                      
        }

        private async Task RunStateTask()
        {
            switch (CurrentGameState)
            {
                case GameState.Waiting:
                    await Waiting();
                    break;
                case GameState.WordSelection:
                    await WordSelection();
                    break;
                case GameState.BasicDrawing:
                    await BasicDrawing();
                    break;
                case GameState.PostDrawing:
                    await PostDrawing();
                    break;
                case GameState.GuessingWord:
                    await GuessingWord();
                    break;
            }
        }

        private async void StartDeferredTask(int milis, Func<Task> clbk)
        {
            var token = TokenSource.Token;

            await Task.Delay(milis);

            if (token.IsCancellationRequested)
                return;

            await clbk();
        }

        private async Task Waiting()
        {
            var all = RoomObj.GetAllConnections();
            await GameHub.HubContext.Clients.Clients(all).SendAsync("SetGameState", GameState.WordSelection.ToString());
            StartDeferredTask(30000, WordSelection);
        }

        private async Task WordSelection()
        {
            if (!AllUserChooseWors())
                NormalizeUsersWord();

            foreach (var uw in UsersWord)
            {
                var connectId = RoomObj[uw.Key];
                await GameHub.HubContext.Clients.Client(connectId).SendAsync("SetChooseWord", uw.Value);
            }

            var all = RoomObj.GetAllConnections();
            await GameHub.HubContext.Clients.Clients(all).SendAsync("SetGameState", GameState.BasicDrawing.ToString());
            StartDeferredTask(120000, BasicDrawing);
        }

        private async Task BasicDrawing()
        {
            var all = RoomObj.GetAllConnections();
            await GameHub.HubContext.Clients.Clients(all).SendAsync("SetGameState", GameState.Waiting.ToString());
            CurrentGameState = GameState.Waiting;
        }

        private async Task PostDrawing()
        {

        }

        private async Task GuessingWord()
        {

        }
    }
}
