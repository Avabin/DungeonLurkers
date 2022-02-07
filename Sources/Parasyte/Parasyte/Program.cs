// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parasyte;
using Parasyte.Features.GameSettings;
using Parasyte.Features.GameSetup;
using Parasyte.Features.Players;
using Parasyte.Features.Voting;

var b = Host.CreateDefaultBuilder();

b.ConfigureServices(s =>
{
    s.AddSingleton<IGamePlayers, GamePlayers>();
    s.AddTransient<IVotingFactory, VotingFactory>();
    s.AddTransient<IVotingFacade, VotingFacade>();
    s.AddSingleton<IGameSettings>(new GameSettings());
});

await b.RunConsoleAppFrameworkAsync<App>(args);