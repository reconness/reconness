<template>
    <div>
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h2 class="text-right">
            <router-link :to="{name: 'target', params: { targetName: targetName }}">{{ targetName }}</router-link> | <router-link :to="{name: 'targetRootDomain', params: { targetName: targetName, rootDomain: rootDomain }}">{{ rootDomain }}</router-link> |
            <a :href="'http://'+subdomain.name" target="blank" v-if="subdomain.isAlive === true">{{ subdomain.name }}</a><span v-else>{{ subdomain.name }}</span>
        </h2>
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
                <subdomain-dashboard-tag></subdomain-dashboard-tag>
            </div>
            <div class="tab-pane fade" id="nav-agents" role="tabpanel" aria-labelledby="nav-agents-tab">
                <agent-tag></agent-tag>
            </div>
            <div class="tab-pane fade" id="nav-services" role="tabpanel" aria-labelledby="nav-services-tab">
                <subdomain-services-tag></subdomain-services-tag>
            </div>
            <div class="tab-pane fade" id="nav-directories" role="tabpanel" aria-labelledby="nav-directories-tab">
                <div class="pt-2" v-if="subdomain.serviceHttp === undefined || subdomain.serviceHttp === null">We don't have directories enumerated yet</div>
                <subdomain-directories-tag></subdomain-directories-tag>
            </div>
            <div class="tab-pane fade" id="nav-notes" role="tabpanel" aria-labelledby="nav-notes-tab">
                <notes-tag></notes-tag>
            </div>
        </div>
        <hr />
        <router-link :to="{name: 'target', params: { targetName: targetName }}">Back</router-link>
    </div>
</template>

<script>

    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import { mapGetters, mapState } from 'vuex'

    import helpers from '../../helpers'

    import SubdomainDashboardTag from '../../components/subdomain/SubdomainDashboardTag'
    import SubdomainDirectoriesTag from '../../components/subdomain/SubdomainDirectoriesTag'
    import SubdomainServicesTag from '../../components/subdomain/SubdomainServicesTag'

    import NotesTag from '../../components/NotesTag'
    import AgentTag from '../../components/agent/AgentTag'

    export default {
        name: 'SubdomainPage',
        components: {
            SubdomainDashboardTag,
            SubdomainServicesTag,
            AgentTag,
            SubdomainDirectoriesTag,
            NotesTag,
            Loading
        },
        data: () => {
            return {
                isLoading: false
            }
        },
        computed: {
            targetName() {
                return this.$route.params.targetName
            },
            rootDomain() {
                return this.$route.params.rootDomain
            },
            // mix the getters into computed with object spread operator
            ...mapGetters({
                agents: 'agents/subdomainAgents'
            }),
            ...mapState({
                subdomain: state => state.subdomains.currentSubdomain
            })
        },
        async mounted() {
            this.initService()
        },
        watch: {
            $route: 'initService'
        },
        methods: {
            async initService() {
                try {
                    this.isLoading = true

                    await this.$store.dispatch('targets/target', this.$route.params.targetName)
                    await this.$store.dispatch('rootdomains/rootDomain', { targetName: this.$route.params.targetName, rootDomain: this.$route.params.rootDomain })
                    await this.$store.dispatch('subdomains/subdomain', { targetName: this.targetName, rootDomain: this.rootDomain, subdomain: this.$route.params.subdomain })
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
</style>