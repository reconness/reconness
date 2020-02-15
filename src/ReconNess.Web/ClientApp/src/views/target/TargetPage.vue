<template>
  <div>
    <h2 class="text-right">{{ target.name }}</h2>
    <nav>
      <div class="nav nav-tabs" id="nav-tab" role="tablist">
        <a class="nav-item nav-link active" id="nav-subdomains-tab" data-toggle="tab" href="#nav-subdomains" role="tab" aria-controls="nav-subdomains" aria-selected="true">Subdomains</a>
        <a class="nav-item nav-link" id="nav-agents-tab" data-toggle="tab" href="#nav-agents" role="tab" aria-controls="nav-agents" aria-selected="false">Agents</a>
        <a class="nav-item nav-link" id="nav-notes-tab" data-toggle="tab" href="#nav-notes" role="tab" aria-controls="nav-notes" aria-selected="false">Notes</a>
        <a class="nav-item nav-link" id="nav-general-tab" data-toggle="tab" href="#nav-general" role="tab" aria-controls="nav-general" aria-selected="false">General</a>
        <a class="nav-item nav-link" id="nav-settings-tab" data-toggle="tab" href="#nav-settings" role="tab" aria-controls="nav-settings" aria-selected="false">Settings</a>
      </div>
    </nav>
    <div class="tab-content" id="nav-tabContent">
      <div class="tab-pane fade show active" id="nav-subdomains" role="tabpanel" aria-labelledby="nav-subdomains-tab">
        <target-subdomains-tag></target-subdomains-tag>
      </div>
      <div class="tab-pane fade" id="nav-agents" role="tabpanel" aria-labelledby="nav-agents-tab">
        <agent-tag v-bind:isTarget="true"></agent-tag>
      </div>
      <div class="tab-pane fade" id="nav-notes" role="tabpanel" aria-labelledby="nav-notes-tab">
        <notes-tag v-bind:isTarget="true"></notes-tag>
      </div>
      <div class="tab-pane fade" id="nav-general" role="tabpanel" aria-labelledby="nav-general-tab">
        <target-general-tag></target-general-tag>
      </div>
      <div class="tab-pane fade" id="nav-settings" role="tabpanel" aria-labelledby="nav-settings-tab">
        <target-form v-on:update="onUpdate" v-on:delete="onDelete"></target-form>
      </div>
    </div>
    <hr />
    <router-link to="/">Back</router-link>
  </div>
</template>

<script>

  import { mapState } from 'vuex'

  import helpers from '../../helpers'

  import TargetSubdomainsTag from '../../components/target/TargetSubdomainsTag'
  import AgentTag from '../../components/agent/AgentTag'
  import NotesTag from '../../components/NotesTag'
  import TargetGeneralTag from '../../components/target/TargetGeneralTag'
  import TargetForm from '../../components/target/TargetForm' 

  export default {
    name: 'TargetPage',
    components: {
      TargetSubdomainsTag,
      AgentTag,
      NotesTag,
      TargetGeneralTag,
      TargetForm
    },
    computed: mapState({
      agents: state => state.agents.agents,
      target: state => state.targets.currentTarget
    }),  
    async mounted () {
     await this.initService()
    },
    watch: {
      $route: 'initService' // to watch the url change
    },
    methods: {   
      async initService() {
        try {
          await this.$store.dispatch('targets/target', this.$route.params.targetName)
        }
        catch(error) {
          helpers.errorHandle(error)
        }
      },
      async onUpdate() {
        try {
          await this.$store.dispatch('targets/updateTarget')

          alert("The target was updated")

          if (this.$route.params.targetName !== this.target.name) {
            this.$router.push({ name: 'target', params: { targetName: this.target.name } })
          }
        }
        catch(error) {
          helpers.errorHandle(error)
        }
      },
      async onDelete() {
        if (confirm('Are you sure to delete this Target with all the subdomains and services: ' + this.target.name)) {
          try {
            await this.$store.dispatch('targets/deleteTarget')
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
<style>

</style>