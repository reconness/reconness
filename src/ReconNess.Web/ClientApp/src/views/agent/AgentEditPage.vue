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
        this.$store.dispatch('updateAgent', { api: this.$api, agent: this.agent })
          .then(() => {    
            alert("The agent script code was saved") 

            if (this.$route.params.agentName !== this.agent.name) {
              this.$router.push({ name: 'agentEdit', params: { agentName: this.agent.name } })
            }
          })
          .catch(error => {
            if (error) {
              alert(error)
            }
            else {
              alert("The Agent cannot be updated. Try again, please!")
            }
          });        
      },
      async onDelete() {
        if (confirm('Are you sure to delete this Agent: ' + this.agent.name)) {  
          this.$store.dispatch('deleteAgent', { api: this.$api, agent: this.agent })
          .then(() => {             
            this.$router.push({ name: 'home' })
          })
          .catch(error => {
            if (error) {
              alert(error)
            }
            else {
              alert("The Agent cannot be deleted. Try again, please!")
            }
          });
        }
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>