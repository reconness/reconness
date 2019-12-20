<template>
  <div>
    <h3>Update Agent</h3>
    <agent-form v-if="agentReady" v-bind:parentAgent="agent"></agent-form>
  </div>
</template>

<script>
  import AgentForm from './AgentForm'

  export default {
    name: 'Agent',
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
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>