// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.EventBus.ActiveMQ;

public class DefaultActiveMQPersistentConnection
    : IActiveMQPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultActiveMQPersistentConnection> _logger;
    private readonly int _retryCount;
    private IConnection _connection;
    private bool _disposed;
    readonly object sync_root = new object();

    public DefaultActiveMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultActiveMQPersistentConnection> logger, int retryCount = 5)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryCount = retryCount;
    }

    public bool IsConnected
    {
        get
        {
            return _connection != null && _connection.IsStarted && !_disposed;
        }
    }

    public ISession CreateSession()
    {
        return CreateSession(AcknowledgementMode.AutoAcknowledge);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            _connection.ExceptionListener -= OnExceptionListener;
            _connection.ConnectionInterruptedListener -= OnConnectionInterruptedListener;
            _connection.ConnectionResumedListener -= OnConnectionResumedListener;

            _connection.Dispose();
        }
        catch (IOException ex)
        {
            _logger.CriticalDefaultActiveMQPersistentConnection(ex);
        }
    }

    private void OnExceptionListener(Exception exception)
    {
        if (_disposed)
        {
            return;
        }

        _logger.WarningActiveMQConnectionThrowException();

        TryConnect();
    }

    private void OnConnectionResumedListener()
    {
        if (_disposed)
        {
            return;
        }

        _logger.WarningActiveMQConnectionThrowException();

        TryConnect();
    }

    private void OnConnectionInterruptedListener()
    {
        if (_disposed)
        {
            return;
        }

        _logger.WarningActiveMQConnectionThrowException();

        TryConnect();
    }

    public bool TryConnect()
    {
        _logger.InformationActiveMQTryingConnect();

        lock (sync_root)
        {
            var policy = Policy.Handle<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.WarningActiveMQCouldNotConnect(time.TotalSeconds, ex);
                }
            );

            policy.Execute(() =>
            {
                _connection = _connectionFactory
                        .CreateConnection();
                _connection.Start();
            });

            if (IsConnected)
            {
                _connection.ExceptionListener += OnExceptionListener;
                _connection.ConnectionInterruptedListener += OnConnectionInterruptedListener;
                _connection.ConnectionResumedListener += OnConnectionResumedListener;

                if (_connection is Apache.NMS.AMQP.NmsConnection)
                {
                    var hostname = ((Apache.NMS.AMQP.NmsConnection)_connection).ConnectionInfo.ConfiguredUri.Host;

                    _logger.InformationActiveMQAcquiredPersistentConnection(hostname);

                }


                return true;
            }
            else
            {
                _logger.CriticalActiveMQCouldNotBeCreated();

                return false;
            }
        }
    }

    public ISession CreateSession(AcknowledgementMode acknowledgementMode)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("No ActiveMQ connections are available to perform this action");
        }

        return _connection.CreateSession(acknowledgementMode);
    }
}
