using cAlgo.API;
using System;

namespace cAlgo.Controls
{
    public class ToggleButton : Button
    {
        private Color _onColor;
        private Color _offColor;

        public ToggleButton()
        {
            Click += args =>
            {
                if (IsOn)
                {
                    TurnOff();
                }
                else
                {
                    TurnOn();
                }
            };
        }

        public event Action<ToggleButton> TurnedOn;

        public event Action<ToggleButton> TurnedOff;

        public Color OnColor
        {
            get { return _onColor; }
            set
            {
                _onColor = value;

                if (IsOn) BackgroundColor = OnColor;
            }
        }

        public Color OffColor
        {
            get { return _offColor; }
            set
            {
                _offColor = value;

                if (!IsOn) BackgroundColor = _offColor;
            }
        }

        public bool IsOn { get; private set; }

        public void TurnOn()
        {
            IsOn = true;

            BackgroundColor = OnColor;

            var turnedOnEvent = TurnedOn;

            if (turnedOnEvent != null) turnedOnEvent.Invoke(this);

            OnTurnedOn();
        }

        public void TurnOff()
        {
            IsOn = false;

            BackgroundColor = OffColor;

            var turnedOffEvent = TurnedOff;

            if (turnedOffEvent != null) turnedOffEvent.Invoke(this);

            OnTurnedOff();
        }

        protected virtual void OnTurnedOn()
        {
        }

        protected virtual void OnTurnedOff()
        {
        }
    }
}