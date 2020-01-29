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
        <div class="pt-2">
          <div class="form-group" v-if="subdomain.isAlive === true">
            <strong>Subdomain: </strong><a :href="'http://'+subdomain.name" target="blank">{{ subdomain.name }}</a>
          </div>
          <div v-else>
            <strong>Subdomain: </strong>{{ subdomain.name }}
          </div>
          <div class="form-group">
            <strong>Ip: </strong>{{ subdomain.ipAddress }}
          </div>
          <div class="form-group">
            <strong>Is Alive: </strong>{{ subdomain.isAlive }}
          </div>
          <div class="form-group">
            <strong>Has HTTP Open: </strong>{{ subdomain.hasHttpOpen }}
          </div>
          <div class="form-group">
            <strong>Subdomain Takeover: </strong>{{ subdomain.takeover }}
          </div>
          <div class="form-group">
            <strong>Agents: </strong>{{ subdomain.fromAgents }}
          </div>
          <div class="form-group" v-if="subdomain.serviceHttp !== undefined && subdomain.serviceHttp !== null && (subdomain.serviceHttp.ScreenshotHttpPNGBase64 !== null || subdomain.serviceHttp.ScreenshotHttpsPNGBase64 !== null)">
            <strong>Screenshots: </strong>
            <p>HTTP</p>
            <img :src="'data:image/png;base64, '+ subdomain.serviceHttp.ScreenshotHttpPNGBase64" />
            <p>HTTPS</p>
            <img :src="'data:image/png;base64, '+ subdomain.serviceHttp.ScreenshotHttpsPNGBase64" />
          </div>

          <div class="form-group">
            <label for="inputLabel"><strong>Labels:</strong></label>
            <vue-tags-input v-model="tag" placeholder="Add label" :tags="tags" :autocomplete-items="filteredItems" @tags-changed="newTags => tags = newTags" />
          </div>
          <div class="pl-3 form-group">
            <input class="form-check-input" type="checkbox" id="isMainPortal" v-model="subdomain.isMainPortal">
            <label class="form-check-label" for="isMainPortal">
              Is Main Portal
            </label>
          </div>
          <div class="form-group">
            <button class="mt-2 btn btn-primary" v-on:click="onUpdate()">Update</button>
          </div>
        </div>

      </div>
      <div class="tab-pane fade" id="nav-agents" role="tabpanel" aria-labelledby="nav-agents-tab">
        <subdomain-tag-agents v-if="agentsReady" v-bind:parentAgents="agents" v-bind:subdomain="subdomain"></subdomain-tag-agents>
      </div>
      <div class="tab-pane fade" id="nav-services" role="tabpanel" aria-labelledby="nav-services-tab">
        <subdomain-tag-services v-if="servicesReady" v-bind:parentServices="subdomain.services"></subdomain-tag-services>
      </div>
      <div class="tab-pane fade" id="nav-directories" role="tabpanel" aria-labelledby="nav-directories-tab">
        <div class="pt-2" v-if="subdomain.serviceHttp === undefined || subdomain.serviceHttp === null">We don't have directories enumerated yet</div>
        <subdomain-tag-directories v-if="directoriesReady && subdomain.serviceHttp" v-bind:parentDirectories="subdomain.serviceHttp.directories"></subdomain-tag-directories>
      </div>
      <div class="tab-pane fade" id="nav-notes" role="tabpanel" aria-labelledby="nav-notes-tab">
        <subdomain-tag-notes v-if="notesReady" v-bind:parentNotes="subdomain.notes"></subdomain-tag-notes>
      </div>
    </div>
    <hr />
    <router-link :to="{name: 'target', params: { targetName: targetName }}">Back</router-link>
  </div>
</template>

<script>
  import VueTagsInput from '@johmun/vue-tags-input';

  import SubdomainTagNotes from './ui/SubdomainTagNotes'
  import SubdomainTagServices from './ui/SubdomainTagServices'
  import SubdomainTagAgents from './ui/SubdomainTagAgents'
  import SubdomainTagDirectories from './ui/SubdomainTagDirectories'

  export default {
    name: 'SubdomainPage',
    components: {
      SubdomainTagNotes,
      SubdomainTagServices,
      SubdomainTagAgents,
      SubdomainTagDirectories,
      VueTagsInput
    },
    data() {
      return { 
        targetName: this.$route.params.targetName,
        subdomain: {},
        notesReady: false,
        servicesReady: false,
        directoriesReady: false,
        agentsReady: false,

        tag: '',
        tags: [],
        autocompleteItems: [],
      }
    },
    computed: {
      filteredItems() {
        return this.autocompleteItems.filter(i => {
          return i.text.toLowerCase().indexOf(this.tag.toLowerCase()) !== -1;
        });
      },
     },
    async mounted() {
      this.initService()
    },
    watch: {
      $route: 'initService'
    },  
    methods: {  
      async initService() {
        this.autocompleteItems = (await this.$api.get('labels')).data.map(label => {
          return { text: label.name };
        })

        this.notesReady = false
        this.servicesReady = false
        this.directoriesReady = false
        this.agentsReady = false

        this.subdomain = (await this.$api.get('subdomains/' + this.$route.params.targetName + '/' + this.$route.params.subdomain)).data 
        this.agents = (await this.$api.get('agents/subdomain/' + this.$route.params.targetName + '/' + this.$route.params.subdomain)).data

        this.tags = this.subdomain.labels.map(label => {
          return { text: label.name };
        })

        this.notesReady = true
        this.servicesReady = true
        this.directoriesReady = true
        this.agentsReady = true
      },
      async onUpdate() {
        this.subdomain.labels = this.tags.map(tag => {
          return { 'name': tag.text }
        })

        await this.$api.update('subdomains', this.subdomain.id, this.subdomain)
        alert("The subdomain was updated")
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>