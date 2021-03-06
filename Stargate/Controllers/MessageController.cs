﻿using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Stargate;
using Aiursoft.Pylon.Models.Stargate.MessageAddressModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Stargate.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class MessageController : Controller
    {
        private readonly StargateDbContext _dbContext;
        private StargateMemory _memoryContext;
        private readonly Counter _counter;
        private readonly ACTokenManager _tokenManager;

        public MessageController(
            StargateDbContext dbContext,
            StargateMemory memoryContext,
            Counter counter,
            ACTokenManager tokenManager)
        {
            _dbContext = dbContext;
            _memoryContext = memoryContext;
            _counter = counter;
            _tokenManager = tokenManager;
        }

        [HttpPost]
        public async Task<IActionResult> PushMessage(PushMessageAddressModel model)
        {
            //Ensure app
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            //Ensure channel
            var channel = await _dbContext.Channels.SingleOrDefaultAsync(t => t.Id == model.ChannelId && t.AppId == appid);
            if (channel == null)
            {
                return Json(new AiurProtocol
                {
                    Code = ErrorType.NotFound,
                    Message = "We can not find your channel!"
                });
            }
            //Create Message
            var message = new Message
            {
                Id = _counter.GetUniqueNo,
                ChannelId = channel.Id,
                Content = model.MessageContent
            };
            _memoryContext.Messages.Add(message);
            return Json(new AiurProtocol
            {
                Code = ErrorType.Success,
                Message = $"You have successfully pushed a new message to channel: {channel.Id}!"
            });
        }
    }
}
