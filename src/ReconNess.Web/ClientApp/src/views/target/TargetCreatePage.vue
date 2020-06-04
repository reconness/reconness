<template>
  <div>
    <h3>New Target</h3>
    <target-form v-on:save="onSave" v-bind:isNew="true"></target-form>
  </div>
</template>

<script>
  import TargetForm from '../../components/target/TargetForm'

  import helpers from '../../helpers'

  export default {
    name: 'TargetCreatePage',
    components: {
      TargetForm
    },
    mounted() {
      this.$store.state.targets.currentTarget = {}
    },
    methods: {
      async onSave(target, rootDomains) {
        try {
          target.rootDomains = rootDomains.map(r => r.name);
          await this.$store.dispatch('targets/createTarget', target)
          this.$router.push({ name: 'target', params: { targetName: target.name } })
        }
        catch(error) {
          helpers.errorHandle(error)
        }
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>