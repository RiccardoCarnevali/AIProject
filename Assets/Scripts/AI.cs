using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages;
using it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.specializations.dlv2.desktop;
public class AI {
    private static AI instance = null;
    private Handler handler;
    private InputProgram fixedProgram;
    private InputProgram variableProgram;
    private GameFlow gameFlow;
    private MakeMove nextMove;
    private bool moveMade;
    private bool moveBeingMade;
    private AI(GameFlow gameFlow) {
        this.gameFlow = gameFlow;

        handler = new DesktopHandler(new DLV2DesktopService(Directory.GetCurrentDirectory() + "/lib/dlv2"));
        ASPMapper.Instance.RegisterClass(typeof(Tile));
        ASPMapper.Instance.RegisterClass(typeof(Next));
        ASPMapper.Instance.RegisterClass(typeof(MakeMove));

        fixedProgram = new ASPInputProgram();
        fixedProgram.AddProgram(System.IO.File.ReadAllText("lib/BloomingGardenAI.txt"));
        handler.AddProgram(fixedProgram);

        Output output = handler.StartSync();

        AnswerSets answerSets = (AnswerSets) output;

        variableProgram = new ASPInputProgram();
        handler.AddProgram(variableProgram);

        nextMove = null;
        moveMade = false;
        moveBeingMade = false;
    }

    public static AI getInstance(GameFlow gameFlow) {
        if(instance == null)
            instance = new AI(gameFlow);
        return instance;
    }

    public MakeMove getNextMove() {
        return nextMove;
    }

    public void startNextMove(Tile[] tiles, Next[] nexts) {
        moveMade = false;
        if(!moveBeingMade) {
            moveBeingMade = true;
            Thread thread = new Thread(()=>calculateMove(tiles, nexts));
            thread.Start();
        } 
    }

    private void calculateMove(Tile[] tiles, Next[] nexts) {
        variableProgram.ClearAll();
        foreach(Tile tile in tiles)
            variableProgram.AddObjectInput(tile);
        foreach(Next next in nexts)
            variableProgram.AddObjectInput(next);
        Output output = handler.StartSync();
        AnswerSets answerSets = (AnswerSets) output;
        AnswerSet optimalAnswerSet = answerSets.GetOptimalAnswerSets()[0];

        foreach(object obj in optimalAnswerSet.Atoms) {
            if(typeof(MakeMove).IsInstanceOfType(obj)) {
                MakeMove move = (MakeMove) obj;
                this.nextMove = move;
                break;
            }
        }
        moveMade = true;
        moveBeingMade = false;
    }

    public bool isMoveMade() {
        return moveMade;
    }

    public void makeMove() {
        moveMade = false;
    }

    public bool isMoveBeingMade() {
        return moveBeingMade;
    }
}

[Id("tile")]
public class Tile {
    [Param(0)]
    private string flower;
    [Param(1)]
    private int x;
    [Param(2)]
    private int y;

    public Tile(string flower, int x, int y) {
        this.flower = flower;
        this.x = x;
        this.y = y;
    }

    public Tile() {

    }

    public void setFlower(string flower) {
        this.flower = flower;
    }

    public string getFlower() {
        return flower;
    }

    public void setX(int x) {
        this.x = x;
    }

    public int getX() {
        return x;
    }

    public void setY(int y) {
        this.y = y;
    }

    public int getY() {
        return y;
    }
}

[Id("next")]
public class Next {
    [Param(0)]
    private string flower;
    [Param(1)]
    private int count;

    public Next(string flower, int count) {
        this.flower = flower;
        this.count = count;
    }

    public Next() {

    }

    public void setFlower(string flower) {
        this.flower = flower;
    }

    public string getFlower() {
        return flower;
    }

    public void setCount(int count) {
        this.count = count;
    }

    public int getCount() {
        return count;
    }
}

[Id("makeMove")]
public class MakeMove {
    [Param(0)]
    private int x1;
    [Param(1)]
    private int y1;
    [Param(2)]
    private int x2;
    [Param(3)]
    private int y2;

    public MakeMove(int x1, int y1, int x2, int y2) {
        this.x1 = x1;
        this.x2 = x2;
        this.y1 = y1;
        this.y2 = y2;
    }

    public MakeMove() {

    }

    public void setX1(int x1) {
        this.x1 = x1;
    }

    public int getX1() {
        return x1;
    }

    public void setX2(int x2) {
        this.x2 = x2;
    }

    public int getX2() {
        return x2;
    }

    public void setY1(int y1) {
        this.y1 = y1;
    }

    public int getY1() {
        return y1;
    }

    public void setY2(int y2) {
        this.y2 = y2;
    }
    
    public int getY2() {
        return y2;
    }
}