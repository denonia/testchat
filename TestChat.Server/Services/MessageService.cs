using TestChat.Server.Data;
using TestChat.Server.Data.Entities;
using TestChat.Server.Models;

namespace TestChat.Server.Services;

public class MessageService : IMessageService
{
    private readonly ChatDbContext _dbContext;

    public MessageService(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveMessageAsync(string senderName, string? recipientName, string text,
        SentimentAnalysisResult? sentiment)
    {
        var message = new Message
        {
            SenderName = senderName,
            RecipientName = recipientName,
            Text = text,
        };
        _dbContext.Messages.Add(message);

        if (sentiment is not null)
        {
            _dbContext.SentimentAnalyses.Add(new SentimentAnalysis
            {
                Message = message,
                PositiveSentiment = sentiment.PositiveScore,
                NeutralSentiment = sentiment.NeutralScore,
                NegativeSentiment = sentiment.NegativeScore
            });
        }

        await _dbContext.SaveChangesAsync();
    }
}