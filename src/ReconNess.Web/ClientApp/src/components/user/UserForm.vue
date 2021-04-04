<template>
    <div class="pt-2">       

        <div class="form-group">
            <label for="userName">UserName</label>
            <input name="userName" formControlName="userName" class="form-control" id="userName" v-model="user.userName">
        </div>
        <div class="form-group">
            <label for="email">Email</label>
            <input name="email" formControlName="email" class="form-control" id="email" v-model="user.email">
        </div>
        <div class="form-group">
            <label for="firstName">First Name</label>
            <input name="firstName" formControlName="firstName" class="form-control" id="firstName" v-model="user.firstName">
        </div>
        <div class="form-group">
            <label for="lastName">Last Name</label>
            <input name="lastName" formControlName="lastName" class="form-control" id="lastName" v-model="user.lastName">
        </div>
        <div class="form-group" v-if="!isNew">
            <label for="currentPassword">Current Password</label>
            <input type="password" name="currentPassword" formControlName="currentPassword" class="form-control" id="currentPassword" v-model="user.currentPassword">
        </div>
        <div class="form-group">
            <label for="newPassword">New Password</label>
            <input type="password" name="newPassword" formControlName="newPassword" class="form-control" id="newPassword" v-model="user.newPassword">
        </div>
        <div class="form-group">
            <label for="confirmationPassword">Confirmation Password</label>
            <input type="password" name="confirmationPassword" formControlName="confirmationPassword" class="form-control" id="confirmationPassword" v-model="user.confirmationPassword">
        </div>
        <div class="form-group">
            <label for="inputName">Role</label>
            <select id="method" class="form-control" v-model="user.role" :disabled='isMember()'>
                <option v-for="role in roles" v-bind:value="role" v-bind:key="role">
                    {{ role }}
                </option>
            </select>
        </div>
        <div class="form-group">
            <button class="btn btn-primary" v-if="isNew" v-on:click="$emit('save', user)" :disabled='!isValid()'>Add</button>
            <button class="mr-2 mt-2 btn btn-primary" v-if="!isNew" v-on:click="$emit('update')" :disabled='!isValid()'>Update</button>
            <button class="mt-2 btn btn-danger" v-if="!isNew" v-on:click="$emit('delete')">Delete</button>
        </div>
    </div>
</template>

<script>
    
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import { mapState } from 'vuex'

    export default {
        name: 'UserForm',
        data: () => {
            return {                
                roles: []
            }
        },
        props: {
            isNew: {
                type: Boolean,
                default: false
            }
        },
        computed: mapState({
            user: state => state.accounts.currentUser
        }),
        async mounted() {
            if (this.isNew) {
                this.$store.state.accounts.currentUser = { }
            }

            this.roles = await this.$store.dispatch('accounts/roles')
        },
        methods: { 
            isValid() {
                return this.user.userName && this.user.email && this.user.role
            },
            isMember() {
                var currentUser = JSON.parse(localStorage.getItem('user'))
                return currentUser.roles === "Member"
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>