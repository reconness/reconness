<template>
  <div>
    <h2>Login</h2>
      <div class="form-group">
        <label for="username">Username</label>
        <input type="text" v-model="username" name="username" class="form-control" />
      </div>
      <div class="form-group">
        <label htmlFor="password">Password</label>
        <input type="password" v-model="password" name="password" class="form-control"  />
      </div>
      <div class="form-group">
        <button class="btn btn-primary" v-on:click="handleSubmit()">Login</button>
      </div>
  </div>
</template>

<script>

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
          const user = (await this.$api.login(this.username, this.password)).data
          if (user !== null) {
            localStorage.setItem('user', JSON.stringify(user))
            this.$router.push({ name: 'home' })
          }
        }
        catch (ex) {
          alert(ex)
        }
      }
    }  
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>