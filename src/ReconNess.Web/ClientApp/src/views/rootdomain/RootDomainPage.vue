<template>
    <div>
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h2 class="text-right"> <router-link :to="{name: 'target', params: { targetName: target.name }}">{{ target.name }}</router-link> | {{ rootDomain.name }}</h2>
        <nav>
            <div class="nav nav-tabs" id="nav-tab" role="tablist">
                <a class="nav-item nav-link active" id="nav-subdomains-tab" data-toggle="tab" href="#nav-subdomains" role="tab" aria-controls="nav-subdomains" aria-selected="true">Subdomains</a>
                <a class="nav-item nav-link" id="nav-agents-tab" data-toggle="tab" href="#nav-agents" role="tab" aria-controls="nav-agents" aria-selected="false">Agents</a>
                <a class="nav-item nav-link" id="nav-notes-tab" data-toggle="tab" href="#nav-notes" role="tab" aria-controls="nav-notes" aria-selected="false">Notes</a>
                <a class="nav-item nav-link" id="nav-general-tab" data-toggle="tab" href="#nav-general" role="tab" aria-controls="nav-general" aria-selected="false">General</a>
            </div>
        </nav>
        <div class="tab-content" id="nav-tabContent">
            <div class="tab-pane fade show active" id="nav-subdomains" role="tabpanel" aria-labelledby="nav-subdomains-tab">
                <rootdomain-subdomains-tag v-if="rootDomain.subdomains !== undefined"></rootdomain-subdomains-tag>
            </div>
            <div class="tab-pane fade" id="nav-agents" role="tabpanel" aria-labelledby="nav-agents-tab">
                <agent-tag v-bind:isTarget="true"></agent-tag>
            </div>
            <div class="tab-pane fade" id="nav-notes" role="tabpanel" aria-labelledby="nav-notes-tab">
                <notes-tag v-bind:isRootDomain="true"></notes-tag>
            </div>
            <div class="tab-pane fade" id="nav-general" role="tabpanel" aria-labelledby="nav-general-tab">
                <rootdomain-general-tag v-if="rootDomain.subdomains !== undefined"></rootdomain-general-tag>
            </div>
        </div>
        <hr />
        <router-link to="/">Back</router-link>
    </div>
</template>

<script>

    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import { mapState } from 'vuex'

    import helpers from '../../helpers'

    import RootdomainSubdomainsTag from '../../components/rootdomain/RootdomainSubdomainsTag'
    import AgentTag from '../../components/agent/AgentTag'
    import NotesTag from '../../components/NotesTag'
    import RootdomainGeneralTag from '../../components/rootdomain/RootdomainGeneralTag'

    export default {
        name: 'RootDomainPage',
        data: () => {
            return {
                isLoading: false
            }
        },
        components: {
            RootdomainSubdomainsTag,
            AgentTag,
            NotesTag,
            RootdomainGeneralTag,
            Loading
        },
        computed: mapState({
            agents: state => state.agents.agents,
            target: state => state.targets.currentTarget,
            rootDomain: state => state.rootdomains.currentRootDomain
        }),
        async mounted() {
            await this.initService()
        },
        watch: {
            $route: 'initService' // to watch the url change
        },
        methods: {
            async initService() {
                try {
                    this.isLoading = true

                    await this.$store.dispatch('targets/target', this.$route.params.targetName)
                    await this.$store.dispatch('rootdomains/rootDomain', { targetName: this.$route.params.targetName, rootDomain: this.$route.params.rootDomain })
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
<style>
</style>