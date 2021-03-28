<template>
    <div class="pt-2">

        <h1>Add New User</h1>
        <user-form v-bind:isNew="true" v-on:save="onSave"></user-form>
    </div>
</template>

<script>
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';
    import UserForm from '../../components/user/UserForm'

    import helpers from '../../helpers'

    export default {
        name: 'UserCreatePage',
        components: {
            UserForm
        },
       
        mounted() {
            this.$store.state.accounts.currentUser = {}
        },
        methods: {
            async onSave(user) {
                try {
                    await this.$store.dispatch('accounts/createUser', user)
                    this.$router.push({ name: 'user' })
                }
                catch (error) {
                    helpers.errorHandle(this.$alert, error)
                }
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>