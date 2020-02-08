<template>
  <div>
    <h3>New Agent</h3>
    <agent-form v-on:save="onSave"></agent-form>
  </div>
</template>

<script>
  import AgentForm from '../../components/agent/AgentForm'

  export default {
    name: 'AgentCreatePage',
    components: {
      AgentForm
    }, 
    methods: {
      async onSave(agent) {
        this.$store.dispatch('createAgent', { api: this.$api, agent: agent })
          .then(() => {
            this.$router.push({ name: 'agentEdit', params: { agentName: agent.name } })
          })
          .catch(error => {
            if (error) {
              alert(error)
            } else {
              alert("The Agent cannot be added. Try again, please!")
            }
          })
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>