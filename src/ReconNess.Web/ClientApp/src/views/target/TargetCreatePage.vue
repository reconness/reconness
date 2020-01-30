<template>
  <div>
    <h3>New Target</h3>
    <target-form ></target-form> 
  </div>
</template>

<script>
  import TargetForm from '../../components/target/TargetForm'

  export default {
    name: 'TargetCreatePage',
    components: {
      TargetForm
    }  
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>