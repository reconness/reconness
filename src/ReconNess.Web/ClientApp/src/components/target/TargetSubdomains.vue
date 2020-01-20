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
      <div slot="name" slot-scope="props" v-if="props.row.isAlive">
        <a target="_blank" :href="'http://'+props.row.name" class="glyphicon glyphicon-eye-open">{{props.row.name}}</a>        
      </div>
      <div slot="name" slot-scope="props" v-else>
        {{props.row.name}}
      </div>

      <div class="subdomain-details" slot="details" slot-scope="props">
        <font-awesome-icon v-if="props.row.isMainPortal" :icon="['fas', 'home']" fixed-width title="Main Portal" />
        <font-awesome-icon v-if="props.row.isAlive" :icon="['fas', 'heart']" fixed-width title="Alive" />
        <font-awesome-icon v-if="props.row.hasHttpOpen" :icon="['fas', 'book-open']" fixed-width title="HTTP Open" />
        <font-awesome-icon v-if="props.row.takeover" :icon="['fas', 'fire-alt']" fixed-width title="Takeover" />

        <div v-if="props.row.fromAgents">Agents: <strong>{{ props.row.fromAgents }} </strong></div>
        <div v-if="props.row.labels.length > 0">Labels: <strong v-for="l in props.row.labels" v-bind:key="l.name"><span :style="{ color: l.color}">{{ l.name }} </span></strong></div>
        <div v-if="props.row.services.length > 0">Services: <strong>{{props.row.services | joinComma('name') }} </strong></div>
        <div v-if="props.row.ipAddress">IpAddress: <strong>{{props.row.ipAddress }} </strong></div>
        <div>Added: {{props.row.createdAt | formatDate('YYYY-MM-DD')}}</div>
      </div>


      <div class="subdomain-labels" slot="labels" slot-scope="props">
        <button type="button" class="btn btn-link" v-on:click="onAddLabel(props.row, 'Checking')" title="Add Checking Label"> <font-awesome-icon :icon="['fas', 'coffee']" /></button>
        <button type="button" class="btn btn-link" v-on:click="onAddLabel(props.row, 'Vulnerable')" title="Add Vulnerable Label"> <font-awesome-icon :icon="['fas', 'bug']" /></button>
        <button type="button" class="btn btn-link" v-on:click="onAddLabel(props.row, 'Interesting')" title="Add Interesting Label"> <font-awesome-icon :icon="['fas', 'exclamation']" /></button>
        <button type="button" class="btn btn-link" v-on:click="onAddLabel(props.row, 'Bounty')" title="Add Bounty Label"> <font-awesome-icon :icon="['fas', 'dollar-sign']" /></button>
        <button type="button" class="btn btn-link" v-on:click="onAddLabel(props.row, 'Ignore')" title="Add Ignore Label"> <font-awesome-icon :icon="['fas', 'guitar']" /></button>
      </div>
      <div class="subdomain-actions" slot="actions" slot-scope="props">
        <button type="button" class="btn btn-link" v-on:click="onOpenSubdomain(props.row)" title="Open"><font-awesome-icon :icon="['fas', 'arrow-alt-circle-right']" fixed-width /></button>
        <button type="button" class="btn btn-link" v-on:click="onDeleteSubdomain(props.row)" title="Delete"> <font-awesome-icon :icon="['fas', 'trash-alt']" fixed-width /></button>
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
        targetName: '',
        columns: ['name', 'details', 'labels', 'actions'],
        options: {
          headings: {
            name: 'Subdomain',
            details: 'Details',
            labels: 'Labels',
            actions: 'Actions'            
          },
          sortable: ['name'],
          filterable: ['name']
        }
      }
    },
    async mounted() {
      this.subdomains = this.parentSubdomains || []
      this.targetName = this.$route.params.targetName
    },
    methods: {
      async onOpenSubdomain(subdomain) {
        this.$router.push({name: 'subdomain', params: { targetName: this.$route.params.targetName, subdomain: subdomain.name }})
      },
      async onDeleteSubdomain(subdomain) {
        if (confirm('Are you sure to delete this subdomain: ' + subdomain.name)) {          
          await this.$api.delete('subdomains/' + this.$route.params.targetName, subdomain.id)
          this.subdomains = this.subdomains.filter(sub => sub !== subdomain)
        }
      },
      async onDeleteAllSubdomains() {
        if (confirm('Are you sure to delete all the subdomains')) {          
          await this.$api.delete('targets/subdomain', this.$route.params.targetName)
          this.subdomains = []
        }
      },
      async onAddLabel(subdomain, label) {
        await this.$api.update('subdomains/label', subdomain.id, { label: label })
        this.$router.go()
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
  .subdomain-details {
    width: 250px;
}
  .subdomain-actions {
    width: 50px;
}
  .subdomain-labels {
    width: 120px;
}
</style>