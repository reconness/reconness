<template>
  <div>
    <div class="pt-2">
      <div class="col-12">
        <input type="file" id="file" ref="file" v-on:change="handleFileUpload()" />
        <label class="custom-file-label" for="file">Import Subdomains...</label>
      </div>
      <hr />
      <div class="form-row align-items-center">
        <div class="col-6">
          <input name="newSubdomain" formControlName="newSubdomain" class="form-control" id="newSubdomain" v-model="newSubdomain" placeholder="New Subdomain">
        </div>
        <div class="col-6">
          <button class="btn btn-primary" v-on:click="onAddNewSubdomain()">Add</button>
          <button class="ml-2 btn btn-danger" v-on:click="onDeleteAllSubdomains()" :disabled="subdomains.length === 0">Delete All subdomains</button>
        </div>
      </div>
    </div>

    <hr />
    <v-client-table :columns="columns" :data="subdomains" :options="options">
      <a slot="name" slot-scope="props" v-if="props.row.isAlive === true" target="_blank" :href="'http://'+props.row.name" class="glyphicon glyphicon-eye-open">{{props.row.name}}</a>
      <div slot="name" slot-scope="props" v-else>{{props.row.name}}</div>
      <div slot="isAlive" slot-scope="props">{{props.row.isAlive}}</div>
      <div slot="hasHttpOpen" slot-scope="props">{{props.row.hasHttpOpen}}</div>
      <div slot="actions" slot-scope="props">
        <button class="btn btn-primary ml-2" v-on:click="onOpenSubdomain(props.row)">Open</button>
        <button class="btn btn-danger ml-2" v-on:click="onDeleteSubdomain(props.row)">Delete</button>
      </div>
    </v-client-table>   
  </div>
</template>

<script>

  export default {
    name: 'TargetSubdomains',
    props: {
      parentSubdomains: {
        type: Array,
        required: true
      }
    },
    data: () => {
      return {
        subdomains: [],
        newSubdomain: null,
        columns: ['name', 'ipAddress', 'isAlive', 'hasHttpOpen', 'actions'],
        options: {
          headings: {
            name: 'Subdomain',
            ipAddress: 'Ip Address',
            isAlive: 'Is Alive',
            hasHttpOpen: 'Has Http Open',
            actions: 'Actions'
          },
          sortable: ['name', 'isAlive'],
          filterable: ['name', 'isAlive']
        }
      }
    },
    async mounted() {
      this.subdomains = this.parentSubdomains || []
    },
    methods: {
      async onOpenSubdomain(subdomain) {
        this.$router.push({name: 'subdomain', params: { targetName: this.$route.params.targetName, subdomain: subdomain.name }})
      },
      async onDeleteSubdomain(subdomain) {
        if (confirm('Are you sure to delete this subdomain: ' + subdomain.name)) {          
          await this.$api.delete('subdomains', subdomain.id)
          this.subdomains = this.subdomains.filter(sub => sub !== subdomain)
        }
      },
      async onDeleteAllSubdomains() {
        if (confirm('Are you sure to delete all the subdomains')) {          
          await this.$api.delete('targets/subdomain', this.$route.params.targetName)
          this.subdomains = []
        }
      },
      async onAddNewSubdomain() {
        const target = this.$route.params.targetName
        try {
          var response = await this.$api.create('subdomains', { target: target, name: this.newSubdomain })
          this.subdomains.push(response.data)
        }
        catch (e) {
          alert(e.response.data.title)
        }
      },
      async handleFileUpload() {
        const formData = new FormData();
        formData.append('file', this.$refs.file.files[0]);
        await this.$api.upload('targets/subdomain', this.$route.params.targetName, formData)
        
        alert("subdomains were uploaded")
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>