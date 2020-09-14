<template>
    <div>
        <div class="pt-2">
            <div class="col-12">
                <input type="file" id="file" ref="file" v-on:change="handleFileUpload()" />
                <label class="custom-file-label" for="file">Load Subdomains</label>
            </div>
            <hr />
            <div class="form-row align-items-center">
                <div class="col-6">
                    <input name="newSubdomain" formControlName="newSubdomain" class="form-control" id="newSubdomain" v-model="newSubdomain" placeholder="New Subdomain">
                </div>
                <div class="col-6">
                    <button class="btn btn-primary" v-on:click="onAddNewSubdomain()">Add</button>
                    <button class="ml-2 btn btn-danger" v-on:click="onDeleteAllSubdomains()" :disabled="subdomains.length === 0">Delete Subdomains</button>
                    <button class="ml-2 btn btn-primary" v-on:click="onExportSubdomains()">Download Subdomains</button>
                </div>
            </div>
        </div>

        <hr />

        <div class="mb-2 form-row align-items-center">
            <div class="col-6">
                <input class="form-control" id="filter" v-model="filter" placeholder="Query Filter" v-on:keyup.enter="filterGrid" />
            </div>
            <div class="col-6">
                <button class="ml-2  btn btn-primary" v-on:click="filterGrid()">Filter</button>
            </div>
        </div>

        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" v-on:click="filterOnlyScopeGrid()">
            <label class="form-check-label">
                Show only <strong class="text-primary">Scope</strong> subdomains
            </label>
        </div>
        <v-client-table :columns="columns" :data="subdomains" :options="options">
            <div slot="name" slot-scope="props" v-if="props.row.isAlive">
                <a target="_blank" :href="'http://'+props.row.name" class="glyphicon glyphicon-eye-open">{{props.row.name}}</a>
            </div>
            <div slot="name" slot-scope="props" v-else>
                {{props.row.name}}
            </div>

            <div class="subdomain-details" slot="details" slot-scope="props">
                <font-awesome-icon v-if="props.row.takeover" :icon="['fas', 'fire-alt']" fixed-width title="Takeover" />
                <font-awesome-icon v-if="props.row.isMainPortal" :icon="['fas', 'home']" fixed-width title="Main Portal" />
                <font-awesome-icon v-if="props.row.isAlive" :icon="['fas', 'heart']" fixed-width title="Alive" />
                <font-awesome-icon v-if="props.row.hasHttpOpen" :icon="['fas', 'book-open']" fixed-width title="HTTP Open" />

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
                <button type="button" class="btn btn-link" v-on:click="onAddLabel(props.row, 'Scope')" title="Add Scope Label"> <font-awesome-icon :icon="['fas', 'microscope']" /></button>
            </div>
            <div class="subdomain-actions" slot="actions" slot-scope="props">
                <router-link class="btn btn-link" :to="{name: 'subdomain', params: { targetName: $route.params.targetName, rootDomain: $route.params.rootDomain, subdomain: props.row.name}}" target="_blank"><font-awesome-icon :icon="['fas', 'arrow-alt-circle-right']" fixed-width /></router-link>
                <button type="button" class="btn btn-link" v-on:click="onDeleteSubdomain(props.row)" title="Delete"> <font-awesome-icon :icon="['fas', 'trash-alt']" fixed-width /></button>
            </div>
        </v-client-table>
    </div>
</template>

<script>

    import { mapState } from 'vuex'
    import helpers from '../../helpers'
    import { Event } from 'vue-tables-2';

    export default {
        name: 'RootdomainSubdomainsTag',
        data: () => {
            return {
                filter: '',
                filterOnlyScope: false,
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
                    filterable: false,
                    customFilters: [{
                        name: 'search',
                        callback: function (row, query) {
                            const nameFilter = row.name.indexOf(query) > -1
                            const labelFilter = row.labels.length > 0 && row.labels.some(l => l.name.indexOf(query) > -1)
                            const serviceFilter = row.services.length > 0 && row.services.some(s => s.name.indexOf(query) > -1)
                            const ipAddressFilter = row.ipAddress !== undefined && row.ipAddress !== null && row.ipAddress.indexOf(query) > -1
                            const agentsFilter = row.fromAgents !== undefined && row.fromAgents !== null && row.fromAgents.indexOf(query) > -1

                            return nameFilter || labelFilter || serviceFilter || ipAddressFilter || agentsFilter;
                        }
                    }]
                }
            }
        },
        computed: mapState({
            subdomains: state => state.rootdomains.currentRootDomain.subdomains
        }),
        async mounted() {
            this.targetName = this.$route.params.targetName
            this.rootDomain = this.$route.params.rootDomain
        },
        methods: {
            async onDeleteSubdomain(subdomain) {
                if (confirm('Are you sure to delete this subdomain: ' + subdomain.name)) {
                    try {
                        await this.$store.dispatch('subdomains/deleteSubdomain', { subdomain: subdomain })
                    }
                    catch (error) {
                        helpers.errorHandle(error)
                    }
                }
            },
            async onDeleteAllSubdomains() {
                if (confirm('Are you sure to delete all the subdomains')) {
                    try {
                        await this.$store.dispatch('rootdomains/deleteAllSubdomains')
                    }
                    catch (error) {
                        helpers.errorHandle(error)
                    }
                }
            },
            async onAddLabel(subdomain, label) {
                try {
                    await this.$store.dispatch('subdomains/updateLabel', { subdomain, label })
                    alert("The Label was added")
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            async onAddNewSubdomain() {
                try {
                    await this.$store.dispatch('rootdomains/createSubdomain', { subdomain: this.newSubdomain })
                    alert("The new Subdomain was added")
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            async handleFileUpload() {
                const formData = new FormData();
                formData.append('file', this.$refs.file.files[0]);
                try {
                    await this.$store.dispatch('rootdomains/uploadSubdomains', { formData })
                    alert("subdomains were uploaded")
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            filterGrid() {
                Event.$emit('vue-tables.filter::search', this.filter);
            },
            filterOnlyScopeGrid() {
                this.filterOnlyScope = !this.filterOnlyScope
                this.filter = this.filterOnlyScope ? "Scope" : ""

                this.filterGrid();
            },
            async onExportSubdomains() {
                try {
                    await this.$store.dispatch('rootdomains/exportSubdomains')
                    alert("subdomains were saved")
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
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