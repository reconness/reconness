import { HubConnectionBuilder } from '@aspnet/signalr'

export default {
    start() {
        const connection = new HubConnectionBuilder()
            .withUrl(`AgentRunLogsHub`)
            .build()

        connection.start()

        return connection
    }
}