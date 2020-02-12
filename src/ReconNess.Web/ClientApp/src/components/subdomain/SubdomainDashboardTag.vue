<template>
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
    <div class="form-group" v-if="hasScreenshots()">
      <strong>Screenshots: </strong>
      <div v-if="subdomain.serviceHttp.screenshotHttpPNGBase64">
        <p>HTTP</p>
        <img :src="'data:image/png;base64, '+ subdomain.serviceHttp.screenshotHttpPNGBase64" />
      </div>
      <div v-if="subdomain.serviceHttp.screenshotHttpsPNGBase64">
        <p>HTTPS</p>
        <img :src="'data:image/png;base64, '+ subdomain.serviceHttp.screenshotHttpsPNGBase64" />
      </div>
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
      <button class="mt-2 ml-2 btn btn-danger" v-on:click="onDelete()">Delete</button>
    </div>
  </div>
</template>

<script>

  import VueTagsInput from '@johmun/vue-tags-input';
  import helpers from '../../helpers'

  export default {
    name: 'SubdomainDashboardTag', 
    components: {
      VueTagsInput
    },
    props: {
      subdomain: {
        type: Object,
        required: true
      }
    },
    data() {
      return { 
        tag: '',
        tags: [],
        autocompleteItems: [],
      }    
    },    
    async mounted() {
      const labels = await this.$store.dispatch('subdomains/labels')
      this.autocompleteItems = labels.map(label => {
        return { text: label.name };
      })

      this.tags = this.subdomain.labels.map(label => {
        return { text: label.name };
      })
    },
    computed: {
      filteredItems() {
        return this.autocompleteItems.filter(i => {
          return i.text.toLowerCase().indexOf(this.tag.toLowerCase()) !== -1;
        });
      },
     },
    methods: {
      async onUpdate() {
        this.subdomain.labels = this.tags.map(tag => {
          return { 'name': tag.text }
        })
        try {
          await this.$store.dispatch('subdomains/updateSubdomain', this.subdomain)
          alert("The subdomain was updated")
        }
        catch (error) {
          helpers.errorHandle(error)
        }
      },
      async onDelete() {
        if (confirm('Are you sure to delete this subdomain: ' + this.subdomain.name)) { 
          try {
            await this.$store.dispatch('subdomains/deleteSubdomain', this.subdomain)
            this.$router.push({ name: 'target', params: { targetName: this.$route.params.targetName } })
          }
          catch (error) {
            helpers.errorHandle(error)
          }
        }
      },
      hasScreenshots() {
        return this.subdomain.serviceHttp && (this.subdomain.serviceHttp.screenshotHttpPNGBase64 || this.subdomain.serviceHttp.screenshotHttpsPNGBase64)
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
  .modal-dialog {
    max-width: 800px !important;
  }
</style>