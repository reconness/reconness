<template>
  <div>
    <div class="jumbotron">
      <h1 class="text-center">Welcome <strong>{{username}}</strong>!!</h1>
      <hr class="my-4">
      <p>A Web App Tool to run and keep all your #recon in the same place and allow you to query your targets in an user friendly way.</p>
      <p class="lead">
        <a class="btn btn-info btn-lg" href="https://docs.reconness.com" target="_blank">Learn more</a>
      </p>
    </div>

    <h3>List of Targets</h3>
    <ul>
      <li v-for="t in targets" v-bind:key="t.id">
        <router-link :to="{name: 'target', params: { targetName: t.name }}">{{ t.name }}</router-link>
      </li>
    </ul>

    <hr />
    <reference-and-resource></reference-and-resource>
  </div>
</template>

<script>
  import ReferenceAndResource from './ui/ReferenceAndResource';

  export default {
    name: 'HomePage',
    components: {
      ReferenceAndResource,
    },
    data: () => {
      return {
        targets: [],
        username: ''
      }
    },
    async mounted() {
      this.targets = (await this.$api.get('targets')).data
      this.username = JSON.parse(localStorage.getItem('user')).userName;
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>