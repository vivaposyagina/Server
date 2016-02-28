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
        //List<List<VisibleObjects.MapObject>> mapObjects;
        Random rand;
        public GameData()
        {
            phaseOfGame = phase.waiting;
            players = new List<string>();
            playersAndColors = new List<PlayerList.Player>();
            playersAndCoords = new List<VisiblePlayers.Player>();
            //mapObjects = new List<List<VisibleObjects.MapObject>>();
            rand = new Random();
            timeOfEndingPhaseWaiting = DateTime.Now.AddSeconds(30);
            timeOfEndingPhaseGame = DateTime.Now.AddSeconds(50);
            timeOfEndingPhaseResult = DateTime.Now.AddSeconds(60);  
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
            if (movement == direction.E)
            {
                playersAndCoords[index].Col += 1;
            }
            if (movement == direction.W)
            {
                playersAndCoords[index].Col -= 1;
            }
            if (movement == direction.S)
            {
                playersAndCoords[index].Row -= 1;
            }
            if (movement == direction.N)
            {
                playersAndCoords[index].Row += 1;
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
                    return new PlayerCoords(playersAndCoords[i].Row, playersAndCoords[i].Col);
                }
            }
            return null;
        }
        public MapSize FormCommandOfMapSize()
        {
            return new MapSize(100, 100);
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
                        if ((playersAndCoords[i].Col + 5 < playersAndCoords[j].Col) && 
                            (playersAndCoords[i].Col - 5 < playersAndCoords[j].Col) && 
                            (playersAndCoords[i].Row + 5 < playersAndCoords[j].Row) &&
                            (playersAndCoords[i].Row - 5 < playersAndCoords[j].Row) && 
                            (i != j))
                        {
                            list.Add(playersAndCoords[j]);
                        }                        
                    }
                    return new VisiblePlayers(playersAndCoords);                    
                }
            }
            return null;      
        }
        public VisibleObjects FormCommandOfVisibleObjects(int index)
        {
            return new VisibleObjects();
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

        public void CreateMap()
        { 

        }

        public void AddNewPlayer(string name)
        {
            players.Add(name);
            playersAndColors.Add(new PlayerList.Player(name, Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
            playersAndCoords.Add(new VisiblePlayers.Player(name, rand.Next(0, 100), rand.Next(0, 100)));
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
