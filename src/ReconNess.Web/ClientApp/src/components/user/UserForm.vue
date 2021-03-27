<template>
    <div class="pt-2">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

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
        <div class="form-group">
            <button class="btn btn-primary" v-if="isNew" v-on:click="$emit('save', user)" :disabled='!isValid()'>Add</button>
            <button class="mr-2 mt-2 btn btn-primary" v-if="!isNew" v-on:click="$emit('update')" :disabled='!isValid()'>Update</button>
            <button class="mt-2 btn btn-danger" v-if="!isNew" v-on:click="$emit('delete')">Delete</button>
        </div>
    </div>
</template>

<script>
    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import { mapState } from 'vuex'

    import helpers from '../../helpers'

    export default {
        name: 'UserForm',
        components: {
            Loading
        },
        data: () => {
            return {
                isLoading: false
            }
        },
        props: {
            isNew: {
                type: Boolean,
                default: false
            }
        },
        computed: mapState({
            user: state => state.users.currentUser
        }),
        mounted() {
            if (this.isNew) {
                this.$store.state.users.currentUser = { }
            }
        },
        methods: { 
            isValid() {
                return this.user.userName && this.user.email
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>