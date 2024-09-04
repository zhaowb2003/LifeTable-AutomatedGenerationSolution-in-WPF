
using System.Text.Json;
using System.Windows;
using System.Diagnostics;
/*

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

*/

namespace life_table_wpf
{
    class Life_table_method
    {
		public static int GenerateRandomSeed()
		{
			return (int)DateTime.Now.Ticks;
		}
		// randNum 为产生随机数的数目
		public int[] GenerateRandom(int minValue, int maxValue, int randNum)
		{
			Random ran = new Random(GenerateRandomSeed());
			int[] arr = new int[randNum];

			for (int i = 0; i < randNum; i++)
			{
				arr[i] = ran.Next(minValue, maxValue);
			}
			return arr;
		}

		private int CountItemsLessThanKeyNum(int[] array,int KeyNum)
		{
			int count = 0;

			// 遍历数组中的每个元素
			foreach (int item in array)
			{
				if (item <= KeyNum)
				{
					count++; // 如果元素小于3，计数器加1
				}
			}

			return count; // 返回计数结果
		}

		private int OneTurn(int _killNum, int _Sample_size_Num)
		{	
			if (_Sample_size_Num > 0) 
			{ 
			int[] arr = GenerateRandom(1, 7, _Sample_size_Num);
			int result = CountItemsLessThanKeyNum(arr, _killNum);
			return result;
			}
            else
            {
				int result = 0;
				return result;
			}
        }

		public int[] Life_table_method_fucnction(ModeType_Enum modeType_ , int _Sample_size_Num, int _killNum,string jsonString)
		{
			Debug.WriteLine($"modeType_:{modeType_.ToString()}, _Sample_size_Num:{_Sample_size_Num.ToString()}");
			// 解析JSON字符串为对象
			// var gamerules = JsonSerializer.Deserialize<Gamerules>(jsonString);

			// 输出解析结果
			// Console.WriteLine($"killNumber: {gamerules.killNumber}, Rounds: {gamerules.Rounds}");
			List<int> myList = new List<int> { };
			int roundCounter = 0;
			int counter = 0;
			int _once_num =  _Sample_size_Num;
			var gamerules = JsonSerializer.Deserialize<Gamerules>(jsonString);
			Debug.WriteLine($"killNumber: {gamerules.killNumber}, Rounds: {gamerules.Rounds}");
			var Rounds = gamerules.Rounds;
			var killNumber = gamerules.killNumber;
			Debug.WriteLine($"killNumber: {killNumber}, Rounds: {Rounds}");

			if (modeType_ == ModeType_Enum.Diagonal_line_type_Enum)
            {
				do
				{
					_once_num = OneTurn(_killNum, _once_num);
					Debug.WriteLine($"_killNum:{_killNum}, roundCounter:{roundCounter}, _once_num:{_once_num}");
					myList.Add(_once_num);
					roundCounter++;
				}
				while (_once_num > 0);
			}
			else
			{
				do
				{
					_once_num = OneTurn(killNumber[counter], _once_num);
					// if (Rounds[counter] == roundCounter && roundCounter != Rounds[Rounds.Length - 1])
					if (Rounds[counter] == roundCounter && roundCounter != Rounds[Rounds.Length - 1])
					{
						counter++;
					}
					if ( roundCounter == Rounds[Rounds.Length - 1])
					{
						MessageBox.Show("循环过长，请重新启动程序！");
						Environment.Exit(0);
					}
					
					Debug.WriteLine($"counter:{counter}, roundCounter:{roundCounter}, _once_num:{_once_num}, kill: {killNumber[counter]}");
					myList.Add(_once_num);
					roundCounter++;
				}
				while (_once_num > 0);
			}
			int[] myArray = myList.ToArray();
			return myArray;
		}
		/*  x   nx     lx     dx        qx     Lx      Tx        ex */

		#region compute_region
		public float[] Get_lx_Method(int[] nx_Life_table)
		{
			List<float> myList = new List<float> { };
			for (int i = 0; i < nx_Life_table.Length; i++)
			{
				float lx = ((float)nx_Life_table[i] / nx_Life_table[0]);
				Debug.WriteLine($"lx:{lx} , nx_Life_table[i]:{nx_Life_table[i]} , nx_Life_table[0]:{nx_Life_table[0]}");
				myList.Add(lx);
			}
			float[] myArray = myList.ToArray();
			return myArray;
		}

		public float[] Get_dx_Method(int[] nx_Life_table)
		{
			List<float> list = new List<float> { };

			for (int i = 0; i < nx_Life_table.Length - 1; i++)
			{
				// nx_Life_table[i].Dump();

				int dx = nx_Life_table[i] - nx_Life_table[i + 1];
				list.Add(dx);
			}
			list.Add(float.NaN);
			float[] dx_Life_table = list.ToArray();
			// dx_Life_table.Dump();
			return dx_Life_table;
		}

		public float[] Get_qx_Method(int[] nx_Life_table, float[] dx_Life_table)
		{
			List<float> list = new List<float> { };

			for (int i = 0; i < nx_Life_table.Length - 1; i++)
			{
				float qx = (float)dx_Life_table[i] / nx_Life_table[i];
				list.Add(qx);
			}
			list.Add(float.NaN);
			float[] qx_Life_table = list.ToArray();
			// dx_Life_table.Dump();
			return qx_Life_table;

		}

		public float[] Get_Lx_Method(int[] nx_Life_table)
		{
			List<float> list = new List<float> { };
			for (int i = 0; i < nx_Life_table.Length - 1; i++)
			{
				float Lx = (float)(nx_Life_table[i] + nx_Life_table[i + 1]) / 2;
				list.Add(Lx);
			}
			list.Add(0);
			float[] Lx_Life_table = list.ToArray();
			return Lx_Life_table;
		}

		public float[] Get_Tx_Method(float[] Lx_Life_table)
		{
			float sum = Lx_Life_table.Sum();
			List<float> list = new List<float> { };

			float removeNum = 0;
			for (int i = 0; i < Lx_Life_table.Length; i++)
			{
				float sumPart = sum - removeNum;
				removeNum += Lx_Life_table[i];
				list.Add(sumPart);
			}
			float[] Tx_Life_table = list.ToArray();
			return Tx_Life_table;
		}


		public float[] Get_ex_Method(float[] Txlist, int[] nx_Life_table)
		{
			float[] list = new float[] { };
			// 使用LINQ创建新数组exlist
			float[] exlist = Txlist.Select((value, index) => value / nx_Life_table[index]).ToArray();
			return exlist;
		}
		#endregion

	}
	public class Gamerules
	{
		public int[] killNumber { get; set; }
		public int[] Rounds { get; set; }
	}

	public class LifeTableItem
	{
		public int x { get; set; }
		public int nx { get; set; }
		public float lx { get; set; }
		public float dx { get; set; }
		public float qx { get; set; }
		public float Lx { get; set; }
		public float Tx { get; set; }
		public float ex { get; set; }

		// 构造函数
		public LifeTableItem(int x_Life_table, int nx_Life_table, float dx_Life_table, float lx_Life_table, float qx_Life_table, float Lx_Life_table, float Tx_Life_table, float ex_Life_table)
		{
			this.x = x_Life_table;
			this.nx= nx_Life_table;
			this.lx= lx_Life_table;
			this.dx = dx_Life_table;
			this.qx = qx_Life_table;
			this.Lx = Lx_Life_table;
			this.Tx = Tx_Life_table;
			this.ex= ex_Life_table;
		}
	}


}
