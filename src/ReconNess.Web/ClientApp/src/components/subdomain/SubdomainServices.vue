<template>
  <div>   
    <div class="pt-2">
      <table class="table table-striped">
        <thead class="thead-dark">
          <tr>
            <th scope="col">Service</th>
            <th scope="col">Port</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="service in services" v-bind:key="service.id">
            <th class="w-25" scope="row">{{ service.name }}</th>
            <th class="w-25" scope="row">{{ service.port }}</th>           
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'SubdomainServices',
    props: {
      parentServices: {
        type: Array,
        required: true
      }
    },
    data: () => {
      return {
        services: []        
      }
    },
    async mounted() {
      this.services = this.parentServices || []
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>