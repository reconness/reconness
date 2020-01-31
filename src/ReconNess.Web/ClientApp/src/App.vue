<template>
  <div id="app">
    <nav-menu v-if="isAuth()"/>
    <div class="content">

      <div class="container">
        <router-view></router-view>
      </div>
    </div>

    <footer class="footer py-4 bg-dark text-white-50">
      <div class="container text-center">
        <small>Copyright &copy; ReconNess 2020</small>
      </div>
    </footer>

  </div>
</template>

<script>
import NavMenu from './components/NavMenu.vue'

export default {
  name: 'app',
  components: {
    NavMenu
  },
  methods: {
    isAuth() {
      const loggedIn = localStorage.getItem('user')
      return loggedIn !== null
    }
  }
}
</script>

<style>
  html, body {
  height: 100%;
}

#app {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.content {
  flex: 1 0 auto;
  /* Prevent Chrome, Opera, and Safari from letting these items shrink to smaller than their content's default minimum size. */
  padding: 20px;
}

.footer {
  flex-shrink: 0;
  /* Prevent Chrome, Opera, and Safari from letting these items shrink to smaller than their content's default minimum size. */
  padding: 20px;
}

* {
  box-sizing: border-box;
}

body {
  margin: 0;
  font: 16px Sans-Serif;
}

h1 {
  margin: 0 0 20px 0;
}

p {
  margin: 0 0 20px 0;
}
</style>
