using cAlgo.API;
using cAlgo.ChartObjectModels;
using System.Threading;
using System.Windows.Forms;
using Button = cAlgo.API.Button;

namespace cAlgo.Controls
{
    public class PatternsSaveButton : Button
    {
        private readonly Chart _chart;

        public PatternsSaveButton(Chart chart)
        {
            _chart = chart;

            Text = "Save";

            Click += OnClick;
        }

        private void OnClick(ButtonClickEventArgs obj)
        {
            var thread = new Thread(() =>
            {
                var chartObjectModels = _chart.GetObjectModels();

                if (chartObjectModels.Length == 0)
                {
                    MessageBox.Show("There is no pattern object on your chart to save", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Save Chart Patters",
                    Filter = "pt Files (*.pt)|*.pt",
                    RestoreDirectory = true
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

                ChartObjectsSerializer.Serialize(chartObjectModels, saveFileDialog.FileName);
            });

            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();

            thread.Join();
        }
    }
}