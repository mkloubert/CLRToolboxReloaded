// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Xml;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    internal sealed class BinaryMessage : Message
    {
        #region Fields (2)

        private MessageHeaders _HEADERS;
        private MessageProperties _PROPERTIES;

        #endregion Fields (2)

        #region Constructors (1)

        internal BinaryMessage(IEnumerable<byte> data)
        {
            this.Data = data.AsArray();
            this._HEADERS = new MessageHeaders(MessageVersion.None);

            this._PROPERTIES = new MessageProperties();
            this._PROPERTIES
                .Add(WebBodyFormatMessageProperty.Name,
                     new WebBodyFormatMessageProperty(WebContentFormat.Raw));
        }

        #endregion Constructors (1)

        #region Properties (4)

        /// <inheriteddoc />
        internal byte[] Data
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public override MessageHeaders Headers
        {
            get { return this._HEADERS; }
        }

        /// <inheriteddoc />
        public override MessageProperties Properties
        {
            get { return this._PROPERTIES; }
        }

        /// <inheriteddoc />
        public override MessageVersion Version
        {
            get { return MessageVersion.None; }
        }

        #endregion Properties (4)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            var writeState = writer.WriteState;

            if (writeState == WriteState.Start)
            {
                writer.WriteStartElement("Binary");
            }

            var data = this.Data ?? new byte[0];
            writer.WriteBase64(data, 0, data.Length);

            if (writeState == WriteState.Start)
            {
                writer.WriteEndElement();
            }
        }

        #endregion Methods (1)
    }
}