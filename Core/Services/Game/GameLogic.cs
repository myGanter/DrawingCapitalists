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

        private CancellationTokenSource TokenSource;

        private readonly GameRoom RoomObj;

        public GameLogic(GameRoom roomObj)
        {
            RoomObj = roomObj;
            CurrentGameState = GameState.Waiting;
            TokenSource = new CancellationTokenSource();
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
            await GameHub.HubContext.Clients.Clients(all).SendAsync("ShowClientMessage", new ClientMessage(ClientMessageType.Common, "Отработал Waiting"));
            StartDeferredTask(10000, WordSelection);
        }

        private async Task WordSelection()
        {
            var all = RoomObj.GetAllConnections();
            await GameHub.HubContext.Clients.Clients(all).SendAsync("ShowClientMessage", new ClientMessage(ClientMessageType.Common, "Ну типо тут таймер отработал на выбор слов"));
        }

        private async Task BasicDrawing()
        {

        }

        private async Task PostDrawing()
        {

        }

        private async Task GuessingWord()
        {

        }
    }
}
