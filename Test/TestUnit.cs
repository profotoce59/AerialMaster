
using System;
using RedUtils;
using RedUtils.StateSetting;


namespace Test;
class TestUnit<TResult> {
    Func<FieldParameters, TResult> _testFunction; // Fonction générique à tester
    private readonly TResult _expectedResult;         // Résultat attendu
    private int frame; // Optionnel : action à exécuter après le test

    private Car myCar;
    private Car[] teamMates;
    private Car[] opponents;
    private Ball ball;

    

    public TestUnit(Func<FieldParameters, TResult> testFunction, TResult expectedResult, StateEntity state, int playerNumber)
    {
        _testFunction = testFunction;
        _expectedResult = expectedResult;
        
        if (playerNumber < state.blueTeam.Length)
        {
            myCar = state.blueTeam[playerNumber];
            teamMates = state.blueTeam;
            opponents = state.orangeTeam;
        }
        else
        {
            myCar = state.orangeTeam[playerNumber - state.blueTeam.Length];
            teamMates = state.orangeTeam;
            opponents = state.blueTeam;
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

        // Comparaison du résultat avec la valeur attendue
        return System.Collections.Generic.EqualityComparer<TResult>.Default.Equals(result, _expectedResult);
    }
}

