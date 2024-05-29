using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace sample_application;

public class GraphTransfer
{
    private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    // Generate a Tranfer Graph using the supplied tick
    public static PlotModel TransferByTick(Dictionary<string, Dictionary<string, ulong>> data, DateTimeIntervalType dtick, string title)
    {
        PlotModel plotModel = new PlotModel();

        plotModel.Title = title;
        plotModel.Background = OxyColors.White;

        DateTimeAxis xAxis = new DateTimeAxis();
        xAxis.Key = "x";
        xAxis.Title = "Timestamp";
        xAxis.Angle = 270.0;
        xAxis.AxisTickToLabelDistance = 75.0;
        xAxis.AxisTitleDistance = -70.0;
        xAxis.IntervalType = dtick;
        xAxis.MinorIntervalType = dtick;
        xAxis.IntervalLength = 1920.0;
        plotModel.Axes.Add(xAxis);

        LinearAxis y1Axis = new LinearAxis();
        y1Axis.Key = "y1";
        y1Axis.Title = "Bytes Transferred";
        y1Axis.Position = AxisPosition.Left;
        //y1Axis.Maximum = 10;
        //y1Axis.Minimum = 0;
        // FIXME: how to make this do (MB, GB, TB) ?
        plotModel.Axes.Add(y1Axis);

        LineSeries seriesUp = new LineSeries();
        seriesUp.YAxisKey = "y1";
        seriesUp.MarkerType = MarkerType.Circle;
        seriesUp.Color = OxyColors.Blue;

        LineSeries seriesDown = new LineSeries();
        seriesDown.YAxisKey = "y1";
        seriesDown.MarkerType = MarkerType.Circle;
        seriesDown.Color = OxyColors.Green;

        List<string> keyList = data.Keys.ToList();
        _logger.Debug("Point Count: {count}", keyList.Count);
        foreach (string keyListItem in keyList)
        {
            DateTime keyDateTime = DateTime.Parse(keyListItem);
            seriesUp.Points.Add(new DataPoint(DateTimeAxis.ToDouble(keyDateTime), data[keyListItem]["up"]));
            seriesDown.Points.Add(new DataPoint(DateTimeAxis.ToDouble(keyDateTime), data[keyListItem]["down"]));
        }

        plotModel.Series.Add(seriesUp);
        plotModel.Series.Add(seriesDown);

        return plotModel;
    }
}
