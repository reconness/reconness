<template>
    <div class="pt-2 row">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <div class="mx-auto"><strong>List of Users</strong></div>
        
        <div class="col-12 pt-2 pb-2" v-if="canAddNewUser()">
            <router-link class="btn btn-primary " :to="{name: 'userCreate'}">Add New User</router-link>
        </div>

        <div class="col-12">
            <table class="table table-striped">
                <thead class="thead-dark">
                    <tr>
                        <th scope="col">UserName</th>
                        <th scope="col">Email</th>
                        <th scope="col">Role</th>
                        <th scope="col">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="user in users" v-bind:key="user.id">
                        <th class="w-25" scope="row">{{ user.userName }}</th>
                        <th class="w-25" scope="row">{{ user.email }}</th>
                        <th class="w-25" scope="row">{{ user.role }}</th>
                        <td class="w-25">
                            <router-link class="btn btn-primary ml-2" :to="{name: 'userDetails', params: { id: user.id }}" v-if="canEdit(user)">Edit</router-link>
                            <button class="btn btn-danger ml-2" v-on:click="onDelete(user)" v-if="canDelete(user)">Delete</button>
                            <button class="btn btn-warning ml-2" v-on:click="onOwner(user)" v-if="canOwner(user)">Owner</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import helpers from '../../helpers'

    export default {
        name: 'UserListPage',
        components: {
            Loading
        },
        data: () => {
            return {
                users: [],
                isLoading: false
            }
        },
        async mounted() {
            this.isLoading = true
            this.users = await this.$store.dispatch('accounts/users')           
            this.isLoading = false
        },
        methods: {           
            async onDelete(user) {
                this.$confirm('Are you sure to delete this user: ' + user.userName, 'Confirm', 'question').then(async () => {
                    this.isLoading = true
                    try {
                        await this.$store.dispatch('accounts/deleteUser', user)
                        this.users = this.users.filter((u) => {
                            return u.userName !== user.userName;
                        })
                    }
                    catch (error) {
                        helpers.errorHandle(this.$alert, error)
                    }

                    this.isLoading = false
                })
            },
            async onOwner(user) {
                this.$confirm('Are you sure to assign this user: ' + user.userName + ' as the new Owner, you will be remove as Owner and sign out.', 'Confirm', 'question').then(async () => {
                    this.isLoading = true
                    try {
                        await this.$store.dispatch('accounts/assignOwner', user)

                        localStorage.removeItem('user');
                        this.$router.push({ name: 'login' })
                    }
                    catch (error) {
                        helpers.errorHandle(this.$alert, error)
                    }

                    this.isLoading = false
                })
            },
            canAddNewUser() {
                var currentUser = JSON.parse(localStorage.getItem('user'))
                return currentUser.owner || currentUser.roles[0] === "Admin"
            },
            canEdit(user) {
                var currentUser = JSON.parse(localStorage.getItem('user'))
                return currentUser.owner || (currentUser.roles[0] === "Admin" && user.role === "Member") || currentUser.userName === user.userName 
            },
            canDelete(user) {
                var currentUser = JSON.parse(localStorage.getItem('user'))
                var isNotMe = currentUser.userName !== user.userName
                return (currentUser.owner && isNotMe) || (currentUser.roles[0] === "Admin" && user.role === "Member")
            },
            canOwner(user) {
                var currentUser = JSON.parse(localStorage.getItem('user'))
                var isNotMe = currentUser.userName !== user.userName
                return currentUser.owner && isNotMe && user.role === "Admin"
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>