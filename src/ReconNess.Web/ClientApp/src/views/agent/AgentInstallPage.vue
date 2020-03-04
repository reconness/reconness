<template>
  <div>
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
          <tr v-for="agent in agentDefaults" v-bind:key="agent.name">
            <th class="w-25" scope="row">{{ agent.name }}</th>
            <td class="w-25">{{ agent.category}}</td>
            <td class="w-25">
              <button class="btn btn-primary ml-2">Install</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>

  import helpers from '../../helpers'

  export default {
    name: 'AgentInstallPage', 
    data: () => {
      return {      
        agentDefaults: []
      }
    },
    async mounted() {         
      try {        
        this.agentDefaults = await this.$store.dispatch('agents/agentsDefault')   
      }
      catch (error) {
        helpers.errorHandle(error)
      }
    },
    methods: {  
      isValid() {
        return this.output && this.content
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>