using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;

namespace Reddit_MVC_App.Models
{
    public class DropdownViewModel
    {
        [Display(Name = "Choose Category")]
        public string? SelectedCategoryID { get; set; }
        public List<SubredditListData> subredditListDatas { get; set; }
    }
    
    public class SubredditListData
    {
        public string Kind { get; set; }

        public Subreddit Data { get; set; }
    }
}
