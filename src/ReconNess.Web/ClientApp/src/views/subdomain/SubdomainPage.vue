<template>
  <div>
    <h2 class="text-right" v-if="subdomain.isAlive === true"><a :href="'http://'+subdomain.name" target="blank">{{ subdomain.name }}</a></h2>
    <h2 class="text-right" v-else>{{ subdomain.name }}</h2>
    <nav>
      <div class="nav nav-tabs" id="nav-tab" role="tablist">
        <a class="nav-item nav-link active" id="nav-details-tab" data-toggle="tab" href="#nav-details" role="tab" aria-controls="nav-details" aria-selected="true">Dashboard</a>
        <a class="nav-item nav-link" id="nav-agents-tab" data-toggle="tab" href="#nav-agents" role="tab" aria-controls="nav-agents" aria-selected="false">Agents</a>
        <a class="nav-item nav-link" id="nav-services-tab" data-toggle="tab" href="#nav-services" role="tab" aria-controls="nav-services" aria-selected="false">Services</a>
        <a class="nav-item nav-link" id="nav-directories-tab" data-toggle="tab" href="#nav-directories" role="tab" aria-controls="nav-directories" aria-selected="false">Directories</a>
        <a class="nav-item nav-link" id="nav-notes-tab" data-toggle="tab" href="#nav-notes" role="tab" aria-controls="nav-notes" aria-selected="false">Notes</a>
      </div>
    </nav>
    <div class="tab-content" id="nav-tabContent">
      <div class="tab-pane fade show active" id="nav-details" role="tabpanel" aria-labelledby="nav-details-tab">
        <subdomain-tag-dashboard v-if="isReady" v-bind:subdomain="subdomain"></subdomain-tag-dashboard>
      </div>
      <div class="tab-pane fade" id="nav-agents" role="tabpanel" aria-labelledby="nav-agents-tab">
        <subdomain-tag-agents v-if="isReady" v-bind:agents="agents" v-bind:subdomain="subdomain"></subdomain-tag-agents>
      </div>
      <div class="tab-pane fade" id="nav-services" role="tabpanel" aria-labelledby="nav-services-tab">
        <subdomain-tag-services v-if="isReady" v-bind:services="subdomain.services"></subdomain-tag-services>
      </div>
      <div class="tab-pane fade" id="nav-directories" role="tabpanel" aria-labelledby="nav-directories-tab">
        <div class="pt-2" v-if="subdomain.serviceHttp === undefined || subdomain.serviceHttp === null">We don't have directories enumerated yet</div>
        <subdomain-tag-directories v-if="isReady && subdomain.serviceHttp" v-bind:directories="subdomain.serviceHttp.directories"></subdomain-tag-directories>
      </div>
      <div class="tab-pane fade" id="nav-notes" role="tabpanel" aria-labelledby="nav-notes-tab">
        <subdomain-tag-notes v-if="isReady" v-bind:notes="subdomain.notes"></subdomain-tag-notes>
      </div>
    </div>
    <hr />
    <router-link :to="{name: 'target', params: { targetName: targetName }}">Back</router-link>
  </div>
</template>

<script>

  import SubdomainTagDashboard from '../../components/subdomain/SubdomainTagDashboard'
  import SubdomainTagNotes from '../../components/subdomain/SubdomainTagNotes'
  import SubdomainTagServices from '../../components/subdomain/SubdomainTagServices'
  import SubdomainTagAgents from '../../components/subdomain/SubdomainTagAgents'
  import SubdomainTagDirectories from '../../components/subdomain/SubdomainTagDirectories'

  export default {
    name: 'SubdomainPage',
    components: {
      SubdomainTagDashboard,      
      SubdomainTagServices,
      SubdomainTagAgents,
      SubdomainTagDirectories,
      SubdomainTagNotes
    },
    data() {
      return { 
        targetName: this.$route.params.targetName,
        subdomain: {},
        isReady: false,        
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

        this.isReady = false

        this.subdomain = (await this.$api.get('subdomains/' + this.targetName + '/' + this.$route.params.subdomain)).data 
        this.agents = (await this.$api.get('agents/subdomain/' + this.targetName + '/' + this.$route.params.subdomain)).data

        this.subdomain.notes = this.subdomain.notes || {}

        this.isReady = true
      }      
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>