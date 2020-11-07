<template>
    <div class="row">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <div class="pl-4 pt-2 col-12"><strong>Target: </strong>{{ target.name }}</div>
        <div class="pl-4 pt-2 col-12"><strong>Root Domain: </strong>{{ rootDomain.name }}</div>
        <div class="pl-4 pt-2 col-12"><strong>Agents: </strong>{{ agentsCount }}</div>

        <div class="pl-4 pt-4 col-12">
            <button class="mr-2 btn btn-primary" v-on:click="onExportRootDomain()">Export Root Domain</button>
        </div>
    </div>
</template>

<script>

    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import helpers from '../../helpers'
    import { mapState } from 'vuex'

    export default {
        name: 'RootdomainGeneralTag',
        components: {
            Loading
        },
        data: () => {
            return {
                isLoading: false
            }
        },
        computed: mapState({
            target: state => state.targets.currentTarget,
            rootDomain: state => state.rootdomains.currentRootDomain,
            agentsCount: state => state.agents.agents.length
        }),
        methods: {
            async onExportRootDomain() {
                try {
                    this.isLoading = true

                    await this.$store.dispatch('rootdomains/export')
                    this.$alert('Rootdomain was exported', 'Success', 'success')
                }
                catch (error) {
                    helpers.errorHandle(this.$alert, error)
                }

                this.isLoading = false
            }            
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>