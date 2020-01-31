<template>
  <div>   
    <div class="pt-2 row">
      <div class="mx-auto"><strong>List of Services</strong></div>

      <div class="col-12">
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
    </div>
</template>

<script>
  export default {
    name: 'SubdomainServicesTag',
    props: {
      services: {
        type: Array,
        required: true
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>