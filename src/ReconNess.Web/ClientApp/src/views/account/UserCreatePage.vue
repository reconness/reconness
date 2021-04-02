<template>
    <div class="pt-2">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h1>Add New User</h1>
        <user-form v-bind:isNew="true" v-on:save="onSave"></user-form>
    </div>
</template>

<script>
    // Import component
    import Loading from 'vue-loading-overlay';

    // Import stylesheet
    import UserForm from '../../components/user/UserForm'

    import helpers from '../../helpers'

    export default {
        name: 'UserCreatePage',
        components: {
            Loading,
            UserForm
        },
        data: () => {
            return {
                isLoading: false
            }
        },
        
        mounted() {
            this.$store.state.accounts.currentUser = {}
        },
        methods: {
            async onSave(user) {
                try {
                    this.isLoading = true
                    await this.$store.dispatch('accounts/createUser', user)

                    this.$router.push({ name: 'user' })
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