using System;
using System.Collections.Generic;
using RedUtils;
using rlbot.flat;
using RedUtils.StateSetting;
namespace Test;

public class TestSuite
{
    private string logFilePath = @"C:\Users\romai\Documents\RL_ML\HARCODED\Test\log.txt";
    // 'true' pour ajouter à la fin du fichier
    private List<TestUnit<PossessionStatus>> _testsHasPossession = new List<TestUnit<PossessionStatus>>();
    string testName;
    private static string frameFolder = @"C:\Users\romai\Documents\RL_ML\HARCODED\RedUtils\StateSetting\Replay";

    private Logger _logger;

    public TestSuite(string testName)
    {   
        _logger = new Logger(logFilePath);
        this.testName = testName;
        StateEntity state = new StateEntity(frameFolder, 350);
        
        Ball.Update2(state.Ball.location, state.Ball.velocity);
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.MyPossession, state, 0, _logger));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.TeammatePossession, state, 1, _logger));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.TeammatePossession, state, 2, _logger));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.OpponentPossessionButClosest, state, 3, _logger));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.OpponentPossession, state, 4, _logger));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.OpponentPossession, state, 5, _logger));
        
    }

    // Méthode pour exécuter le test
    public void Run()
    {
        try
        {
            foreach (var test in _testsHasPossession)
        {
            if (!test.Run())
            {
                _logger.Log("Test failed"+testName);
            }
            _logger.StopLogging();
            Console.WriteLine("Test passed");
        }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error catched: " + e.Message);
            _logger.StopLogging();
        }
        
        
    }
}
