<template>
  <div>
    <div class="pt-2 row">
      <div class="mx-auto"><strong>List of Directories</strong></div>

      <div class="col-12">
        <table class="table table-striped">
          <thead class="thead-dark">
            <tr>
              <th scope="col">Directory</th>
              <th scope="col">Method</th>
              <th scope="col">Status Code</th>
              <th scope="col">Size</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="directory in directories" v-bind:key="directory.id">
              <th class="w-25" scope="row">{{ directory.directory }}</th>
              <th class="w-25" scope="row">{{ directory.method }}</th>
              <th class="w-25" scope="row">{{ directory.statusCode }}</th>
              <th class="w-25" scope="row">{{ directory.size }}</th>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'SubdomainDirectories',
    props: {
      parentDirectories: {
        type: Array,
        required: true
      }
    },
    data: () => {
      return {
        directories: []
      }
    },
    async mounted() {
      this.directories = this.parentDirectories || []
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>