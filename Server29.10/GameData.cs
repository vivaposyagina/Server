using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server29._10
{
    public enum phase { waiting, game, result };
    class GameData
    {        
        DateTime timeOfEndingPhaseWaiting;
        DateTime timeOfEndingPhaseGame;
        DateTime timeOfEndingPhaseResult;
        public phase phaseOfGame;
        List<string> players;
        List<PlayerList.Player> playersAndColors;
        List<VisiblePlayers.Player> playersAndCoords;
        List<VisibleObjects.MapObject> mapObjects;
        private Random rand;
        private int[,] labyrinth;
        private int sizeH, sizeW;        
        System.IO.StreamReader read;
        List<string> namesOfFileLabyrinths;
        public GameData()
        {
            namesOfFileLabyrinths = new List<string>();
            namesOfFileLabyrinths.Add("labyrinth1.txt");            
            players = new List<string>();
            playersAndColors = new List<PlayerList.Player>();
            playersAndCoords = new List<VisiblePlayers.Player>();
            mapObjects = new List<VisibleObjects.MapObject>();
            rand = new Random();            
            timeOfEndingPhaseWaiting = DateTime.Now.AddSeconds(30);
            timeOfEndingPhaseGame = DateTime.Now.AddSeconds(50);
            timeOfEndingPhaseResult = DateTime.Now.AddSeconds(60);
            ReadLabyrinth("labyrinth1.txt");
            phaseOfGame = phase.waiting;

        }
        public void PlayerMoved(direction movement, string name)
        {
            int index = 0;
            for (int i = 0; i < players.Count; i++)
            {
                if (name == players[i])
                {
                    index = i;
                }
            }            
            //Надо ли отправлять клиенту сообщение о том, что движение невозможно??
            if (movement == direction.S && labyrinth[playersAndCoords[index].Col, playersAndCoords[index].Row + 1] == 0)
            {
                labyrinth[playersAndCoords[index].Col, playersAndCoords[index].Row] = 0;
                labyrinth[playersAndCoords[index].Col, playersAndCoords[index].Row + 1] = 2;
                playersAndCoords[index].Row += 1;
            }
            if (movement == direction.N && labyrinth[playersAndCoords[index].Col, playersAndCoords[index].Row - 1] == 0)
            {
                labyrinth[playersAndCoords[index].Col, playersAndCoords[index].Row] = 0;
                labyrinth[playersAndCoords[index].Col, playersAndCoords[index].Row - 1] = 2;
                playersAndCoords[index].Row -= 1;
            }
            if (movement == direction.W && labyrinth[playersAndCoords[index].Col - 1, playersAndCoords[index].Row] == 0)
            {
                labyrinth[playersAndCoords[index].Col, playersAndCoords[index].Row] = 0;
                labyrinth[playersAndCoords[index].Col - 1, playersAndCoords[index].Row] = 2;
                playersAndCoords[index].Col -= 1;
            }
            if (movement == direction.E && labyrinth[playersAndCoords[index].Col + 1, playersAndCoords[index].Row] == 0)
            {
                labyrinth[playersAndCoords[index].Col, playersAndCoords[index].Row] = 0;
                labyrinth[playersAndCoords[index].Col + 1, playersAndCoords[index].Row] = 2;
                playersAndCoords[index].Col += 1;
            }
            
        }
        public DateTime TimeOfEndingThisWaiting
        {
            get { return timeOfEndingPhaseWaiting; }
            set { timeOfEndingPhaseWaiting = value; }
        }
        public DateTime TimeOfEndingPhaseGame
        {
            get { return timeOfEndingPhaseGame; }
            set { timeOfEndingPhaseGame = value; }
        }
        public DateTime TimeOfEndingPhaseResult
        {
            get { return timeOfEndingPhaseResult; }
            set { timeOfEndingPhaseResult = value; }
        }
        
        public PlayerList FormCommandOfPlayersList()
        {
            return new PlayerList(playersAndColors);
        }
        public TimeLeft FormCommandOfTimeLeft()
        {
            if (phaseOfGame == phase.waiting)
                return new TimeLeft(timeOfEndingPhaseWaiting - DateTime.Now);
            else if (phaseOfGame == phase.game)
                return new TimeLeft(timeOfEndingPhaseGame - DateTime.Now);
            else 
                return new TimeLeft(timeOfEndingPhaseResult - DateTime.Now);
        }
        public PlayerCoords FormCommandOfPlayerCoords(string name)
        {
            for (int i = 0; i < playersAndColors.Count; i++)
            {
                if (name == playersAndColors[i].Name)
                {
                    return new PlayerCoords(playersAndCoords[i].Col, playersAndCoords[i].Row);
                }
            }
            return null;
        }
        public void ReadLabyrinth(string newString)
        {
            read = new System.IO.StreamReader(namesOfFileLabyrinths[0]);
            sizeH = Convert.ToInt32(read.ReadLine());
            sizeW = Convert.ToInt32(read.ReadLine());
            labyrinth = new int[sizeH, sizeW];
            for (int i = 0; i < sizeH; i++)
            {
                newString = read.ReadLine();
                for(int j = 0; j < sizeW; j++)
                {
                    labyrinth[i, j] = Convert.ToInt32(newString[j]) - Convert.ToInt32('0');
                    if (labyrinth[i, j] == 1)
                    {
                        mapObjects.Add(new VisibleObjects.MapObject(types.WALL, i, j));
                    } 
                }
            }            
        }
        public Tuple<int, int> GetSizeMaps()
        {
            return new Tuple<int, int>(sizeH, sizeW);
        }
        public MapSize FormCommandOfMapSize()
        {            
            return new MapSize(GetSizeMaps().Item1, GetSizeMaps().Item2);
        }
        public VisiblePlayers FormCommandOfVisiblePlayers(string name)
        {
            List<VisiblePlayers.Player> list = new List<VisiblePlayers.Player>();
            for (int i = 0; i < playersAndColors.Count; i++)
            {
                if (name == playersAndColors[i].Name)
                {
                    for (int j = 0; j < playersAndCoords.Count; j++)
                    {
                        if ((playersAndCoords[i].Col + 5 > playersAndCoords[j].Col) && 
                            (playersAndCoords[i].Col - 5 < playersAndCoords[j].Col) && 
                            (playersAndCoords[i].Row + 5 > playersAndCoords[j].Row) &&
                            (playersAndCoords[i].Row - 5 < playersAndCoords[j].Row) && 
                            (i != j))
                        {
                            list.Add(playersAndCoords[j]);
                        }                        
                    }
                    return new VisiblePlayers(list);                    
                }
            }
            return null;      
        }
        public VisibleObjects FormCommandOfVisibleObjects(string name)
        {
            List<VisibleObjects.MapObject> list = new List<VisibleObjects.MapObject>();
            for (int i = 0; i < playersAndColors.Count; i++)
            {
                if (name == playersAndColors[i].Name)
                {
                    for (int j = 0; j < mapObjects.Count; j++)
                    {
                        if ((mapObjects[j].Col < (playersAndCoords[i].Col + 5)) &&
                            (mapObjects[j].Col > (playersAndCoords[i].Col - 5)) &&
                            (mapObjects[j].Row < (playersAndCoords[i].Row + 5)) &&
                            (mapObjects[j].Row > (playersAndCoords[i].Row - 5)))
                        {
                            list.Add(mapObjects[j]);
                        }
                    }
                    return new VisibleObjects(list);
                }
            }
            return null;   
        }
        public GameOver FormCommandOfGameOver()
        {
            return new GameOver(-1);
        }

        public void StartGame()
        {
            phaseOfGame = phase.game;            
        }
        public void FinishGame()
        {
            phaseOfGame = phase.result;
        }    

        public void AddNewPlayer(string name)
        {
            players.Add(name);
            playersAndColors.Add(new PlayerList.Player(name, Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
            int coordRow = 0, coordCol = 0;
            while (labyrinth[coordCol, coordRow] == 1)
            {
                coordCol = rand.Next(0, GetSizeMaps().Item1);
                coordRow = rand.Next(0, GetSizeMaps().Item2);
                for (int i = 0; i < playersAndCoords.Count; i++)
                {
                    if (playersAndCoords[i].Col == coordCol && playersAndCoords[i].Row == coordRow)
                    {
                        coordRow = 0;
                        coordCol = 0;
                    }
                }
            }
            labyrinth[coordCol, coordRow] = 2;
            playersAndCoords.Add(new VisiblePlayers.Player(name, coordCol, coordRow));            
        }
        public void DeletePlayer(string name)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == name)
                {
                    players.RemoveAt(i);
                    playersAndColors.RemoveAt(i);
                    playersAndCoords.RemoveAt(i);

                }
            }
        }
    }
}
