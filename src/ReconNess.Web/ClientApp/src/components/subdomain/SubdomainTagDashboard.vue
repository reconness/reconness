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
</template>

<script>

  import VueTagsInput from '@johmun/vue-tags-input';

  export default {
    name: 'SubdomainTagDashboard', 
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
      this.autocompleteItems = (await this.$api.get('labels')).data.map(label => {
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

        await this.$api.update('subdomains', this.subdomain.id, this.subdomain)
        alert("The subdomain was updated")
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