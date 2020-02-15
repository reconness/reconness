<template>
  <div>
    <h3>New Agent</h3>
    <agent-form  v-bind:isNew="true" v-on:save="onSave"></agent-form>
  </div>
</template>

<script>
  import AgentForm from '../../components/agent/AgentForm'

  import helpers from '../../helpers'

  export default {
    name: 'AgentCreatePage',
    components: {
      AgentForm
    },
    methods: {
      async onSave(agent) {
        try {
          await this.$store.dispatch('agents/createAgent', agent)
          this.$router.push({ name: 'agentEdit', params: { agentName: agent.name } })
        }
        catch(error) {
          helpers.errorHandle(error)
        }
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>