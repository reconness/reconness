<template>
    <div>
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>
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
                    <button class="ml-2 btn btn-danger" v-on:click="onDeleteSubdomains()">Delete Subdomains</button>
                    <button class="ml-2 btn btn-primary" v-on:click="onExportSubdomains()">Download Subdomains</button>
                </div>
            </div>
        </div>

        <hr />
        
        <v-server-table ref="table" :url="'/api/subdomains/GetPaginate/' + $route.params.targetName + '/' + $route.params.rootDomain" :columns="columns" :options="options">
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

                <div v-if="props.row.agentsRanBefore">Agents: <strong>{{ props.row.agentsRanBefore }} </strong></div>
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
        </v-server-table>
    </div>
</template>

<script>

    window.axios = require('axios');

    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';
    
    import helpers from '../../helpers'   

    export default {
        name: 'RootdomainSubdomainsTag',
        components: {
            Loading
        },
        data: () => {
            return {
                isLoading: false,
                newSubdomain: null,
                columns: ['name', 'details', 'labels', 'actions'],
                options: {
                    perPage: 25,
                    perPageValues: [25,50,100],
                    filterable: ['name'],
                    requestFunction(data) {
                        let user = JSON.parse(localStorage.getItem('user'));
                        return axios.get(this.url, {
                            params: data,
                            headers: {
                                'Authorization': 'Bearer ' + user.auth_token,
                                'Content-Type': 'application/json'
                            }
                        }).catch(function (e) {
                            this.dispatch('error', e);
                        });
                    }                    
                }                
            }
        },
        methods: {
            async onDeleteSubdomain(subdomain) {
                if (confirm('Are you sure to delete this subdomain: ' + subdomain.name)) {
                    try {
                        this.isLoading = true
                        await this.$store.dispatch('rootdomains/deleteSubdomain', { subdomain: subdomain })
                        this.$refs.table.refresh()
                    }
                    catch (error) {
                        helpers.errorHandle(error)
                    }

                    this.isLoading = false
                }
            },
            async onDeleteSubdomains() {
                if (confirm('Are you sure to delete all the subdomains')) {
                    try {
                        this.isLoading = true
                        await this.$store.dispatch('rootdomains/deleteSubdomains')
                        this.$refs.table.refresh()
                    }
                    catch (error) {
                        helpers.errorHandle(error)
                    }

                    this.isLoading = false
                }
            },
            async onAddLabel(subdomain, label) {
                try {
                    await this.$store.dispatch('subdomains/updateLabel', { subdomain, label })
                    alert("The Label was added")
                    this.$refs.table.refresh()
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            async onAddNewSubdomain() {
                try {
                    await this.$store.dispatch('rootdomains/createSubdomain', { subdomain: this.newSubdomain })
                    alert("The new Subdomain was added")
                    this.$refs.table.refresh()
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            async handleFileUpload() {
                const formData = new FormData();
                formData.append('file', this.$refs.file.files[0]);
                try {
                    this.isLoading = true
                    await this.$store.dispatch('rootdomains/uploadSubdomains', { formData })
                    alert("subdomains were uploaded")
                    this.$refs.table.refresh()
                }
                catch (error) {
                    helpers.errorHandle(error)
                }

                this.isLoading = false
            },
            async onExportSubdomains() {
                try {
                    this.isLoading = true

                    await this.$store.dispatch('rootdomains/exportSubdomains')
                    alert("subdomains were saved")
                }
                catch (error) {
                    helpers.errorHandle(error)
                }

                this.isLoading = false
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
    .VueTables__search-field {
        background-color: red;
    }
    .VuePagination {
        text-align: center;
    }
</style>