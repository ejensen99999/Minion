using Minion.DataControl;
using System;

namespace Minion.TestConsole.Test2
{
	public class FoodCube : Indice<int, User>
	{
	}

	public class User : Indice<DayOfWeek, WeekDay> 
	{
	}

	public class WeekDay : Indice<TimeSpan, TimeOfDay>
	{
	}

	public class TimeOfDay : Indice<CuisineType, Cuisine>
	{
	}

	public class Cuisine : Indice<FoodType, Food>
	{
	}

	public class Food : IndiceCounter<Order>
	{
	}
}
