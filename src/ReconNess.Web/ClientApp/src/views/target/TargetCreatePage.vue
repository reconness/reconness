<template>
  <div>
    <h3>New Target</h3>
    <target-form v-on:save="onSave"></target-form> 
  </div>
</template>

<script>
  import TargetForm from '../../components/target/TargetForm'

  export default {
    name: 'TargetCreatePage',
    components: {
      TargetForm
    },
    methods: {
      async onSave(target) {
        await this.$api.create('targets', target)
        this.$router.push({ name: 'target', params: { targetName: target.name } })
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>