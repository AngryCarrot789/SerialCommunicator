using SerialCommunicator.Graph;
using SerialCommunicator.Graph.DataPoints;
using SerialCommunicator.Utilities;
using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;

namespace SerialCommunicator.ViewModels
{
    public class GraphViewModel : BaseViewModel
    {
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }

        /// <summary>
        /// The graph is constantly plotted forever. So to 'plot data', change this value 
        /// to affect the To-Be-Plotted Y value which will be plotted eventually.
        /// </summary>
        public double ActivePlotValue { get; set; }

        public DispatcherTimer GraphTimer { get; set; }

        public GraphViewModel()
        {
            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MinimumY = 0;
            Controller.Range.MaximumY = 1024;
            Controller.Range.MaximumX = TimeSpan.FromSeconds(15);
            Controller.Range.AutoY = false;

            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Serial Values",
                Stroke = Color.FromRgb(11, 99, 205)
            });

            GraphTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(GlobalSettings.DEFAULT_GRAPH_SPEED_MS)
            };

            GraphTimer.Tick += GraphTimer_Tick;
        }
        private Stopwatch GraphStopWatch = new Stopwatch();
        private void GraphTimer_Tick(object sender, EventArgs e)
        {
            Controller.PushData(GraphStopWatch.Elapsed, ActivePlotValue);
        }
        public void StartPlotting()
        {
            GraphStopWatch = new Stopwatch();
            GraphStopWatch.Start();
            Controller.Clear();
            GraphTimer.Start();
        }

        public void StopPlotting()
        {
            //PlotLoopRunning = false;
            GraphStopWatch.Stop();
            GraphTimer.Stop();
        }

        /// <summary>
        /// Plots a value onto the graph. only a Y value is required as the graph is time-based (X is used by the timer)
        /// </summary>
        /// <param name="yValue">The height of the plot. Min = 0, Max = 1024</param>
        public void PlotGraph(double yValue)
        {
            ActivePlotValue = yValue;
        }
    }
}
