<template>
  <div>
    <h3>New Agent</h3>
    <agent-form v-bind:parentAgent="newAgent"></agent-form>
  </div>
</template>

<script>
  import AgentForm from './AgentForm'

  export default {
    name: 'AgentCreate',
    components: {
      AgentForm
    },    
    data: () => {
      return {      
        newAgent: {},
      }
    }  
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>