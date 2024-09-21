
using System;
using System.Collections.Generic;
using RedUtils;
using RedUtils.StateSetting;
using System.IO;


namespace Test;
class TestUnit<TResult> {
    Func<FieldParameters, TResult> _testFunction; // Fonction générique à tester
    private readonly TResult _expectedResult;         // Résultat attendu

    private Car myCar;
    private List<Car> teamMates;
    private List<Car> opponents;
    private Ball ball;
    private Logger _logger;

    

    public TestUnit(Func<FieldParameters, TResult> testFunction, TResult expectedResult, StateEntity state, int playerNumber, Logger logger)
    {
        _testFunction = testFunction;
        _expectedResult = expectedResult;
        _logger = logger;
        
        if (playerNumber < state.blueTeam.Count)
        {
            myCar = state.orangeTeam[playerNumber];
            teamMates = state.orangeTeam;
            opponents = state.blueTeam;
        }
        else
        {
            myCar = state.blueTeam[playerNumber - state.blueTeam.Count];
            teamMates = state.blueTeam;
            opponents = state.orangeTeam;
        }
        ball = state.Ball;
        //pas vraiment besoin Cars.StateSet(StateEntity state, frame);
    }
    public bool Run()
    {
        var parameters = new FieldParameters
            {
                MyCar = myCar,
                TeamMates = teamMates,
                Opponents = opponents,
                Ball = ball
            };
            var result = _testFunction(parameters);
            _logger.Log("Test result"+result+ myCar.Index);

        // Comparaison du résultat avec la valeur attendue
        return System.Collections.Generic.EqualityComparer<TResult>.Default.Equals(result, _expectedResult);
    }
}

