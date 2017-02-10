using System;

namespace Minion.TestConsole.Test2
{
    public interface IOrder
    {
    }

    public class Order: IOrder
    {
        public int UserId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan TimeOfDay { get; set; }
        public CuisineType Cuisine { get; set; }
        public FoodType Food { get; set; }
        public int Count { get; set; }
    }
}
