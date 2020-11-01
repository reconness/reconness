<template>
    <div class="row">
        <div class="pl-4 pt-2 col-12"><strong>Target: </strong>{{ target.name }}</div>
        <div class="pl-4 pt-2 col-12"><strong>Root Domain: </strong>{{ rootDomain.name }}</div>
        <div class="pl-4 pt-2 col-12"><strong>Agents: </strong>{{ agentsCount }}</div>
        
        <div class="pl-4 pt-4 col-12">
            <button class="mr-2 btn btn-primary" v-on:click="onExportRootDomain()">Export Root Domain</button>
        </div>
    </div>
</template>

<script>

    import helpers from '../../helpers'
    import { mapState } from 'vuex'

    export default {
        name: 'RootdomainGeneralTag',
        computed: mapState({
            target: state => state.targets.currentTarget,
            rootDomain: state => state.rootdomains.currentRootDomain,
            agentsCount: state => state.agents.agents.length
        }),
        methods: {
            async onExportRootDomain() {
                try {
                    await this.$store.dispatch('rootdomains/export')
                    alert("Root Domain was exported")
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
</style>