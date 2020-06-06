<template>
  <div>
    <h2 class="text-right">{{ target.name }}</h2>
    <target-form v-on:update="onUpdate" v-on:delete="onDelete"></target-form>
    <hr />
    <router-link to="/">Back</router-link>
  </div>
</template>

<script>

  import { mapState } from 'vuex'

  import helpers from '../../helpers'
  import TargetForm from '../../components/target/TargetForm' 

  export default {
    name: 'TargetPage',
    components: {
      TargetForm
    },
    computed: mapState({
      target: state => state.targets.currentTarget
    }),  
    async mounted () {
     await this.initService()
    },
    watch: {
      $route: 'initService' // to watch the url change
    },
    methods: {   
      async initService() {
        try {
          await this.$store.dispatch('targets/target', this.$route.params.targetName)
        }
        catch(error) {
          helpers.errorHandle(error)
        }
      },
      async onUpdate() {
        try {
          await this.$store.dispatch('targets/updateTarget')

          alert("The target was updated")

          if (this.$route.params.targetName !== this.target.name) {
            this.$router.push({ name: 'target', params: { targetName: this.target.name } })
          }
        }
        catch(error) {
          helpers.errorHandle(error)
        }
      },
      async onDelete() {
        if (confirm('Are you sure to delete this Target with all the subdomains and services: ' + this.target.name)) {
          try {
            await this.$store.dispatch('targets/deleteTarget')
            this.$router.push({ name: 'home' })
          }
          catch (error) {
            helpers.errorHandle(error)
          }
        }
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style>

</style>