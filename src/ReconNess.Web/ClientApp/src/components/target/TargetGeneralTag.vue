<template>
    <div class="row">
        <div class="pl-4 pt-2 col-12"><strong>Target: </strong>{{ target.name }}</div>
        <div class="pl-4 pt-2 col-12"><strong>Root Domain: </strong>{{ rootDomain.name }}</div>
        <div class="pl-4 pt-2 col-12"><strong>Subdomains: </strong>{{ subdomainsCount }}</div>
        <div class="pl-4 pt-2 col-12"><strong>Agents: </strong>{{ agentsCount }}</div>

        <div class="pl-4 pt-2">
            <div class="col-12">
                <input type="file" id="fileRootdomain" ref="fileRootdomain" v-on:change="handleFileUploadRootDomain()" />
                <label class="custom-file-label" for="fileRootdomain">Load Root Domain</label>
            </div>
        </div>
        <div class="pl-4 pt-4 col-12">
            <button class="mr-2 btn btn-primary" v-on:click="onExportRootDomain()">Save Root Domain</button>
        </div>
    </div>
</template>

<script>

    import helpers from '../../helpers'
    import { mapState } from 'vuex'

    export default {
        name: 'TargetGeneralTag',
        computed: mapState({
            target: state => state.targets.currentTarget,
            rootDomain: state => state.targets.currentRootDomain,
            subdomainsCount: state => state.targets.currentRootDomain.subdomains.length,
            agentsCount: state => state.agents.agents.length
        }),
        methods: {
            async onExportRootDomain() {
                try {
                    await this.$store.dispatch('targets/export')
                    alert("Root Domain was saved")
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            async handleFileUploadRootDomain() {
                const formData = new FormData();
                formData.append('file', this.$refs.fileRootdomain.files[0]);
                try {
                    await this.$store.dispatch('targets/upload', { formData })
                    alert("Root Domain was uploaded")
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