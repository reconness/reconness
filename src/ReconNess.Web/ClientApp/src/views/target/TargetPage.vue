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
          <target-subdomains-tag v-if="isReady" v-bind:subdomains="target.subdomains"></target-subdomains-tag>
        </div>
        <div class="tab-pane fade" id="nav-agents" role="tabpanel" aria-labelledby="nav-agents-tab">
          <agent-tag v-if="isReady" v-bind:agents="agents"></agent-tag>
        </div>
        <div class="tab-pane fade" id="nav-notes" role="tabpanel" aria-labelledby="nav-notes-tab">
          <notes-tag v-if="isReady" v-bind:notes="target.notes"></notes-tag>
        </div>
        <div class="tab-pane fade" id="nav-general" role="tabpanel" aria-labelledby="nav-general-tab">
          <target-general-tag v-if="isReady" v-bind:target="target" v-bind:agents="agents"></target-general-tag>
        </div>
        <div class="tab-pane fade" id="nav-settings" role="tabpanel" aria-labelledby="nav-settings-tab">
          <target-form v-if="isReady" v-bind:target="target" v-on:update="onUpdate" v-on:delete="onDelete"></target-form>
        </div>
      </div>
      <hr/>       
      <router-link to="/">Back</router-link>  
    </div>
</template>

<script>

  
  import TargetSubdomainsTag from '../../components/target/TargetSubdomainsTag'  
  import TargetGeneralTag from '../../components/target/TargetGeneralTag'
  import TargetForm from '../../components/target/TargetForm'  

  import NotesTag from '../../components/NotesTag'
  import AgentTag from '../../components/agent/AgentTag'

  export default {
    name: 'TargetPage',
    components: {
      TargetSubdomainsTag,
      AgentTag,
      NotesTag,
      TargetGeneralTag,
      TargetForm    
    },
    data() {
      return { 
        target: {
          subdomains: [],
          notes: {}
        },
        agents: [],

        isReady: false
      }
    },
    async mounted () {
     this.initService()
    },
    watch: {
      $route: 'initService'
    },
    methods: {  
      async initService() {
        this.isReady = false

        this.target = (await this.$api.getById('targets', this.$route.params.targetName)).data 
        this.agents = (await this.$api.get('agents/target/' + this.$route.params.targetName)).data

        this.target.notes = this.target.notes || {}

        this.isReady = true
      },
      async onUpdate() {
        this.$store.dispatch('updateTarget', { api: this.$api, target: this.target })
          .then(() => {             
            if (this.$route.params.targetName !== this.target.name) {
              this.$router.push({ name: 'target', params: { targetName: this.target.name } })
            } else {
              alert("The target was updated")
            }
          })
          .catch(error => {
            if (error) {
              alert(error)
            }
            else {
              alert("The Target cannot be updated. Try again, please!")
            }
          });        
      },
      async onDelete() {
        if (confirm('Are you sure to delete this Target with all the subdomains and services: ' + this.target.name)) { 
          this.$store.dispatch('deleteTarget', { api: this.$api, target: this.target })
          .then(() => {             
            this.$router.push({ name: 'home' })
          })
          .catch(error => {
            if (error) {
              alert(error)
            }
            else {
              alert("The Target cannot be deleted. Try again, please!")
            }
          });
        }
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style>
</style>