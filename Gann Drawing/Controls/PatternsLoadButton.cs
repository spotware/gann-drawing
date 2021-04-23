using cAlgo.API;
using cAlgo.ChartObjectModels;
using System.Threading;
using System.Windows.Forms;
using Button = cAlgo.API.Button;

namespace cAlgo.Controls
{
    public class PatternsLoadButton : Button
    {
        private readonly Chart _chart;

        public PatternsLoadButton(Chart chart)
        {
            _chart = chart;

            Text = "Load";

            Click += OnClick;
        }

        private void OnClick(ButtonClickEventArgs obj)
        {
            var thread = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Load Chart Patters",
                    Filter = "pt Files (*.pt)|*.pt",
                    RestoreDirectory = true
                };

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                var chartObjectModels = _chart.GetObjectModels();

                var models = ChartObjectsSerializer.Deserialize(openFileDialog.FileName);

                if (models.Length == 0)
                {
                    MessageBox.Show("There is no pattern object inside your selected file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                _chart.DrawModels(models);
            });

            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();

            thread.Join();
        }
    }
}