import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

export default {
    start() {
        const connection = new HubConnectionBuilder()
            .withUrl(`AgentRunLogsHub`)
            .configureLogging(LogLevel.Information)
            .build()

        connection.start()

        return connection
    }
}