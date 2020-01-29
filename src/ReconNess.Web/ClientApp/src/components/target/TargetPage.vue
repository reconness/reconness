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
          <target-tag-subdomains v-if="subdomainsReady" v-bind:parentSubdomains="target.subdomains"></target-tag-subdomains>
        </div>
        <div class="tab-pane fade" id="nav-agents" role="tabpanel" aria-labelledby="nav-agents-tab">
          <target-tag-agents v-if="agentsReady" v-bind:parentAgents="agents"></target-tag-agents>
        </div>
        <div class="tab-pane fade" id="nav-notes" role="tabpanel" aria-labelledby="nav-notes-tab">
          <target-tag-notes v-if="notesReady" v-bind:parentNotes="target.notes"></target-tag-notes>
        </div>
        <div class="tab-pane fade" id="nav-general" role="tabpanel" aria-labelledby="nav-general-tab">
          <div class="row">
            <div class="pl-4 col-12"><strong>Target: </strong>{{ target.name }}</div>
            <div class="pl-4 col-12"><strong>Root Domain: </strong>{{ target.rootDomain }}</div>
            <div class="pl-4 col-12"><strong>Subdomains: </strong>{{ target.subdomains.length }}</div>
            <div class="pl-4 col-12"><strong>Agents: </strong>{{ agents.length }}</div>
          </div>
        </div>
        <div class="tab-pane fade" id="nav-settings" role="tabpanel" aria-labelledby="nav-settings-tab">
          <target-form v-if="targetReady" v-bind:parentTarget="target"></target-form>
        </div>
      </div>
      <hr/>       
      <router-link to="/">Back</router-link>  
    </div>
</template>

<script>

  
  import TargetTagSubdomains from './ui/TargetTagSubdomains'
  import TargetTagAgents from './ui/TargetTagAgents'
  import TargetTagNotes from './ui/TargetTagNotes'
  import TargetForm from './ui/TargetForm'  

  export default {
    name: 'TargetPage',
    components: {
      TargetTagSubdomains,
      TargetTagAgents,
      TargetTagNotes,
      TargetForm    
    },
    data() {
      return { 
        target: {
          subdomains: [],
          notes: {}
        },
        agents: [],

        targetReady: false,
        agentsReady: false,
        notesReady: false,
        subdomainsReady: false,
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
        this.targetReady = false
        this.notesReady = false
        this.subdomainsReady = false
        this.agentsReady = false

        this.target = (await this.$api.getById('targets', this.$route.params.targetName)).data 
        this.agents = (await this.$api.get('agents/target/' + this.$route.params.targetName)).data

        this.targetReady = true
        this.notesReady = true
        this.subdomainsReady = true
        this.agentsReady = true
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style>
</style>