<template>
    <div class="pt-2">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h1>Add New User</h1>
        
    </div>
</template>

<script>
    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import helpers from '../../helpers'

    export default {
        name: 'UserCreatePage',
        components: {
            Loading
        },
        data: () => {
            return {
                notification: { url: '', method: '', payload: '' },
                isLoading: false
            }
        },
        async mounted() {
            this.isLoading = true
            const notif = await this.$store.dispatch('accounts/notification')
            if (notif !== '') {
                this.notification = notif
            }
            this.isLoading = false
        },
        methods: {
            async onSave() {
                try {
                    this.isLoading = true

                    await this.$store.dispatch('accounts/saveNotification', this.notification)
                    this.$alert('The notifications were saved', 'Success', 'success')
                }
                catch (error) {
                    helpers.errorHandle(this.$alert, error)
                }

                this.isLoading = false
            },
            isValid() {
                return this.notification.url && this.notification.method && this.notification.payload
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>