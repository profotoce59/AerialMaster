using System;
using System.Collections.Generic;
using RedUtils;
using rlbot.flat;
using RedUtils.StateSetting;
namespace Test;

public class TestSuite
{
    private List<TestUnit<PossessionStatus>> _testsHasPossession = new List<TestUnit<PossessionStatus>>();
    string testName;
    private static string frameFolder = @"C:\Users\romai\Documents\RL_ML\HARCODED\RedUtils\StateSetting\Replay";

    public TestSuite(string testName)
    {   
        this.testName = testName;
        StateEntity state = new StateEntity(frameFolder, 350);
        Ball.Update(state.Ball.location, state.Ball.velocity, state.Ball.angularVelocity);
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.MyPossession, state, 0));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.TeammatePossession, state, 1));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.TeammatePossession, state, 2));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.OpponentPossessionButClosest, state, 3));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.OpponentPossession, state, 4));
        _testsHasPossession.Add(new TestUnit<PossessionStatus>(Possession.HasPossession, PossessionStatus.OpponentPossession, state, 5));
        
    }

    // Méthode pour exécuter le test
    public void Run()
    {
        foreach (var test in _testsHasPossession)
        {
            if (!test.Run())
            {
                Console.WriteLine("Test failed"+testName);
            }
        }
    }
}
