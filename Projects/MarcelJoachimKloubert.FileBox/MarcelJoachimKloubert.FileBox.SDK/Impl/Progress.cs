// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox.Impl
{
    internal sealed class Progress : NotificationObjectBase, IProgress
    {
        #region Fields (3)

        private int? _category;
        private string _description;
        private double? _percentage;

        #endregion Fields (3)

        #region Properties (3)

        public int? Category
        {
            get { return this._category; }

            internal set
            {
                this.OnPropertyChange(() => this.Category, ref this._category, value);
            }
        }

        public string Description
        {
            get { return this._description; }

            internal set
            {
                this.OnPropertyChange(() => this.Description, ref this._description, value);
            }
        }

        public double? Percentage
        {
            get { return this._percentage; }

            internal set
            {
                this.OnPropertyChange(() => this.Percentage, ref this._percentage, value);
            }
        }

        #endregion Properties (3)

        #region Methods (2)

        internal void Update(double min,
                             double max,
                             string description = null,
                             int? category = null)
        {
            double? percentage = null;
            if (max != 0)
            {
                percentage = min / max * 100d;
            }

            this.Update(percentage: percentage,
                        description: description,
                        category: category);
        }

        internal void Update(double? percentage = null,
                             string description = null,
                             int? category = null)
        {
            this.Percentage = percentage;
            this.Description = description;
            this.Category = category;
        }

        #endregion Methods (2)
    }
}