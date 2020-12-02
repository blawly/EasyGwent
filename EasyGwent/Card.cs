using System;

namespace EasyGwent
{
    enum Type
    {
        Field,
        Weather
    }
    enum SubType
    {
        Infantry,
        Range,
        Machine,
        Freeze,
        Fog,
        Rain,
        Sun
    }
    abstract class Card
    {
        public Type Type { get; protected set; }
        public SubType SubType { get; protected set; }
        public abstract bool Playable(Player playerPlayer, Player otherPlayer);
        public abstract void Play(ref Player playerPlayer, ref Player otherPlayer);
        public abstract override string ToString();
    }
    class WeatherCard : Card
    {
        public WeatherCard(Random rnd)
        {
            Type = (Type)1;
            SubType = (SubType)rnd.Next(3, 7);
        }
        public override void Play(ref Player playerPlayer, ref Player otherPlayer)
        {
            if (SubType == (SubType)6)
            {
                for (int i = 0; i < playerPlayer.Played.Count; i++)
                {
                    playerPlayer.Played[i].Weak = false;
                }
                playerPlayer.UpdateScore();
            }
            else
            {
                for (int i = 0; i < otherPlayer.Played.Count; i++)
                {
                    if (otherPlayer.Played[i].SubType == SubType - 3)
                    {
                        otherPlayer.Played[i].Weak = true;
                    }
                }
                otherPlayer.UpdateScore();
            }
            playerPlayer.Hand.Remove(this);
        }
        public override bool Playable(Player playerPlayer, Player otherPlayer)
        {
            return true;
        }
        public override string ToString()
        {
            string display = $"{SubType}";            
            return display;
        }
    }
    class FieldCard : Card
    {
        int _value;
        public int Value
        {
            get => (Weak ? 1 : _value);
            private set => _value = value;
        }
        public bool Weak { get; set; }
        public FieldCard(Random rnd)
        {
            Type = (Type)0;
            Value = rnd.Next(11);
            SubType = (SubType)rnd.Next(0, 3);
            Weak = false;
        }
        public override void Play(ref Player playerPlayer, ref Player otherPlayer)
        {
            playerPlayer.Score += Value;
            playerPlayer.Hand.Remove(this);
            playerPlayer.Played.Add(this);
        }
        public override bool Playable(Player playerPlayer, Player otherPlayer)
        {
            int count = 0;
            if (playerPlayer.Played != null)
            {
                for (int i = 0; i < playerPlayer.Played.Count; i++)
                {
                    if (playerPlayer.Played[i].SubType == this.SubType) count++;
                }
            }
            if (otherPlayer.Played != null)
            {
                for (int i = 0; i < otherPlayer.Played.Count; i++)
                {
                    if (otherPlayer.Played[i].SubType == this.SubType) count++;
                }
            }
            if (count >= 5) return false;
            else return true;
        }
        public override string ToString()
        {
            string display = "";
            if (Weak)
            {
                display += "Weakened ";
            }
            display += $"{SubType} ";
            display += $"({Value})";
            return display;
        }
    }    
}
