﻿using System.Collections;
using System.Collections.ObjectModel;

namespace ConsoleAppSocketServer.Domain
{
    public class Game
    {
        public string Title { get; set; }
        
        public string Cover { get; set; }  // image format 
        
        public string Genre { get; set; }
        
        public string Synopsis { get; set; }
        
        public string AgeRating { get; set; }
        
        public int Stars { get; set; }

        public Collection<Qualification> CommunityQualifications { get; set; }

        public Game(string title, string cover, string genre, string synopsis, string ageRating)
        {
            Title = title;
            Cover = cover;
            Genre = genre;
            Synopsis = synopsis;
            AgeRating = ageRating;
            Stars = 0;
            CommunityQualifications = new Collection<Qualification>();
        }

        public void AddCommunityQualification(Qualification qualification)
        {
            this.CommunityQualifications.Add(qualification);
            UpdateStars();
        }

        private void UpdateStars()
        {
            GameDetails gameDetails = new GameDetails(this);
            Stars = gameDetails.AverageMark;
        }
    }
}