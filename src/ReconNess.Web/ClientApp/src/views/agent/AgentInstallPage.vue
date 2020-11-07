<template>
    <div>
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

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

    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import helpers from '../../helpers'

    export default {
        name: 'AgentInstallPage',
        components: {
            Loading
        },
        data: () => {
            return {
                agentMarketplaces: [],
                currentAgent: null,
                showCommandModal: false,
                isLoading: false
            }
        },
        async mounted() {
            try {
                this.isLoading = true
                this.agentMarketplaces = await this.$store.dispatch('agents/agentsMarketplace')
            }
            catch (error) {
                helpers.errorHandle(this.$alert, error)
            }

            this.isLoading = false
        },
        methods: {
            async onUninstallation(agent) {

                this.$confirm('Are you sure to unistall this agent: ' + agent.name, 'Confirm', 'question').then(async () => {
                    try {
                        this.isLoading = true

                        await this.$store.dispatch('agents/uninstall', agent)
                    }
                    catch (error) {
                        helpers.errorHandle(this.$alert, error)
                    }

                    this.isLoading = false
                })
            },
            async onConfirmInstallation(agent) {

                this.showCommandModal = true
                this.currentAgent = agent
            },
            async install(agent) {
                try {

                    this.isLoading = true

                    this.showCommandModal = false

                    await this.$store.dispatch('agents/install', agent)
                    this.$alert('The agent was installed', 'Success', 'success')
                }
                catch (error) {
                    helpers.errorHandle(this.$alert, error)
                }

                this.isLoading = false
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