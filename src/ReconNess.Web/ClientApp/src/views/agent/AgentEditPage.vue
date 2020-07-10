<template>
    <div>
        <h3>Update Agent</h3>
        <agent-form v-on:update="onUpdate" v-on:delete="onDelete"></agent-form>
    </div>
</template>

<script>

    import AgentForm from '../../components/agent/AgentForm'
    import { mapState } from 'vuex'
    import helpers from '../../helpers'

    export default {
        name: 'AgentEditPage',
        components: {
            AgentForm
        },
        computed: mapState({
            agent: state => state.agents.currentAgent
        }),
        async mounted() {
            await this.initService()
        },
        watch: {
            $route: 'initService' // to watch the url change
        },
        methods: {
            async initService() {
                try {
                    await this.$store.dispatch('agents/agent', this.$route.params.agentName)
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            async onUpdate() {
                try {
                    await this.$store.dispatch('agents/updateAgent')
                    alert("The agent script code was saved")

                    if (this.$route.params.agentName !== this.agent.name) {
                        this.$router.push({ name: 'agentEdit', params: { agentName: this.agent.name } })
                    }
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            async onDelete() {
                if (confirm('Are you sure to delete this Agent: ' + this.agent.name)) {
                    try {
                        await this.$store.dispatch('agents/deleteAgent')
                        this.$router.push({ name: 'home' })
                    }
                    catch (error) {
                        helpers.errorHandle(error)
                    }
                }
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>