<template>
    <div>
        <h2>Login</h2>
        <div class="form-group">
            <label for="username">Username</label>
            <input type="text" v-model="username" name="username" class="form-control" />
        </div>
        <div class="form-group">
            <label htmlFor="password">Password</label>
            <input type="password" v-model="password" name="password" class="form-control" v-on:keyup.enter="handleSubmit" />
        </div>
        <div class="form-group">
            <button class="btn btn-primary" v-on:click="handleSubmit">Login</button>
        </div>
    </div>
</template>

<script>
    import helpers from '../../helpers'

    export default {
        name: 'LoginPage',
        data() {
            return {
                username: '',
                password: ''
            }
        },
        methods: {
            async handleSubmit() {
                try {
                    const user = await this.$store.dispatch('login/login', { username: this.username, password: this.password })
                    if (user !== null) {
                        localStorage.setItem('user', JSON.stringify(user))
                        this.$router.push({ name: 'home' })
                    }
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