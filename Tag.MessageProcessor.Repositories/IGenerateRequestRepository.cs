using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

internal interface IGenerateRequestRepository
{
    Task SendRequest(GenerateRequestEntity entity);
}
