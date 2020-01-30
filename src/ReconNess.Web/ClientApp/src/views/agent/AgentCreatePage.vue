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
        await this.$api.create('agents', agent)
        this.$router.push({ name: 'agentEdit', params: { agentName: agent.name } })
        // TODO: refresh menu
      },
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>