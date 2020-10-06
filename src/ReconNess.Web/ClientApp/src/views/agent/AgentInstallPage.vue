<template>
    <div>
        <h3>Install Agents</h3>
        <div class="col-12">
            <table class="table table-striped">
                <thead class="thead-dark">
                    <tr>
                        <th scope="col">Name</th>
                        <th scope="col">Categories</th>
                        <th scope="col">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="agent in agentMarketplaces" v-bind:key="agent.name">
                        <th class="w-25" scope="row">{{ agent.name }}</th>
                        <td class="w-25">{{ agent.category}}</td>
                        <td class="w-25">
                            <button class="btn btn-danger ml-2" v-if="installed(agent)" v-on:click="onUninstallation(agent)">
                                Uninstall
                            </button>
                            <button class="btn btn-primary ml-2" v-else v-on:click="onConfirmInstallation(agent)">
                                Install
                            </button>
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
                                <h5 class="modal-title" id="exampleModalLabel">Confirm Installation</h5>
                                <button class="close" type="button" v-on:click="showCommandModal = !showCommandModal"><span aria-hidden="true">x</span></button>
                            </div>
                            <div class="modal-body">
                                <div class="form-group">
                                    <p>Remember you need to have the Dockerfile with the Agent installation instruction.</p>
                                    <a href="https://raw.githubusercontent.com/reconness/reconness-agents/master/Dockerfile" target="_blank">Donwload Dockerfile</a>
                                </div>
                                <div class="form-group">
                                    <button class="btn btn-primary ml-2" v-on:click="install(currentAgent)">Install</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </transition>
            <div class="modal-backdrop fade d-none" ref="commandBackdrop"></div>
        </div>

    </div>
</template>

<script>

    import helpers from '../../helpers'

    export default {
        name: 'AgentInstallPage',
        data: () => {
            return {
                agentMarketplaces: [],
                currentAgent: null,
                showCommandModal: false
            }
        },
        async mounted() {
            try {
                this.agentMarketplaces = await this.$store.dispatch('agents/agentsMarketplace')
            }
            catch (error) {
                helpers.errorHandle(error)
            }
        },
        methods: {
            async onUninstallation(agent) {

                if (confirm('Are you sure to Unistall this Agent: ' + agent.name)) {
                    try {
                        await this.$store.dispatch('agents/uninstall', agent)
                    }
                    catch (error) {
                        helpers.errorHandle(error)
                    }
                }
            },
            async onConfirmInstallation(agent) {

                this.showCommandModal = true
                this.currentAgent = agent
            },
            async install(agent) {
                try {

                    this.showCommandModal = false

                    await this.$store.dispatch('agents/install', agent)
                    alert("The agent was installed")
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            installed(agent) {
                return this.$store.getters['agents/installed'](agent)
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
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>