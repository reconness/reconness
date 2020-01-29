<template>
  <div>
    <h3>New Target</h3>
    <target-form v-bind:parentTarget="newTarget"></target-form> 
  </div>
</template>

<script>
  import TargetForm from './ui/TargetForm'

  export default {
    name: 'TargetCreatePage',
    components: {
      TargetForm
    },
    data: () => {
      return {
        newTarget: {}
      }
    }    
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>