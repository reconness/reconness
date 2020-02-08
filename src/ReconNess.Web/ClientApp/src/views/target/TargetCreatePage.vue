<template>
  <div>
    <h3>New Target</h3>
    <target-form v-on:save="onSave" v-bind:isNew="true"></target-form> 
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
        this.$store.dispatch('createTarget', { api: this.$api, target: target })
          .then(() => {
            this.$router.push({ name: 'target', params: { targetName: target.name } })
          })
          .catch(error => {
            if (error) {
              alert(error)
            }
            else {
              alert("The Target cannot be added. Try again, please!")
            }
          });
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>