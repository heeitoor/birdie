using System;
using System.Threading.Tasks;
using Birdie.Data.Entities;
using Birdie.Data.Repositories;
using Birdie.Service.Models;

namespace Birdie.Service.Business
{
    public interface IMessageBusiness
    {
        Task Add(MessageCreateModel model);
    }

    public class MessageBusiness : IMessageBusiness
    {
        private readonly IMessageRepository messageRepository;

        public MessageBusiness(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public async Task Add(MessageCreateModel model)
        {
            await messageRepository.Add(new Message
            {
                Content = model.Content,
                RoomId = model.RoomId,
                UserId = model.UserId,
                UpdatedAt = DateTimeOffset.UtcNow
            });
        }
    }
}