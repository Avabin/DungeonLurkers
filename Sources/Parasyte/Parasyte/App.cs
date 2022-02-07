using Parasyte.Features.GameSetup;
using Parasyte.Features.Players;
using Parasyte.Features.Voting;

namespace Parasyte;

public class App : ConsoleAppBase
{
    private readonly IVotingFacade _votingFacade;
    private readonly IGamePlayers  _players;
    private          IPlayer?      CurrentUserPlayer;

    public App(IVotingFacade votingFacade, IGamePlayers players)
    {
        _votingFacade = votingFacade;
        _players      = players;
    }

    public void Run()
    {
        Console.WriteLine("Welcome to Parasyte!");
        ReadCurrentUserPlayer();

        Console.WriteLine("Welcome, {0}!", CurrentUserPlayer.Name);

        char command;

        do
        {
            ShowHelp();
            command = Console.ReadKey().KeyChar;
            Console.WriteLine();

            switch (command)
            {
                case 'h':
                    ShowHelp();
                    break;
                case 'a':
                    ReadPlayer();
                    break;
                case 'v':
                    Vote();
                    break;
                case 's':
                    StartVote();
                    break;
                case 'r':
                    FinishVoteAndDisplayResults();
                    break;
            }
        } while (command is not 'q');

        Console.WriteLine("Bye!");
    }

    private void FinishVoteAndDisplayResults()
    {
        Console.WriteLine("Vote completed!");
        var result = _votingFacade.FinishVote();
        Console.WriteLine("Result: {0}", result);
    }

    private void StartVote()
    {
        Console.WriteLine("Starting voting...");
        _votingFacade.StartVote();
    }

    private void Vote()
    {
        Console.WriteLine("Who do you want to vote for?");
        var target = ReadVoteTarget();
        _votingFacade.AddVote(CurrentUserPlayer, target);
        Console.WriteLine("Voted for {0}!", target.Name);
    }

    private IPlayer ReadVoteTarget()
    {
        var targetsNames = _players.Players
                                   .Where(x => x != CurrentUserPlayer)
                                   .Select((x, i) => (x.Name, index: i))
                                   .ToList();
        var message = string.Join(Environment.NewLine, targetsNames.Select(x => $"{x.index} - {x.Name}"));
        Console.WriteLine(message);
        var targetIndex = ReadVoteTargetIndex(targetsNames, message);

        var target = _players.Players.ElementAt(targetIndex);
        return target;
    }

    private static int ReadVoteTargetIndex(List<(string Name, int index)> targetsNames, string message)
    {
        var maybeIndexAsString = Console.ReadLine();

        int targetIndex;

        while (!(int.TryParse(maybeIndexAsString, out targetIndex)
              && targetIndex >= 0 &&
                 targetIndex <= targetsNames.Count - 1))
        {
            Console.WriteLine("Invalid index");
            Console.WriteLine(message);
            maybeIndexAsString = Console.ReadLine();
        }

        return targetIndex;
    }

    private void ReadCurrentUserPlayer()
    {
        Console.WriteLine("Enter your name:");
        var name = Console.ReadLine();
        CurrentUserPlayer = new Player(name ?? ":(", PlayerRole.Human, PlayerState.Alive);
        _players.AddPlayer(CurrentUserPlayer);
    }

    private void ReadPlayer()
    {
        var name = ReadPlayerName();
        Console.WriteLine("Select player role:");
        ShowPlayerRoles();
        var role = ReadPlayerRole();


        _players.AddPlayer(new Player(name, role, PlayerState.Alive));
        Console.WriteLine("Added player {0} with role {1:G}", name, role);
    }

    private static string ReadPlayerName()
    {
        string? name;

        do
        {
            Console.WriteLine("Enter player name:");
            name = Console.ReadLine();

            if (name is null or "")
            {
                Console.WriteLine("Player name cannot be empty");
            }
            else
            {
                break;
            }
        } while (true);

        return name;
    }

    private static PlayerRole ReadPlayerRole()
    {
        var roleKey = Console.ReadKey();
        Console.WriteLine();
        var role = roleKey.KeyChar switch
        {
            '1' => PlayerRole.Human,
            '2' => PlayerRole.Doctor,
            '3' => PlayerRole.Engineer,
            '4' => PlayerRole.Guardian,
            '5' => PlayerRole.Parasite,
            '6' => PlayerRole.Liar,
            _   => throw new ArgumentOutOfRangeException(nameof(roleKey),"Unknown role!")
        };
        return role;
    }

    private static void ShowPlayerRoles()
    {
        Console.WriteLine("1 - Human");
        Console.WriteLine("2 - Doctor");
        Console.WriteLine("3 - Engineer");
        Console.WriteLine("4 - Guardian");
        Console.WriteLine("5 - Parasite");
        Console.WriteLine("6 - Liar");
    }

    private static void ShowHelp()
    {
        Console.WriteLine(">h - Show this help");
        Console.WriteLine(">a - Add player");
        Console.WriteLine(">s - Start vote");
        Console.WriteLine(">r - Show vote results");
        Console.WriteLine(">v - Vote");
        Console.WriteLine(">q - Quit");
    }
}