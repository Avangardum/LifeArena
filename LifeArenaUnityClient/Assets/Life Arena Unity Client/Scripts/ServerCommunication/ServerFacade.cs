using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Avangardum.AvangardumUnityUtilityLib;
using Avangardum.LifeArena.Shared;
using Avangardum.LifeArena.UnityClient.Data;
using Avangardum.LifeArena.UnityClient.Exceptions;
using Avangardum.LifeArena.UnityClient.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Avangardum.LifeArena.UnityClient.ServerCommunication
{
    public class ServerFacade : IServerFacade
    {
        private const string InternetConnectionTestUrl = "https://www.google.com";
        private const string ServerUrl = "";
        private const string GameApiRootUrl = ServerUrl + "/Api/Game";
        private const string GetGameStateUrl = GameApiRootUrl + "/GetState";
        private const string AddCellUrlTemplate = GameApiRootUrl + "/AddCell?x={0}&y={1}";
        private static readonly int TimeoutSeconds = 3;
        private const string GetMethod = "GET";
        private const string PutMethod = "PUT";
        
        private readonly ILivingCellsArrayPreserializer _livingCellsArrayPreserializer;

        public ServerFacade(ILivingCellsArrayPreserializer livingCellsArrayPreserializer)
        {
            _livingCellsArrayPreserializer = livingCellsArrayPreserializer;
        }
        
        public Task<GameState> GetGameState() => SendRequestAndReceiveGameState(GetGameStateUrl, GetMethod);

        public Task<GameState> AddCell(int x, int y) => 
            SendRequestAndReceiveGameState(string.Format(AddCellUrlTemplate, x, y), PutMethod);

        private async Task<GameState> SendRequestAndReceiveGameState(string url, string method)
        {
            using var request = await SendRequest(url, method);
            if (request.result != UnityWebRequest.Result.Success)
            {
                if (await IsConnectedToInternet())
                {
                    throw new ServerUnavailableException();
                }
                else
                {
                    throw new NoInternetConnectionException();
                }
            }

            var jsonResponse = request.downloadHandler.text;
            var jsonRoot = JObject.Parse(jsonResponse);
            var livingCells = _livingCellsArrayPreserializer
                .Depreserialize(jsonRoot["livingCells"]!.ToObject<List<string>>());
            var gameState = new GameState
            (
                LivingCells: livingCells,
                Generation: jsonRoot["generation"]!.ToObject<int>(),
                TimeUntilNextGeneration: jsonRoot["timeUntilNextGeneration"]!.ToObject<TimeSpan>(),
                NextGenerationInterval: jsonRoot["nextGenerationInterval"]!.ToObject<TimeSpan>(),
                CellsLeft: jsonRoot["cellsLeft"]!.ToObject<int>(),
                MaxCellsPerPlayerPerGeneration: jsonRoot["maxCellsPerPlayerPerGeneration"]!.ToObject<int>()
            );
            return gameState;
        }

        private async Task<UnityWebRequest> SendRequest(string url, string method)
        {
            var request = CreateRequest(url, method);
            var sendRequestOperation = request.SendWebRequest();
            await CoroutineHelper.StartCoroutine(AwaitSendRequestOperationCoroutine(sendRequestOperation)).ToTask();
            return request;
            
            IEnumerator AwaitSendRequestOperationCoroutine(UnityWebRequestAsyncOperation operation)
            {
                yield return operation;
            }
        }

        private UnityWebRequest CreateRequest(string url, string method)
        {
            var request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = TimeoutSeconds;
            return request;
        }

        private async Task<bool> IsConnectedToInternet()
        {
            using var request = await SendRequest(InternetConnectionTestUrl, GetMethod);
            return request.result == UnityWebRequest.Result.Success;
        }
    }
}