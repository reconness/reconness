<template>
    <div>
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h2 class="text-right">{{ target.name }}</h2>
        <target-form v-on:update="onUpdate" v-on:delete="onDelete"></target-form>
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
    import TargetForm from '../../components/target/TargetForm'

    export default {
        name: 'TargetPage',
        components: {
            TargetForm,
            Loading
        },
        data: () => {
            return {
                isLoading: false
            }
        },
        computed: mapState({
            target: state => state.targets.currentTarget
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
                }
                catch (error) {
                    helpers.errorHandle(error)
                }

                this.isLoading = false
            },
            async onUpdate() {
                try {
                    this.isLoading = true

                    await this.$store.dispatch('targets/updateTarget')

                    alert("The target was updated")

                    if (this.$route.params.targetName !== this.target.name) {
                        this.$router.push({ name: 'target', params: { targetName: this.target.name } })
                    }
                }
                catch (error) {
                    helpers.errorHandle(error)
                }

                this.isLoading = false
            },
            async onDelete() {
                if (confirm('Are you sure to delete this Target with all the subdomains and services: ' + this.target.name)) {
                    try {
                        this.isLoading = true
                        await this.$store.dispatch('targets/deleteTarget')
                        this.$router.push({ name: 'home' })
                    }
                    catch (error) {
                        helpers.errorHandle(error)
                    }

                    this.isLoading = false
                }
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style>
</style>