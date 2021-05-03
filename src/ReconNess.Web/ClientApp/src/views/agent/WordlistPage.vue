<template>
    <div>
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

            <nav>
                <div class="nav nav-tabs" id="nav-tab" role="tablist">
                    <a class="nav-item nav-link active" id="nav-subdomain-tab" data-toggle="tab" href="#nav-subdomain" role="tab" aria-controls="nav-subdomain" aria-selected="true">Subdomains Enum</a>
                    <a class="nav-item nav-link" id="nav-directories-tab" data-toggle="tab" href="#nav-directories" role="tab" aria-controls="nav-directories" aria-selected="false">Directories Enum</a>
                    <a class="nav-item nav-link" id="nav-resolvers-tab" data-toggle="tab" href="#nav-resolvers" role="tab" aria-controls="nav-resolvers" aria-selected="false">DNS Resolvers</a>
                </div>
            </nav>
            <div class="tab-content" id="nav-tabContent">
                <div class="tab-pane fade show active" id="nav-subdomain" role="tabpanel" aria-labelledby="nav-subdomain-tab">
                    <div class="col-12 pb-4 mt-4">
                        <input type="file" id="file_subdomain" ref="file_subdomain" v-on:change="handleSubdomainFileUpload()" />
                        <label class="custom-file-label" for="file_subdomain">Add Subdomain Enum File</label>
                    </div>

                    <table class="table table-striped">
                        <thead class="thead-dark">
                            <tr>
                                <th scope="col">Filename</th>
                                <th scope="col">Count of Words</th>
                                <th scope="col">Size</th>
                                <th scope="col">Path</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="subdomain in subdomains" v-bind:key="subdomain.filename">
                                <th class="w-25" scope="row">{{ subdomain.filename }}</th>
                                <th class="w-25" scope="row">{{ subdomain.count }}</th>
                                <th class="w-25" scope="row">{{ subdomain.size }}</th>
                                <th class="w-25" scope="row">{{ subdomain.path }}</th>
                                <td class="w-25">
                                    <router-link class="btn btn-primary mb-2" :to="{name: 'wordlistEdit', params: { type: 'subdomain_enum', filename: subdomain.filename }}">Edit</router-link>
                                    <button class="btn btn-danger mb-2" v-on:click="onDeleteSubdomain(subdomain.filename)">Delete</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="tab-pane fade" id="nav-directories" role="tabpanel" aria-labelledby="nav-directories-tab">
                    <div class="col-12 pb-4 mt-4">
                        <input type="file" id="file_directory" ref="file_directory" v-on:change="handleDirectoryFileUpload()" />
                        <label class="custom-file-label" for="file_directory">Add Directory Enum File</label>
                    </div>

                    <table class="table table-striped">
                        <thead class="thead-dark">
                            <tr>
                                <th scope="col">Filename</th>
                                <th scope="col">Count of Words</th>
                                <th scope="col">Size</th>
                                <th scope="col">Path</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="directory in directories" v-bind:key="directory.filename">
                                <th class="w-25" scope="row">{{ directory.filename }}</th>
                                <th class="w-25" scope="row">{{ directory.count }}</th>
                                <th class="w-25" scope="row">{{ directory.size }}</th>
                                <th class="w-25" scope="row">{{ directory.path }}</th>
                                <td class="w-25">
                                    <router-link class="btn btn-primary mb-2" :to="{name: 'wordlistEdit', params: { type: 'dir_enum', filename: directory.filename }}">Edit</router-link>
                                    <button class="btn btn-danger mb-2" v-on:click="onDeleteDirectory(directory.filename)">Delete</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="tab-pane fade" id="nav-resolvers" role="tabpanel" aria-labelledby="nav-resolvers-tab">
                    <div class="col-12 pb-4 mt-4">
                        <input type="file" id="file_resolver" ref="file_resolver" v-on:change="handleResolverFileUpload()" />
                        <label class="custom-file-label" for="file_resolver">Add DNS Resolver File</label>
                    </div>

                    <table class="table table-striped">
                        <thead class="thead-dark">
                            <tr>
                                <th scope="col">Filename</th>
                                <th scope="col">Count of Words</th>
                                <th scope="col">Size</th>
                                <th scope="col">Path</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="resolver in resolvers" v-bind:key="resolver.filename">
                                <th class="w-25" scope="row">{{ resolver.filename }}</th>
                                <th class="w-25" scope="row">{{ resolver.count }}</th>
                                <th class="w-25" scope="row">{{ resolver.size }}</th>
                                <th class="w-25" scope="row">{{ resolver.path }}</th>
                                <td class="w-25">
                                    <div>
                                        <router-link class="btn btn-primary mb-2" :to="{name: 'wordlistEdit', params: { type: 'dns_resolver_enum', filename: resolver.filename }}">Edit</router-link>
                                        <button class="btn btn-danger mb-2" v-on:click="onDeleteResolver(resolver.filename)">Delete</button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
    </div>
</template>

<script>

    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import helpers from '../../helpers'

    export default {
        name: 'WordlistPage',
        components: {
            Loading
        },
        data: () => {
            return {
                isLoading: false,
                subdomains: [],
                directories: [],
                resolvers: []
            }
        },
        async mounted() {
            this.isLoading = true

            var wordlists = await this.$store.dispatch('wordlists/get')

            this.subdomains = wordlists.subdomainsEnum
            this.directories = wordlists.directoriesEnum
            this.resolvers = wordlists.dnsResolvers

            this.isLoading = false
        },
        methods: {       
            async onDelete(type, filename) {
                this.$confirm('Are you sure to delete ' + filename + '?', 'Confirm', 'question').then(async () => {
                    this.isLoading = true
                    try {
                        await this.$store.dispatch('wordlists/delete', { type: type, filename: filename })                         
                        this.$alert('The file was deleted', 'Success', 'success')

                        if (type === 'subdomain_enum') {
                            this.subdomains = this.subdomains.filter(s => s.filename !== filename)
                        }
                        else if (type === 'dir_enum') {
                            this.directories = this.directories.filter(s => s.filename !== filename)
                        }
                        else if (type === 'dns_resolver_enum') {
                            this.resolvers = this.resolvers.filter(s => s.filename !== filename)
                        }
                    }
                    catch (error) {
                        helpers.errorHandle(this.$alert, error)
                    }

                    this.isLoading = false
                })
            },
            async onDeleteSubdomain(filename) {
                await this.onDelete('subdomain_enum', filename)
            },
            async onDeleteDirectory(filename) {
                await this.onDelete('dir_enum', filename)
            },
            async onDeleteResolver(filename) {
                await this.onDelete('dns_resolver_enum', filename)
            },
            async handleFileUpload(type, formData) {
                
                try {
                    this.isLoading = true
                    await this.$store.dispatch('wordlists/upload', { type: type, formData: formData })
                    this.$alert('The file were uploaded', 'Success', 'success')
                }
                catch (error) {
                    helpers.errorHandle(this.$alert, error)
                }

                this.isLoading = false
            },
            async handleSubdomainFileUpload() {                
                const formData = new FormData();
                formData.append('file', this.$refs.file_subdomain.files[0]);
                await this.handleFileUpload('subdomain_enum', formData)
            },
            async handleDirectoryFileUpload() {
                const formData = new FormData();
                formData.append('file', this.$refs.file_directory.files[0]);
                await this.handleFileUpload('dir_enum', formData)
            },
            async handleResolverFileUpload() {
                const formData = new FormData();
                formData.append('file', this.$refs.file_resolver.files[0]);
                await this.handleFileUpload('dns_resolver_enum', formData)
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>