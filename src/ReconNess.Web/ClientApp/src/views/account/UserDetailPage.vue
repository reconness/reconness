<template>
    <div class="pt-2">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h1>User Details</h1>

        <user-form v-on:update="onUpdate" v-on:delete="onDelete"></user-form>

    </div>
</template>

<script>
    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import { mapState } from 'vuex'
    import UserForm from '../../components/user/UserForm'
    import helpers from '../../helpers'

    export default {
        name: 'UserDetailPage',
        components: {
            UserForm,
            Loading
        },
        computed: mapState({
            user: state => state.accounts.currentUser
        }),
        data: () => {
            return {
                isLoading: false
            }
        },
        async mounted() {
            try {
                this.isLoading = true
                await this.$store.dispatch('accounts/user', this.$route.params.id)

                this.user.currentPassword = ""
                this.user.newPassword = ""
                this.user.confirmationPassword = ""
            }
            catch (error) {
                helpers.errorHandle(this.$alert, error)
            }

            this.isLoading = false
        },
        methods: {
            async onUpdate() {
                try {
                    this.isLoading = true
                    await this.$store.dispatch('accounts/updateUser', this.user)                    

                    this.$alert('Please if you changed your username or your role, sign out and sign in again', 'Success', 'success')
                }
                catch (error) {                    
                    helpers.errorHandle(this.$alert, error)
                } 

                this.isLoading = false
            },
            async onDelete() {
                this.$confirm('Are you sure to delete this user: ' + this.user.userName, 'Confirm', 'question').then(async () => {
                    
                    try {
                        this.isLoading = true
                        await this.$store.dispatch('accounts/deleteUser', this.user)

                        this.$router.push({ name: 'user' })
                    }
                    catch (error) {                        
                        helpers.errorHandle(this.$alert, error)
                    }  

                    this.isLoading = false
                })
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>