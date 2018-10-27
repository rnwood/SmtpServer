﻿namespace Rnwood.SmtpServer.Extensions
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public class EightBitMimeDataVerb : DataVerb, IParameterProcessor
    {
        public EightBitMimeDataVerb()
        {
        }

        public Task SetParameter(IConnection connection, string key, string value)
        {
            if (key.Equals("BODY", StringComparison.OrdinalIgnoreCase))
            {
                if (value.Equals("8BITMIME", StringComparison.CurrentCultureIgnoreCase))
                {
                    connection.CurrentMessage.EightBitTransport = true;
                }
                else if (value.Equals("7BIT", StringComparison.OrdinalIgnoreCase))
                {
                    connection.CurrentMessage.EightBitTransport = false;
                }
                else
                {
                    throw new SmtpServerException(
                        new SmtpResponse(
                            StandardSmtpResponseCode.SyntaxErrorInCommandArguments,
                                         "BODY parameter value invalid - must be either 7BIT or 8BITMIME"));
                }
            }

            return Task.CompletedTask;
        }

        public async override Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (connection.CurrentMessage != null && connection.CurrentMessage.EightBitTransport)
            {
                connection.SetReaderEncoding(Encoding.UTF8);
            }

            try
            {
                await base.ProcessAsync(connection, command).ConfigureAwait(false);
            }
            finally
            {
                await connection.SetReaderEncodingToDefault().ConfigureAwait(false);
            }
        }
    }
}