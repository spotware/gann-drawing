using cAlgo.API;
using cAlgo.Helpers;
using System.Linq;
using System.Windows.Forms;
using Button = cAlgo.API.Button;

namespace cAlgo.Controls
{
    public class PatternsRemoveAllButton : Button
    {
        private readonly Chart _chart;

        public PatternsRemoveAllButton(Chart chart)
        {
            _chart = chart;

            Text = "Remove All";

            Click += OnClick;
        }

        private void OnClick(ButtonClickEventArgs obj)
        {
            var dialogResult = MessageBox.Show("Are you sure you want to remove all patterns from this chart?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (dialogResult != DialogResult.OK) return;

            var chartObjects = _chart.Objects.ToArray();

            foreach (var chartObject in chartObjects)
            {
                if (!chartObject.IsPattern() || chartObject.IsHidden) continue;

                _chart.RemoveObject(chartObject.Name);
            }
        }
    }
}