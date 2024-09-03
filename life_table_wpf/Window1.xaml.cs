using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using OxyPlot;
using OxyPlot.Series;
using System.Diagnostics;
using OxyPlot.Wpf;
using Microsoft.Win32;

namespace life_table_wpf
{
	/// <summary>
	/// Window1.xaml 的交互逻辑
	/// </summary>
	public partial class Window1 : Window
	{
		public PlotModel Model { get; set; }
		public string log10ListString { get; set; }
		public Window1()
		{
			InitializeComponent();
		}

		private void PhotoSave_Click(object sender, RoutedEventArgs e)
		{
			PlotModel plotModel = MyPlotView.Model;

			var pngExporter = new PngExporter { Width = 600, Height = 400 };
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = "result.png"; // Default file name
			saveFileDialog.DefaultExt = ".png"; // Default file extension
			saveFileDialog.Filter = "图像文件|*.png"; // Filter files by extension
												  // saveFileDialog.InitialDirectory = initFolder;
			if ((bool)saveFileDialog.ShowDialog())
			{
				string fileName = saveFileDialog.FileName;
				pngExporter.ExportToFile(plotModel, fileName);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var tmp = new PlotModel { Subtitle = "using OxyPlot" };
			MainViewModel mainViewModel = new MainViewModel();
			var plotRules = JsonSerializer.Deserialize<PlotRules>(log10ListString);
			float[] nx_floats = plotRules.nXresult;
			// 生成一个从0开始的整数序列，与原始数组同长度
			int[] sequence = Enumerable.Range(0, nx_floats.Length).ToArray();
			// 将整数序列转换为float数组
			float[] X_floatSequence = sequence.Select(x => (float)x).ToArray();
			int i = 0;
			var titleString = "";
			if (plotRules.type == "Concave_line_type_Enum") {
				titleString = "凹线型"; Debug.WriteLine($"{titleString}");
			}
			else if (plotRules.type == "Diagonal_line_type_Enum") {
				titleString = "对角线型"; Debug.WriteLine($"{titleString}");
				LeastSquareMethodResult leastSquareMethodResult = new LeastSquareMethodResult(X_floatSequence, nx_floats);
				List<float> Y_MEAN_floats_L = new List<float>();
				foreach (var x in X_floatSequence)
				{
					var Y_M = leastSquareMethodResult.getLineY(x);
					Y_MEAN_floats_L.Add(Y_M);



				}
				float[] Y_MEAN_floats = Y_MEAN_floats_L.ToArray();

				var lineSeries = new LineSeries
				{
					Points = { new DataPoint(X_floatSequence[0], Y_MEAN_floats[0]), new DataPoint(X_floatSequence[X_floatSequence.Length - 1], Y_MEAN_floats[X_floatSequence.Length - 1]) },
					Color = OxyColors.Red
				};

				tmp.Series.Add(lineSeries);

				if (tmp != null)
				{
					// tmp.Title = $"{titleString} 示例绘图";
					tmp.Subtitle = $"拟合的直线 y = {leastSquareMethodResult.m:0.00}x + {leastSquareMethodResult.b:0.00}";
				}
			}
			else if (plotRules.type == "Convex_line_type_Enum") 
			{
				titleString = "凸线型"; Debug.WriteLine($"{titleString}");
			}
			else
			{ 
				throw (new NoneLineTypeException("请在主页面选择类型！")); 
			}

			// Title = $"{titleString} 示例绘图",

				if (tmp != null)
				{
				tmp.Title = $"{titleString} 示例绘图";
				// tmp.Subtitle = $"拟合的直线 y = {m:0.00}x + {b:0.00}";}​
				}
				var scatterSeries = new ScatterSeries
				{
					MarkerType = MarkerType.Circle,
					MarkerSize = 3,
					MarkerFill = OxyColors.Blue
				};

				// Create two line series (markers are hidden by default)
				// var series3 = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };

				foreach (var nx_float in nx_floats)
				{
					scatterSeries.Points.Add(new ScatterPoint(i, nx_float));
					i = i + 1;
				}

				tmp.Series.Add(scatterSeries);

				mainViewModel.Model = tmp;
				//MessageBox.Show(log10ListString);
				Debug.WriteLine(log10ListString);
				// MessageBox.Show(plotRules.type + plotRules.nXresult.ToString());
				MyPlotView.Model = mainViewModel.Model;
			}
		}

	public class LeastSquareMethodResult
	{
		public float m { get; set; }
		public float b { get; set; }

		public LeastSquareMethodResult(float[] X_Floats, float[] Y_Floats)
		{
			float a = 0;
			float c = 0;

			float x_mean = X_Floats.Average();
			float y_mean = Y_Floats.Average();

			//计算a和c
			for (int i = 0; i < X_Floats.Length; i++)
			{
				a += (X_Floats[i] - x_mean) * (Y_Floats[i] - y_mean);
				c += (X_Floats[i] - x_mean) * (X_Floats[i] - x_mean);
			}
			//计算斜率和截距
			float m_ = a / c;
			float b_ = y_mean - m_ * x_mean;
			this.m = m_;
			this.b = b_;
		}

		public float getLineY(float x) { return this.m * x + this.b; }
	}

	public class MainViewModel
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="MainViewModel" /> class.
			/// </summary>
			public MainViewModel()
			{
				// Create the plot model
				var tmp = new PlotModel { Title = "Simple example", Subtitle = "using OxyPlot" };

				// Create two line series (markers are hidden by default)
				var series1 = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };
				series1.Points.Add(new DataPoint(0, 0));
				series1.Points.Add(new DataPoint(10, 18));
				series1.Points.Add(new DataPoint(20, 12));
				series1.Points.Add(new DataPoint(30, 8));
				series1.Points.Add(new DataPoint(40, 15));

				var series2 = new LineSeries { Title = "Series 2", MarkerType = MarkerType.Square };
				series2.Points.Add(new DataPoint(0, 4));
				series2.Points.Add(new DataPoint(10, 12));
				series2.Points.Add(new DataPoint(20, 16));
				series2.Points.Add(new DataPoint(30, 25));
				series2.Points.Add(new DataPoint(40, 5));


				// Add the series to the plot model
				tmp.Series.Add(series1);
				tmp.Series.Add(series2);

				// Axes are created automatically if they are not defined

				// Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
				this.Model = tmp;
			}

			/// <summary>
			/// Gets the plot model.
			/// </summary>
			public PlotModel Model { get; set; }
		}

		public class PlotRules
		{
			public float[] nXresult { get; set; }
			public string type { get; set; }
		}

		public class NoneLineTypeException : ApplicationException
		{
			public NoneLineTypeException(string message) : base(message)
			{
			}
		}

} 
