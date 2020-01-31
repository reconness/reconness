<template>
  <div>
    <h3>Update Agent</h3>
    <agent-form v-if="agentReady" v-bind:parentAgent="agent" v-on:update="onUpdate" v-on:delete="onDelete"></agent-form>
  </div>
</template>

<script>
  import AgentForm from '../../components/agent/AgentForm'

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
    async mounted() {
      this.initService()
    },
    watch: {
      $route: 'initService'
    },    
    methods: {  
      async initService() {
       this.agentReady = false
        this.agent = (await this.$api.getById('agents', this.$route.params.agentName)).data
        this.agentReady = true
      },
      async onUpdate() {
        await this.$api.update('agents', this.agent.id, this.agent)

        alert("The agent script code was saved")        

        if (this.$route.params.agentName !== this.agent.name) {
          this.$router.push({ name: 'agentEdit', params: { agentName: this.agent.name } })
          // TODO: refresh menu
        }
      },
      async onDelete() {
        if (confirm('Are you sure to delete this agent: ' + this.agent.name)) {          
          await this.$api.delete('agents', this.agent.name)
          this.$router.push({ name: 'home' })
          // TODO: refresh menu
        }
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>