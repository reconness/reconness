<template>
    <div class="pt-2 row">
        <div class="mx-auto"><strong>List of Agents</strong></div>

        <div class="col-12">
            <table class="table table-striped">
                <thead class="thead-dark">
                    <tr>
                        <th scope="col">Name</th>
                        <th scope="col">Categories</th>
                        <th scope="col">Last Run</th>
                        <th scope="col">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="agent in agents" v-bind:key="agent.id">
                        <th class="w-25" scope="row">{{ agent.name }}</th>
                        <td class="w-25">{{ agent.categories.join(', ') }}</td>
                        <td class="w-25">{{ agent.lastRun | formatDate('YYYY-MM-DD') }}</td>
                        <td class="w-25">
                            <button class="btn btn-primary ml-2" v-on:click="onConfirmCommand(agent)" v-if="!agent.isRunning">Run</button>
                            <button class="btn btn-danger ml-2" v-on:click="onStopAgent(agent)" v-if="agent.isRunning">Stop</button>
                            <button class="btn btn-dark ml-2" v-on:click="showTerminalModal = !showTerminalModal" v-if="agent.isRunning">Terminal</button>
                            <button class="btn btn-dark ml-2" v-on:click="showLogModal = !showLogModal" v-if="agent.isRunning">Logs</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="commandModal">
            <!-- Modal-->
            <transition @enter="startTransitionCommandModal" @after-enter="endTransitionCommandModal" @before-leave="endTransitionCommandModal" @after-leave="startTransitionCommandModal">
                <div class="modal fade" v-if="showCommandModal" ref="commandModal">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="exampleModalLabel">Confirm Command</h5>
                                <button class="close" type="button" v-on:click="closeConfirmModal"><span aria-hidden="true">x</span></button>
                            </div>
                            <div class="modal-body">
                                <div class="form-group">
                                    <label for="command">Command</label>
                                    <input class="form-control" id="command" v-model="currentAgent.command" />
                                </div>
                                <div class="form-group" v-if="currentAgent.repository">
                                    <label class="form-check-label">
                                        Repository:
                                    </label>
                                    <a :href="currentAgent.repository" target="_blank">{{currentAgent.repository}}</a>
                                </div>
                                <div class="form-group form-check">
                                    <input class="form-check-input" type="checkbox" id="notifyIfAgentDone" ref="notifyIfAgentDone" v-model="currentAgent.activateNotification">
                                    <label class="form-check-label" for="notifyIfAgentDone">
                                        Activate Notification
                                    </label>
                                </div>
                                <div class="form-group">
                                    <button class="btn btn-primary ml-2" v-on:click="onRunAgent(currentAgent)">Run</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </transition>
            <div class="modal-backdrop fade d-none" ref="commandBackdrop"></div>
        </div>

        <div class="terminalModal">
            <!-- Modal-->
            <transition @enter="startTransitionModal" @after-enter="endTransitionModal" @before-leave="endTransitionModal" @after-leave="startTransitionModal">
                <div class="modal fade" v-if="showTerminalModal" ref="terminalModal">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="exampleModalLabel">Terminal</h5>
                                <button class="close" type="button" v-on:click="showTerminalModal = !showTerminalModal"><span aria-hidden="true">x</span></button>
                            </div>
                            <div class="modal-body">
                                <div id="terminal"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </transition>
            <div class="modal-backdrop fade d-none" ref="terminalBackdrop"></div>
        </div>

        <div class="logModal">
            <!-- Modal-->
            <transition @enter="startTransitionLogModal" @after-enter="endTransitionLogModal" @before-leave="endTransitionLogModal" @after-leave="startTransitionLogModal">
                <div class="modal fade" v-if="showLogModal" ref="logsModal">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="exampleModalLabel">Logs</h5>
                                <button class="close" type="button" v-on:click="showLogModal = !showLogModal"><span aria-hidden="true">x</span></button>
                            </div>
                            <div class="modal-body">
                                <div id="log"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </transition>
            <div class="modal-backdrop fade d-none" ref="logsBackdrop"></div>
        </div>

    </div>
</template>

<script>

    import helpers from '../../helpers'
    import { Terminal } from 'xterm'

    export default {
        name: 'AgentTag',
        props: {
            isTarget: {
                type: Boolean,
                default: false
            }
        },
        data() {
            return {
                showCommandModal: false,
                showTerminalModal: false,
                showLogModal: false,
                term: null,
                termLog: null,
                agents: [],
                currentAgent: null,
                runningAgents: []
            }
        },
        computed: {
            targetName: function () {
                return this.$route.params.targetName
            },
            rootDomain: function () {
                return this.$route.params.rootDomain
            },
            subdomain: function () {
                return this.$route.params.subdomain
            }
        },
        watch: {
            '$route'() {
                this.connectAgent()
            }
        },
        async mounted() {

            if (this.$store.state.agents.agents.length === 0) {
                await this.$store.dispatch('agents/agents')
            }

            if (this.isTarget) {
                this.agents = this.$store.state.agents.agents
            }
            else {
                this.agents = this.$store.getters['agents/subdomainAgents']
            }

            this.runningAgents = await this.$store.dispatch('agents/runningAgents', { targetName: this.targetName, rootDomain: this.rootDomain, subdomain: this.subdomain })
            this.connectAgent()
        },
        methods: {
            connectAgent() {
                if (this.agents.length > 0) {
                    this.agents.map(agent => {

                        if (this.runningAgents.length !== 0) {
                            if (this.runningAgents.indexOf(agent.name) > -1) {
                                agent.isRunning = true
                            }
                        }

                        const channel = this.isTarget ?
                            `${agent.name}_${this.targetName}_${this.rootDomain}` :
                            `${agent.name}_${this.targetName}_${this.rootDomain}_${this.subdomain}`

                        this.$connection.on(channel, (message) => {
                            if (message === "Agent stopped!" || message === "Agent done!") {
                                agent.isRunning = false
                                this.currentAgent = null
                            }

                            if (this.term !== null) {
                                console.log(message)
                                this.term.writeln(message)
                            }
                        });

                        this.$connection.on("logs_" + channel, (message) => {

                            if (this.termLog !== null) {
                                console.log("logs_" + message)
                                this.termLog.writeln(message)
                            }
                        });
                    })
                }
            },
            async onConfirmCommand(agent) {
                if (agent.isRunning) {
                    return
                }

                this.showCommandModal = true
                this.currentAgent = agent
            },
            async onRunAgent(agent) {
                if (agent.isRunning) {
                    return
                }

                agent.isRunning = true
                this.showCommandModal = false
                this.showTerminalModal = true

                const agentName = agent.name

                try {
                    await this.$store.dispatch('agents/run', {
                        agent: agentName,
                        command: agent.command,
                        target: this.targetName,
                        rootDomain: this.rootDomain,
                        subdomain: this.subdomain,
                        activateNotification: agent.activateNotification
                    })
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            async onStopAgent(agent) {
                if (!agent.isRunning) {
                    return
                }

                this.currentAgent = null

                const agentName = agent.name
                try {
                    await this.$store.dispatch('agents/stop', {
                        agent: agentName,
                        target: this.targetName,
                        rootDomain: this.rootDomain,
                        subdomain: this.subdomain
                    })
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            disabledCanRun(agent) {
                const anotherAgentIsRunning = this.currentAgent != null && this.currentAgent.name !== agent.name
                const currentSubdomain = this.$store.state.subdomains.currentSubdomain
                const needToBeAlive = this.subdomain !== undefined ? agent.onlyIfIsAlive === true && currentSubdomain.isAlive !== true : false


                return needToBeAlive || anotherAgentIsRunning
            },
            closeConfirmModal() {
                this.currentAgent = null
                this.showCommandModal = false
            },
            startTransitionCommandModal() {
                this.$refs.commandBackdrop.classList.toggle("d-block");
                if (this.$refs.commandModal !== undefined) {
                    this.$refs.commandModal.classList.toggle("d-block");
                }
            },
            endTransitionCommandModal() {
                this.$refs.commandBackdrop.classList.toggle("show");
                if (this.$refs.commandModal !== undefined) {
                    this.$refs.commandModal.classList.toggle("show");
                }
            },
            startTransitionModal() {
                this.$refs.terminalBackdrop.classList.toggle("d-block");
                if (this.$refs.terminalModal !== undefined) {
                    this.$refs.terminalModal.classList.toggle("d-block");

                    this.term = new Terminal({
                        disableStdin: true
                    })
                    this.term.open(document.getElementById('terminal'))
                }
            },
            endTransitionModal() {
                this.$refs.terminalBackdrop.classList.toggle("show");
                if (this.$refs.terminalModal !== undefined) {
                    this.$refs.terminalModal.classList.toggle("show");
                }
            },
            startTransitionLogModal() {
                this.$refs.logsBackdrop.classList.toggle("d-block");
                if (this.$refs.logsModal !== undefined) {
                    this.$refs.logsModal.classList.toggle("d-block");

                    this.termLog = new Terminal({
                        disableStdin: true
                    })
                    this.termLog.open(document.getElementById('log'))
                }
            },
            endTransitionLogModal() {
                this.$refs.logsBackdrop.classList.toggle("show");
                if (this.$refs.logsModal !== undefined) {
                    this.$refs.logsModal.classList.toggle("show");
                }
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .modal-dialog {
        max-width: 800px !important;
    }
</style>