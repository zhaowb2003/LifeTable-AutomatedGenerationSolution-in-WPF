using System.Diagnostics;
using System.Windows;
using System.Text.Json;
using Microsoft.Win32;
using System.IO;

/*
using System.Text;
using OxyPlot.Wpf;
using OxyPlot;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Linq;
using Microsoft.Data.Analysis;
*/
namespace life_table_wpf;

enum ModeType_Enum
{
	Concave_line_type_Enum, //凹线型枚举
	Diagonal_line_type_Enum,
	Convex_line_type_Enum
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	List<LifeTableItem> DataList = new List<LifeTableItem> { };
	List<float> log10List = new List<float> { };

	ModeType_Enum modelType = new ModeType_Enum();
	public MainWindow()
    {
        InitializeComponent();
		tbMultiLine.Text = "{\n\"killNumber\":[2,3,4],\n\"Rounds\":[2,4,30]\n}";
		Slider1.IsEnabled = false;
    }
	Life_table_method life_Table_Method = new Life_table_method();

	public string LifeTableItem_To_CSVString(LifeTableItem lifeTableItem)
	{
		string CSV_string =
		lifeTableItem.x.ToString() + "," +
		lifeTableItem.nx.ToString() + "," +
		lifeTableItem.lx.ToString() + "," +
		lifeTableItem.dx.ToString() + "," +
		lifeTableItem.qx.ToString() + "," +
		lifeTableItem.Lx.ToString() + "," +
		lifeTableItem.Tx.ToString() + "," +
		lifeTableItem.ex.ToString() + "\n";
		
		return CSV_string;
	}
	private void submitButton_Click(object sender, RoutedEventArgs e)
	{
        Debug.WriteLine("hello");
    }

	private void Slider_optionIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		// 当Slider的值发生变化时，更新TextBlock的文本
		// TextBlock1.Text = Slider1.Value.ToString();
	}
	private void Slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		// 当Slider的值发生变化时，更新TextBlock的文本
		TextBlock1.Text = Slider1.Value.ToString();
	}

	/// <summary>
	/// 将一个?bool转为bool
	/// </summary>
	/// <param name="_bool"></param>
	/// <returns></returns>
	private bool tobool(bool? _bool) 
	{
        if (_bool == true)
        {
			return true;
		}
        else
        {
            return false;
        }

    }
	private void NewData_Click(object sender, RoutedEventArgs e)
	{

		ModeType_Enum modeType_ = ModeType_Enum.Concave_line_type_Enum;

		if (tobool(Concave_line_type.IsChecked))
        {
			//Debug.WriteLine(Concave_line_type);
			modeType_ = ModeType_Enum.Concave_line_type_Enum;

		}
		else if (tobool(Diagonal_line_type.IsChecked))
		{
			//Debug.WriteLine(Diagonal_line_type);
			modeType_ = ModeType_Enum.Diagonal_line_type_Enum;
		}
        else if(tobool(Convex_line_type.IsChecked))
        {
			//Debug.WriteLine(Convex_line_type);
			modeType_ = ModeType_Enum.Convex_line_type_Enum;
		}
		Debug.WriteLine(modeType_);
		modelType = modeType_;
		int Sample_size_Num=0;
		if (Sample_size_Box.Text != null && Sample_size_Box.Text !="")
		{
			Sample_size_Num = int.Parse(Sample_size_Box.Text);
		
        // if (Sample_size_NumberBox.Value != null) { Sample_size_Num = ((int)Sample_size_NumberBox.Value); }
        Debug.WriteLine(Sample_size_Num.ToString());
			int StartValue = Sample_size_Num;
			var nx_Life_table2 =  life_Table_Method.Life_table_method_fucnction(modeType_, Sample_size_Num, int.Parse(TextBlock1.Text), tbMultiLine.Text);
			// int[] nx_Life_table = { 142, 62, 34, 20, 15, 11, 6, 2, 2, 0 };
			var nx_Life_table = new int[nx_Life_table2.Length + 1];

			nx_Life_table[0] = StartValue;

			Array.Copy(nx_Life_table2, 0, nx_Life_table, 1, nx_Life_table2.Length);

			float[] lx_Life_table = life_Table_Method.Get_lx_Method(nx_Life_table);
		float[] dx_Life_table = life_Table_Method.Get_dx_Method(nx_Life_table);
		float[] qx_Life_table = life_Table_Method.Get_qx_Method(nx_Life_table, dx_Life_table);
		float[] Lx_Life_table = life_Table_Method.Get_Lx_Method(nx_Life_table);
		float[] Tx_Life_table = life_Table_Method.Get_Tx_Method(Lx_Life_table);
		float[] ex_Life_table = life_Table_Method.Get_ex_Method(Tx_Life_table, nx_Life_table);

		List<LifeTableItem> list = new List<LifeTableItem> { };
		for (int i = 0; i <= nx_Life_table.Length - 1; i++)
		{
			LifeTableItem lifeTableItem = new LifeTableItem(i, nx_Life_table[i], dx_Life_table[i], lx_Life_table[i], qx_Life_table[i], Lx_Life_table[i], Tx_Life_table[i], ex_Life_table[i]);
			list.Add(lifeTableItem);
		}
		DataList = list;

		DataGrid1.ItemsSource = DataList;

		float[] floatArray = nx_Life_table.Select(value => (float)value).ToArray();
		float[] newArray = floatArray.Take(floatArray.Length - 1).ToArray();

		List<float> lx_Life_table_list = new List<float>(newArray);

		// 对List<float>中的每个元素取log10
		log10List = lx_Life_table_list.Select(x => (float)Math.Log10(x)).ToList();
		}
		else
		{
			MessageBox.Show("请检查是否按要求填好所有值");

		}
	}

	private void Debug_Button_Click(object sender, RoutedEventArgs e)
	{
		string jsonString = tbMultiLine.Text;
		Debug.WriteLine(jsonString);
		// 解析JSON字符串为对象
		var gamerules = JsonSerializer.Deserialize<Gamerules>(jsonString);

		// 输出解析结果
		Debug.WriteLine($"killNumber: {gamerules.killNumber}, Rounds: {gamerules.Rounds}");
	}

	private void print_Button_Click(object sender, RoutedEventArgs e)
	{
		// 假设这是你的List<float>
		List<float> lx_Life_table_list = log10List;

		// 将List<float>转换成字符串，每个元素之间用逗号分隔
		string result = string.Join(", ", lx_Life_table_list.Select(x => x.ToString()));
		result = "[" + result + "]";

		string resultJson = "{\n\t"
			+ "\"nXresult\":" + result+",\n\t"
			+ "\"type\":\"" + modelType.ToString()
			+ "\"\n}";

		Debug.WriteLine(resultJson);
		Window1 window1 = new Window1();
		window1.log10ListString = resultJson;
		window1.Show();
    }
	
	private void Save_Button_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.FileName = $"{modelType}_result.csv"; // Default file name
		saveFileDialog.DefaultExt = ".csv"; // Default file extension
		saveFileDialog.Filter = "逗号分隔值(Comma-Separated Values, CSV) |*.csv";

		string wholeString = "x,nx,lx,dx,qx,Lx,Tx,ex\n";

        foreach (var DataListitem in DataList)
        {
			wholeString += LifeTableItem_To_CSVString(DataListitem);
		}

        if ((bool)saveFileDialog.ShowDialog())
		{
			string fileName = saveFileDialog.FileName;
			System.IO.File.WriteAllText(fileName, wholeString);
			// pngExporter.ExportToFile(plotModel, fileName);
		}
	}

	private void Concave_line_type_Click(object sender, RoutedEventArgs e)
	{
		// Concave_line_type Diagonal_line_type Convex_line_type
		if (Concave_line_type.IsChecked == true)
		{
			Slider1.IsEnabled = false;
			tbMultiLine.IsEnabled = true;
			tbMultiLine.Text = "{\n\"killNumber\":[2,3,4],\n\"Rounds\":[2,4,30]\n}";
			// IsEnabled = false
		}
		else if (Diagonal_line_type.IsChecked == true)
		{
			tbMultiLine.IsEnabled = false;
			Slider1.IsEnabled = true;
		}
		else if (Convex_line_type.IsChecked == true)
		{
			Slider1.IsEnabled = false;
			tbMultiLine.IsEnabled = true;
			tbMultiLine.Text = "{\n\"killNumber\":[5,3,2],\n\"Rounds\":[2,4,30]\n}";
		}
		else { Debug.Write("?"); }
	}

	private void ResetButton_Click(object sender, RoutedEventArgs e)
	{
		DataGrid1.ItemsSource = null;
		Sample_size_Box.Text = string.Empty;
	}

	private void Exit_Button_Click(object sender, RoutedEventArgs e)
	{
		Environment.Exit(0);
	}

	private void OpenJson_Button_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*|text file (.txt)|*.txt",
			Title = "选择配置文件"
		};

		if (openFileDialog.ShowDialog() == true)
			tbMultiLine.Text = File.ReadAllText(openFileDialog.FileName);
	}

	private void SaveJson_Button_Click(object sender, RoutedEventArgs e)
	{

		SaveFileDialog saveFileDialog = new SaveFileDialog
		{
			Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*|text file (.txt)|*.txt",
			Title = "选择配置文件保存位置"
		}; ;

		if (saveFileDialog.ShowDialog() == true)
			File.WriteAllText(saveFileDialog.FileName, tbMultiLine.Text);
	}

	private void OpenCSV_Button_Click(object sender, RoutedEventArgs e)
	{
		// 用于存储第二列数据的列表
		List<int> secondColumn = new List<int>();

		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Filter = "CSV files (*.csv)|*.json|All files (*.*)|*.*|text file (.txt)|*.txt",
			Title = "选择表格文件"
		};

		if (openFileDialog.ShowDialog() == true)
		{
			// CSV文件的路径
			string csvFilePath = openFileDialog.FileName;
			// 读取CSV文件
			using (StreamReader reader = new StreamReader(csvFilePath))
			{
				string line;
				// 逐行读取
				while ((line = reader.ReadLine()) != null)
				{
					// 使用逗号分隔符分割每行的数据
					string[] columns = line.Split(',');

					// 检查列数是否足够
					if (columns.Length > 1)
					{
						// 尝试将第二列的数据转换为整数并添加到列表中
						if (int.TryParse(columns[1], out int value))
						{
							secondColumn.Add(value);
						}
						else
						{
							// 如果转换失败，可以打印错误消息或者处理异常
							Console.WriteLine("Warning: Cannot convert to int - " + columns[1]);
						}
					}
				}
			}
			// 将列表转换为数组
			
		}
		int[] nx_Life_table = secondColumn.ToArray();

		float[] lx_Life_table = life_Table_Method.Get_lx_Method(nx_Life_table);
		float[] dx_Life_table = life_Table_Method.Get_dx_Method(nx_Life_table);
		float[] qx_Life_table = life_Table_Method.Get_qx_Method(nx_Life_table, dx_Life_table);
		float[] Lx_Life_table = life_Table_Method.Get_Lx_Method(nx_Life_table);
		float[] Tx_Life_table = life_Table_Method.Get_Tx_Method(Lx_Life_table);
		float[] ex_Life_table = life_Table_Method.Get_ex_Method(Tx_Life_table, nx_Life_table);

		List<LifeTableItem> list = new List<LifeTableItem> { };
		for (int i = 0; i <= nx_Life_table.Length - 1; i++)
		{
			LifeTableItem lifeTableItem = new LifeTableItem(i, nx_Life_table[i], dx_Life_table[i], lx_Life_table[i], qx_Life_table[i], Lx_Life_table[i], Tx_Life_table[i], ex_Life_table[i]);
			list.Add(lifeTableItem);
		}
		DataList = list;

		DataGrid1.ItemsSource = DataList;

		float[] floatArray = nx_Life_table.Select(value => (float)value).ToArray();
		float[] newArray = floatArray.Take(floatArray.Length - 1).ToArray();

		List<float> lx_Life_table_list = new List<float>(newArray);

		// 对List<float>中的每个元素取log10
		log10List = lx_Life_table_list.Select(x => (float)Math.Log10(x)).ToList();


	}

	private void Help_Button_Click(object sender, RoutedEventArgs e)
	{
		
		string url = "https://github.com/zhaowb2003/LifeTable-AutomatedGenerationSolution-in-WPF?tab=readme-ov-file#lifetable-automatedgenerationsolution-in-wpf---step-to-step--%E6%89%8B%E6%8A%8A%E6%89%8B%E4%BD%BF%E7%94%A8%E6%95%99%E7%A8%8B";
		// 使用默认浏览器打开网页
		Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
	}
}

public class GamerulesString
{
	//	Debug.WriteLine($"killNumber: {gamerulesString.killNumber}, Rounds: {gamerulesString.Rounds}");
	public string killNumber { get; set; }
	public string Rounds { get; set; }
}