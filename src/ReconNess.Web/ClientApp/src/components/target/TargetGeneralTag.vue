<template>
  <div class="row">
    <div class="pl-4 col-12"><strong>Target: </strong>{{ target.name }}</div>
    <div class="pl-4 col-12"><strong>Root Domain: </strong>{{ target.rootDomain }}</div>
    <div class="pl-4 col-12"><strong>Subdomains: </strong>{{ target.subdomains.length }}</div>
    <div class="pl-4 col-12"><strong>Agents: </strong>{{ agents.length }}</div>
  </div>
</template>

<script>

  export default {
    name: 'TargetGeneralTag', 
    props: {
      target: {
        type: Object,
        required: true
      },
      agents: {
        type: Array,
        required: true
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>