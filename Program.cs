using System;
using System.Threading;

public class BlackJackGame
{
private static deckCard deck;
public void Play()
{
    bool continuePlay = true;

    Console.Title = "Steve BlackJack Game (Version 2)";
    Console.Write("Steve BlackJack Game ");
    Utility.MakeColor2(" ♠ ",ConsoleColor.White);
    Utility.MakeColor2(" ♥ ",ConsoleColor.Red);
    Utility.MakeColor2(" ♣ ",ConsoleColor.White);
    Utility.MakeColor2(" ♦ ",ConsoleColor.Red);

    deck = new deckCard();
    Console.Write("\n\nEnter player's name: ");

    // Create player
    var player = new Player(Console.ReadLine());

    // Create dealer
    var dealerComputer = new Player();

    while (continuePlay)
    {
        // Initialize screen and player's certain property - Start
        Console.Clear();
        player.IsNaturalBlackJack = false;
        player.IsBusted = false;
        dealerComputer.IsNaturalBlackJack = false;
        dealerComputer.IsBusted = false;
        // Initialize screen and player's certain property - End            

        if (deck.GetRemainingDeckCount() < 20)
        {
            // Get a new shuffled deck.
            deck.Initialize();
            Console.WriteLine("Low number of cards remaining. New cold deck created.");
        }

        deck.ShowRemainingDeckCount();

        // Show player bank roll
        Console.WriteLine($"{player.Name} Chips Balance: {player.ChipsOnHand}");

        // Get bet amount from player
        Console.Write("Enter chip bet amount: ");
        player.ChipsOnBet = Convert.ToInt16(Console.ReadLine());

        // Deal first two cards to player
        deck.DealHand(player);

        // Show player's hand
        player.ShowUpCard();
        Thread.Sleep(1500);

        // Deal first two cards to dealer
        deck.DealHand(dealerComputer);

        // Show dealer's hand        
        dealerComputer.ShowUpCard(true);
        Thread.Sleep(1500);
        // Check natural black jack
        if (!checkNaturalBlack(player, dealerComputer))
        {
            // If both also don't have natural black jack, 
            // then player's turn to continue.
            PlayerAction(player);

            Console.WriteLine("\n--------------------------------------------------");

            PlayerAction(dealerComputer);

            Console.WriteLine("\n--------------------------------------------------");

            //Announce the winner.
            AnnounceWinner(player, dealerComputer);
        }

        Console.WriteLine("This round is over.");

        Console.Write("\nPlay again? Y or N? ");

        continuePlay = Console.ReadLine() == "Y" ? true : false;
        // for brevity, no input validation
    }

    Console.WriteLine($"{player.Name} won {player.TotalWins} times.");
    Console.WriteLine($"{dealerComputer.Name} won {dealerComputer.TotalWins} times.");
    Console.WriteLine("Game over. Thank you for playing.");

}

private static void PlayerAction(Player currentPlayer)
{
    // set to player's turn

    bool playerTurnContinue = true;

    string opt = "";

    while (playerTurnContinue)
    {
        Console.Write($"\n{currentPlayer.Name}'s turn. ");

        if (currentPlayer.Name.Equals("Dealer"))
        {
            Thread.Sleep(2000); // faking thinking time.
            // Mini A.I for dealer.
            opt = currentPlayer.GetHandValue() < 16 ? "H" : "S";
        }
        else
        {
            // Prompt player to enter Hit or Stand.
            Console.Write("Hit (H) or Stand (S): ");
            opt = Console.ReadLine();
        }

        switch (opt.ToUpper())
        {
            case "H":
                Console.Write($"{currentPlayer.Name} hits. ");
                Thread.Sleep(1500);
                // Take a card from the deck and put into player's Hand.
                currentPlayer.Hand.Add(deck.DrawCard());
                Thread.Sleep(1500);

                // Check if there is any Ace in the Hand. If yes, change all the Ace's value to 1.
                if (currentPlayer.GetHandValue() > 21 && currentPlayer.CheckAceInHand())
                    currentPlayer.Hand = currentPlayer.ChangeAceValueInHand();

                currentPlayer.ShowHandValue();

                break;
            case "S":
                if (currentPlayer.GetHandValue() < 16)
                    Console.WriteLine($"{currentPlayer.Name} is not allowed to stands when hand value is less than 16.");
                else
                {
                    Console.WriteLine($"{currentPlayer.Name} stands.");
                    Thread.Sleep(1500);
                    // Show player's hand
                    currentPlayer.ShowUpCard();
                    Thread.Sleep(1500);
                    Console.WriteLine($"{currentPlayer.Name}'s turn is over.");
                    Thread.Sleep(1500);
                    playerTurnContinue = false;
                }

                break;
            default:
                Console.WriteLine("Invalid command.");
                break;
        }

        // If current player is busted, turn is over.
        if (currentPlayer.GetHandValue() > 21)
        {
            Utility.MakeColor("Busted!", ConsoleColor.Red);
            Thread.Sleep(1500);
            Console.WriteLine($"{currentPlayer.Name}'s turn is over.");
            Thread.Sleep(1500);
            currentPlayer.IsBusted = true;
            playerTurnContinue = false;
        }
        // If current player total card in hand is 5, turn is over.
        else if (currentPlayer.Hand.Count == 5)
        {
            Console.WriteLine($"{currentPlayer.Name} got 5 cards in hand already.");
            Thread.Sleep(1500);
            Console.WriteLine($"{currentPlayer.Name}'s turn is over.");
            Thread.Sleep(1500);
            playerTurnContinue = false;
        }


    }
}



private static bool checkNaturalBlack(Player _player, Player _dealer)
{
    Console.WriteLine();
    if (_dealer.IsNaturalBlackJack && _player.IsNaturalBlackJack)
    {
        Console.WriteLine("Player and Dealer got natural BlackJack. Tie Game!");
        _dealer.ShowUpCard();

        return true;
    }
    else if (_dealer.IsNaturalBlackJack && !_player.IsNaturalBlackJack)
    {
        Console.WriteLine($"{_dealer.Name} got natural BlackJack. {_dealer.Name} won!");
        _dealer.ShowUpCard();
        _dealer.AddWinCount();
        _player.ChipsOnHand = _player.ChipsOnHand - (int)Math.Floor(_player.ChipsOnBet * 1.5);            
        return true;
    }
    else if (!_dealer.IsNaturalBlackJack && _player.IsNaturalBlackJack)
    {
        Console.WriteLine($"{_player.Name} got natural BlackJack. {_player.Name} won!");
        _player.AddWinCount();
        _player.ChipsOnHand = _player.ChipsOnHand + (int)Math.Floor(_player.ChipsOnBet * 1.5);
        return true;
    }

    // guard block
    return false;
}

private static void AnnounceWinner(Player _player, Player _dealer)
{
    Console.WriteLine();
    if (!_dealer.IsBusted && _player.IsBusted)
    {
        Console.WriteLine($"{_dealer.Name} won.");
        _dealer.AddWinCount();
    }
    else if (_dealer.IsBusted && !_player.IsBusted)
    {
        Console.WriteLine($"{_player.Name} won.");
        _player.AddWinCount();
        _player.ChipsOnHand = _player.ChipsOnHand + _player.ChipsOnBet;
    }
    else if (_dealer.IsBusted && _player.IsBusted)
        Console.WriteLine("Tie game.");
    else if (!_dealer.IsBusted && !_player.IsBusted)
        if (_player.GetHandValue() > _dealer.GetHandValue())
        {
            Console.WriteLine($"{_player.Name} won.");
            _player.AddWinCount();
            _player.ChipsOnHand = _player.ChipsOnHand + _player.ChipsOnBet;
        }
        else if (_player.GetHandValue() < _dealer.GetHandValue())
        {
            Console.WriteLine($"{_dealer.Name} won.");
            _dealer.AddWinCount();
            _player.ChipsOnHand = _player.ChipsOnHand - _player.ChipsOnBet;                
        }

        else if (_player.GetHandValue() == _dealer.GetHandValue())
            Console.WriteLine("Tie game.");


}
