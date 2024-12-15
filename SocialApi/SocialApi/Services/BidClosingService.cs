using Microsoft.EntityFrameworkCore;
using SocialApi.Data;
using SocialApi.Models.Domain;

namespace SocialApi.Services // Add this namespace declaration
{
    public class BidClosingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BidClosingService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<WebSocialDbContext>();

                    // Fetch all expired Bids
                    var expiredBids = await dbContext.Bids
                        .Where(b => b.IsOpen && b.EndTime <= DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    foreach (var bid in expiredBids)
                    {
                        // Find the associated OpenBid
                        var openBid = await dbContext.OpenBids
                            .FirstOrDefaultAsync(ob => ob.BidId == bid.BidId, stoppingToken);

                        if (openBid != null)
                        {
                            // Move OpenBid data to CloseBids
                            var closedBid = new CloseBids
                            {
                                BidId = openBid.BidId,
                                NftId = openBid.NftId,
                                HighestBidder = openBid.HighestBidder,
                                Amount = openBid.Amount,
                                CloseTime = DateTime.UtcNow
                            };

                            await dbContext.CloseBids.AddAsync(closedBid, stoppingToken);

                            // Remove the OpenBid
                            dbContext.OpenBids.Remove(openBid);
                        }

                        // Mark the Bid as closed
                        bid.IsOpen = false;
                        dbContext.Bids.Update(bid);
                    }

                    // Save all changes to the database
                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                // Wait before running again (e.g., every minute)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}