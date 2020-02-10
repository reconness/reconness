<template>
  <div>
    <h3>Update Agent</h3>
    <agent-form v-if="agentReady" v-bind:agent="agent" v-on:update="onUpdate" v-on:delete="onDelete"></agent-form>
  </div>
</template>

<script>
  import AgentForm from '../../components/agent/AgentForm'

  import helpers from '../../helpers'

  export default {
    name: 'AgentEditPage',
    components: {
      AgentForm
    },
    data() {
      return {
        agent: {},
        agentReady: false
      }
    },
    async mounted () {
     await this.initService()
    },
    watch: {
      $route: 'initService' // to watch the url change
    },
    methods: {
      async initService() {
        this.agentReady = false
        try {
          this.agent = await this.$store.dispatch('agents/agent', this.$route.params.agentName)
          this.agentReady = true
        }
        catch (error) {
          helpers.errorHandle(error)
        }
      },
      async onUpdate() {
        try {
          await this.$store.dispatch('agents/updateAgent', this.agent)
          alert("The agent script code was saved")

          if (this.$route.params.agentName !== this.agent.name) {
            this.$router.push({ name: 'agentEdit', params: { agentName: this.agent.name } })
          }
        }
        catch(error) {
          helpers.errorHandle(error)
        }
      },
      async onDelete() {
        if (confirm('Are you sure to delete this Agent: ' + this.agent.name)) {
          try {
            await this.$store.dispatch('agents/deleteAgent', this.agent)
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