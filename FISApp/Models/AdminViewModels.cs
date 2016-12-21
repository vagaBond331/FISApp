using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FISApp.Models
{
    public class AdminViewModels
    {
        public User logUser { get; set; }

        public List<string> empListID { get; set; }

        public List<string> empListName { get; set; }

        public List<bool[]> monthAttend { get; set; }

        public int numDays { get; set; }

        public List<Device> listDevice { get; set; }

        public List<AttendViewModel> listAttent { get; set; }

        public int[] countAttent { get; set; }

        public int[] countAbsent { get; set; }

        public string[] countDate { get; set; }

        public CircleChart circleChart { get; set; }

        public AdminViewModels()
        {
            logUser = new User();
            empListID = new List<string>();
            empListName = new List<string>();
            monthAttend = new List<bool[]>();
            numDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            listDevice = new List<Device>();
            listAttent = new List<AttendViewModel>();
            countDate = new string[10];
            circleChart = new CircleChart();
        }
    }

    public class AttendViewModel
    {
        public string user_id;
        public string fullname;
        public string location;
        public DateTime log_time;
    }

    public class CircleChart
    {
        public int attentTotal;
        public int absentTotal;
        public int notyetTotal;

        public CircleChart()
        {
            attentTotal = 0;
            absentTotal = 0;
            notyetTotal = 0;
        }
    }

    public class ExportLOGModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public Nullable<System.DateTime> startDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public Nullable<System.DateTime> endDate { get; set; }
    }
}
